using Sirenix.OdinInspector;
using UnityEngine;
using System;
using UnityEngine.Playables;

public class MapGenerator : MonoBehaviour
{   
    #region References
    [Header("Debug")]
    [ShowInInspector] private Logger logger;
    private LevelManager levelManager;
    private Submit submitButton;
    #endregion

    #region Grid stats
    [SerializeField] private int firstCubeYPosition = 650;
    [SerializeField] private int firstCubeXPosition = -400;
    [SerializeField] private int lastRowYPos = -450;
    [SerializeField] private int gapBetweenCubes = 100;
    [SerializeField] private int gapBetweenRows = 100;
    #endregion

    [SerializeField] private GameObject cubeImage;
    [SerializeField] private GameObject clickableNumber;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Transform grid;
    private const int gridSize = 9;

    private readonly GameObject[,] sudokuBoardGameOjbects = new GameObject[gridSize, gridSize];
    private readonly GridPosition[,] sudokuBoardGridPositions = new GridPosition[gridSize, gridSize];

    private void Awake()
    {
        Init();
        CreateBoardsOnScreen();
    }

    private void Init()
    {
        logger = GameObject.Find("MapLogger").GetComponent<Logger>();
        levelManager = FindObjectOfType<LevelManager>();
        submitButton = FindObjectOfType<Submit>();
    }

    #region Subscribing and Unsubscribing
    private void OnEnable() 
    {
        levelManager.OnLevelChange += LevelManager_OnLevelChange;   
        levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
        submitButton.OnLevenCompleteWrongSolutionUIPopUp += SubmitButton_OnLevenCompleteWrongSolutionUIPopUp;
    }

    private void OnDisable()
    {
        levelManager.OnLevelChange -= LevelManager_OnLevelChange;
        levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
        submitButton.OnLevenCompleteWrongSolutionUIPopUp -= SubmitButton_OnLevenCompleteWrongSolutionUIPopUp;
    }
    #endregion

    #region Getters and Setters
    public GridPosition[,] GetSudokuBoardGridPositions()
    {
        int rows = sudokuBoardGridPositions.GetLength(0);
        int cols = sudokuBoardGridPositions.GetLength(1);
        GridPosition[,] copy = new GridPosition[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                copy[i, j] = sudokuBoardGridPositions[i, j];
            }
        }

        return copy;
    }
    #endregion

    private void LevelManager_OnLevelChange(SudokuDataContainer sudokuDataContainer)
    {
        SetBoardWithSudokuDataContainer(sudokuDataContainer);
    }

    private void CreateBoardsOnScreen()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject cube = Instantiate(cubeImage, grid);
                
                float xPos = firstCubeXPosition + j * gapBetweenCubes + mainCanvas.transform.position.x;
                float yPos = firstCubeYPosition - i * gapBetweenRows + mainCanvas.transform.position.y;
                cube.transform.position = new Vector3(xPos, yPos, 0);
                cube.name = "Cube (" + i + ", " + j + ")";
                sudokuBoardGameOjbects[i, j] = cube;

                sudokuBoardGridPositions[i, j] = cube.GetComponent<GridPosition>();
                sudokuBoardGridPositions[i, j].SetRowAndCol(i, j);
            }
        }

        CreateButtons();
    }

    // creates the non pressable buttons
    private void CreateButtons()
    {
        // Spawn the playerable buttons
        for(int i = 0; i < gridSize; i++)
        {
            GameObject clickable = Instantiate(clickableNumber, grid);
            float xPos = firstCubeXPosition + i * gapBetweenCubes + mainCanvas.transform.position.x;
            float yPos = lastRowYPos + mainCanvas.transform.position.y;
            clickable.transform.position = new Vector3(xPos, yPos, 0);
            clickable.name = "Clickable (Last Row Clickable: " + i + ")";
            GridPosition gridPos = clickable.GetComponent<GridPosition>();
            gridPos.SetNumText(i + 1);
        }
    }

    public void SetBoardWithNumbers(int [,] playableBoard)
    {
        int[,] sudokuBoard = playableBoard;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                sudokuBoardGridPositions[i, j].SetNumText(sudokuBoard[i, j]);
            }
        }
    }

    public void SetBoardWithSudokuDataContainer(SudokuDataContainer sudokuDataContainer)
    {
        int[,] sudokuBoard = sudokuDataContainer.GetPlayableBoard();
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                sudokuBoardGridPositions[i, j].SetNumText(sudokuBoard[i, j]);
            }
        }
    }

    
    private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
    {
        logger.Log("OnLevelRestartAfterWrongSolution: ResetVisibleBoard", this);
        ResetVisibleBoard(container);
    }

    
    private void ResetVisibleBoard(SudokuDataContainer container)
    {
        int[,] playableBoard = container.GetPlayableBoard();
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                sudokuBoardGridPositions[i, j].SetNumText(playableBoard[i, j]);
                // cancel any tween
                sudokuBoardGridPositions[i, j].CancelTween();
            }
        }

        logger.Log("OnLevelRestartAfterWrongSolution: ResetVisibleBoard = completed", this);
    }

    // Cancels all the tweens, the value is not used
    private void SubmitButton_OnLevenCompleteWrongSolutionUIPopUp()
    {
        logger.Log("SubmitButton_OnLevenCompleteWrongSolutionUIPopUp: Cancel Tweens", this);

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                sudokuBoardGridPositions[i, j].CancelTween();
            }
        }
    }

}
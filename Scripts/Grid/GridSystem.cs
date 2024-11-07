using Sirenix.OdinInspector;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    #region References
    private Player player;
    private MapGenerator mapGenerator;
    private LevelManager levelManager;
    
    [Header("Debug")]
    [ShowInInspector] private Logger logger;
    #endregion

    [ShowInInspector] private GridPosition currentPressedCube = null;
    [ShowInInspector] private GridPosition lastPressedCube = null;
    private const int gridSize = 9;
    private GridPosition[,] sudokuBoardGridPositions = new GridPosition[gridSize, gridSize];
    [ShowInInspector] private int[,] playable;

    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        logger = GameObject.Find("SystemLogger").GetComponent<Logger>();
        mapGenerator = FindObjectOfType<MapGenerator>();
        player = FindObjectOfType<Player>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void Start() 
    {
        sudokuBoardGridPositions = mapGenerator.GetSudokuBoardGridPositions(); 
    }

    #region Subscribing and Unsubscribing to events
    private void OnEnable()
    {
        player.OnNumberPlacedOnBoard += Player_OnNumberPlacedOnBoard;
        levelManager.OnLevelChange += LevelManager_OnLevelChange;
        levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
    }

    private void OnDisable()
    {
        player.OnNumberPlacedOnBoard -= Player_OnNumberPlacedOnBoard;
        levelManager.OnLevelChange -= LevelManager_OnLevelChange;
        levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
    }
    #endregion

    #region Getters and Setters
    public int[,] GetPlayableBoard() => playable;
    public GridPosition GetCurrentPressedCube() => currentPressedCube != null ?  currentPressedCube : null;
    public GridPosition GetLastPressedCube() => lastPressedCube != null ?  lastPressedCube : null;

    private void SetPlayableBoard(SudokuDataContainer sudokuDataContainer)
    {
        playable = sudokuDataContainer.GetPlayableBoard();
        logger.Log("GridSystem :: SetPlayableBoard with CurrentSudokuDataContainer", this);
    }
    
    public void SetCurrentPressedCube(GridPosition pressedCube)
    {
        if(currentPressedCube != null)
        {
            if(lastPressedCube != null)
                    lastPressedCube.ResetColor();


            lastPressedCube = currentPressedCube;
            lastPressedCube.ResetColor();
        }

        currentPressedCube = pressedCube;
    }

    #endregion
    
    private void Player_OnNumberPlacedOnBoard(int number)
    {
        PlaceNumberOnBoard(number);
    }

    private void LevelManager_OnLevelChange(SudokuDataContainer sudokuDataContainer)
    {
        SetPlayableBoard(sudokuDataContainer);
        ResetButtonsOnLevelChange();
    }

    private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
    {
        logger.Log("OnReloadLevelDataAfterSolution: GridSystem playable board reset", this);
        playable = container.GetPlayableBoard();
        ResetButtonsOnLevelChange();
    }

    public void PlaceNumberOnBoard(int number)
    {  
        if(currentPressedCube != null)
        {
            currentPressedCube.SetNumText(number);
            logger.Log("NUMBER CHANGED: " + number + "   Position: " + currentPressedCube.GetPoint().ToString(), this);
            
            int x = currentPressedCube.GetPoint().X;
            int y = currentPressedCube.GetPoint().Y;
            playable[x,y] = number;
        }
    }

    public void ResetCubeOnUndo(Point point)
    {
        sudokuBoardGridPositions[point.X, point.Y].SetNumText(0);

        if(lastPressedCube != null)
                lastPressedCube.ResetColor();
    }

    public void PlaceHintOnBoard(Point point, int number)
    {
        GridPosition hintGridPosition = sudokuBoardGridPositions[point.X, point.Y];
        hintGridPosition.SetNumText(number);
        playable[point.X, point.Y] = number;

        if(currentPressedCube != null)
                currentPressedCube.ResetColor();

        if(lastPressedCube != null)
                lastPressedCube.ResetColor();

        // show the hint on the board
        hintGridPosition.LerpColorOnHintEffect();
    }
    
    private void ResetButtonsOnLevelChange()
    {
        if(lastPressedCube != null)
                lastPressedCube.ResetColor();

        if (currentPressedCube != null)
                currentPressedCube.ResetColor();

        currentPressedCube = null;
        lastPressedCube = null;
    }
}

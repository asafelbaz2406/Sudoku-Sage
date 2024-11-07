using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Submit : MonoBehaviour
{
    #region References to other game objects
    private LevelManager levelManager;
    private GridSystem gridSystem;
    private Logger logger;
    #endregion

    #region UI References
    [SerializeField] private Button sumbitButton;
    [SerializeField] private RectTransform winScreenRectTransform;
    #endregion

    [Header("Data")]
    [ShowInInspector] private int[,] solution;
    [ShowInInspector] private int[,] playable;
    private bool isCheckingSolution = false;

    public event Action<bool> OnLevelCompleteSolutionUIPopUpTweenBackToStart;

    #region Events - Right solution
    public event Action OnLevelCompleteRightSolution;
    public event Action OnLevelCompleteRightSolutionUIPopUp;
    #endregion

    #region Events - Wrong solution
    public event Action OnLevenCompleteWrongSolutionUIPopUp;
    public event Action OnLevelCompleteWrongSolution;
    #endregion
    
    private void Awake() {
        levelManager = FindObjectOfType<LevelManager>();
        logger = GameObject.Find("SystemLogger").GetComponent<Logger>();
        gridSystem = FindObjectOfType<GridSystem>();
    }

    private void Start() 
    {
        SetPlayable(gridSystem.GetPlayableBoard());
    }

    #region Subscribing and Unsubscribing to events
    private void OnEnable()
    {
        sumbitButton.onClick.AddListener(() => SumbitButton_onClick()); 

        levelManager.OnLevelChange += LevelManager_OnLevelChange;
        levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
    }
    private void OnDisable() 
    {
        sumbitButton.onClick.RemoveListener(() => SumbitButton_onClick());

        levelManager.OnLevelChange -= LevelManager_OnLevelChange;
        levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
    }
    #endregion

    #region Stopping all tweens
    private void OnDestroy()
    {
        int killed = DOTween.Kill(gameObject);
        logger.Log($"Submit :: OnDestroy() {killed} ", this);
    }
    #endregion

    #region Getters and Setters
    public void SetPlayable(int[,] playableBoard) 
    {
        playable = playableBoard;

        logger.Log("Submit :: SetPlayable()", this);
    }
    #endregion

    private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer sudokuDataContainer)
    {
        SetSolution(sudokuDataContainer);
        logger.Log("Submit :: OnReloadLevelDataAfterSolution", this);
    }

    private void LevelManager_OnLevelChange(SudokuDataContainer sudokuDataContainer)
    {
        SetSolution(sudokuDataContainer);
        logger.Log("Submit :: LevelManager_OnLevelChange", this);
    }

    public void SetSolution(SudokuDataContainer sudokuDataContainer)
    {
        solution = sudokuDataContainer.GetSolutionBoard();
        logger.Log("Submit :: SetSolutionBoard with CurrentSudokuDataContainer", this);
    }

    private void SumbitButton_onClick()
    {
        CheckSolution();
    }

    public void CheckSolution()
    {
        if(isCheckingSolution)
        {
            logger.Log("Already checking solution", this);
            return;
        }
        
        isCheckingSolution = true;
        
        logger.Log("Checking solution", this);
        SetPlayable(gridSystem.GetPlayableBoard());

        int gridSize = playable.GetLength(0);
        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                if (playable[i, j] != solution[i, j])
                {
                    logger.Log("Wrong solution", this);
                    logger.Log($"Wrong solution: playable[{i}, {j}] = {playable[i, j]}, solution[{i}, {j}] = {solution[i, j]}", this);
                    
                    // pop up UI lose screen
                    OnLevenCompleteWrongSolutionUIPopUp?.Invoke();
                    return;
                }
            }
        }

        OnLevelCompleteRightSolutionUIPopUp?.Invoke();

        // should save the level and stars stats here
        if(levelManager.GetCurrentLevel() == GameManager.Instance.GetPlayableLevels() - 1) // if last level (one)
                GameManager.Instance.IncrementPlayableLevles();

        // save the level and stars stats after a win
        SavingSystem.Instance.SaveDataLevelsDictionary(GameManager.Instance.GetLevelDictionary());
    }


    // this is getting called from the restart button in the UI lose screen
    public void OnLevelCompleteWrongSolutionRestart()
    {
        isCheckingSolution = false;

        Fader fader = FindObjectOfType<Fader>();
        if(fader != null)
        {
            fader.FadeOnSubmit(OnLevelCompleteWrongSolution, OnLevelCompleteSolutionUIPopUpTweenBack, false);
        }else{
            logger.Log("ERROR   Fader not found", this);
        }
    }

    // this is getting called from the NextLevelButton in the UI win screen
    public void OnLevelCompletedButton()
    {
        isCheckingSolution = false;

        Fader fader = FindObjectOfType<Fader>();
        if(fader != null)
        {
            fader.FadeOnSubmit(OnLevelCompleteRightSolution, OnLevelCompleteSolutionUIPopUpTweenBack, true);
        }
        else{
            logger.Log("ERROR   Fader not found", this);
        }
    }  

    private void OnLevelCompleteSolutionUIPopUpTweenBack(bool rightSolution)
    {
        // tween back to the start position
        OnLevelCompleteSolutionUIPopUpTweenBackToStart?.Invoke(rightSolution);
    }
}
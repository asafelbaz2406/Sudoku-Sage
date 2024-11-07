using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region  References
    [SerializeField] private Submit submitButton;

    [Header("Debug")]
    [SerializeField] private Logger logger;
    #endregion

    [Header("Properties")]
    [ShowInInspector] private int currentLevel;
    public event Action<Dictionary<Point, int>> OnChangeHintDictionary;
    public event Action<SudokuDataContainer> OnLevelChange;
    [ShowInInspector] private List<SudokuDataContainer> sudokuDataContainersList;
    public event Action <SudokuDataContainer>OnReloadLevelDataAfterSolution;


    private void Awake()
    {
        // Game manager will always be in the scene before the level manager, so this will always work
        sudokuDataContainersList = GameManager.Instance.GetAllSudokuDataContainers();
    }

    private void Start() 
    {
        LoadLevel(GameManager.Instance.GetLevelToLoad());
    }

    #region Subscribing and Unsubscribing to events
    private void OnEnable() 
    {
        submitButton.OnLevelCompleteWrongSolution += SubmitButton_OnLevelCompleteWrongSolution;
        submitButton.OnLevelCompleteRightSolution += SubmitButton_OnLevelCompleteRightSolution;
    }
    
    private void OnDisable() 
    {
        submitButton.OnLevelCompleteWrongSolution -= SubmitButton_OnLevelCompleteWrongSolution;
        submitButton.OnLevelCompleteRightSolution -= SubmitButton_OnLevelCompleteRightSolution;
    }
    #endregion

    #region  Getters and Setters
    public int GetCurrentLevel() => currentLevel;
    public Dictionary<Point, int> GetCurrentLevelDeletedPositionsDictionary() => sudokuDataContainersList[currentLevel].GetDeletedPositions();
    public List<SudokuDataContainer> GetAllSudokuDataContainers() => new(sudokuDataContainersList);
    public int GetCellValueFromSolutionBoard(Point point) => sudokuDataContainersList[currentLevel].GetSolutionBoard()[point.X, point.Y];

    #endregion

    // load the current level data again
    private void SubmitButton_OnLevelCompleteWrongSolution()
    {
        logger.Log($"OnLevelCompleteWrongSolution: {sudokuDataContainersList[currentLevel]}, currentLevel: {currentLevel}", this);
        OnReloadLevelDataAfterSolution?.Invoke(sudokuDataContainersList[currentLevel]);
    }

    // load the next level data
    private void SubmitButton_OnLevelCompleteRightSolution()
    {
        int nextLevel = currentLevel + 1;
        logger.Log($"OnLevelCompleteRightSolution: {sudokuDataContainersList[nextLevel]}, nextLevel: {nextLevel}", this);
        OnReloadLevelDataAfterSolution?.Invoke(sudokuDataContainersList[nextLevel]);

        currentLevel++;
    }


    public void LoadLevel(int level)
    {
        // sudokuDataContainersList.Clear();   
        // sudokuDataContainersList = SavingSystem.Instance.LoadLevelsData();

        if(level < 1)
        {
            logger.Log("INVALID LEVEL: " + level, this);
            return;
        }

        if(level > sudokuDataContainersList.Count)
        {
            logger.Log("Creating a new level: " + level, this);
            return;
        }
        
        currentLevel = level - 1;

        var hintMap = sudokuDataContainersList[currentLevel].GetDeletedPositions();
        OnChangeHintDictionary?.Invoke(hintMap);

        OnLevelChange?.Invoke(sudokuDataContainersList[currentLevel]);
    }


    // this function is used to add a new level to the list, which will be saved to the json file
    // but I created 200 levels throughout the Inspector, and this function is no longer needed
    /*
    // public void AddLevelToSudokuDataContainers(SudokuDataContainer sudokuDataContainer)
    // {
    //     sudokuDataContainersList.Add(sudokuDataContainer);

    //     SavingSystem.Instance.SaveDataLevels(sudokuDataContainersList);
    //     SavingSystem.Instance.SaveDataLevelsDictionary(GameManager.Instance.GetLevelDictionary());
    // }
    */
}
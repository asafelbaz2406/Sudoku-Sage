using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System;

public class Undo : MonoBehaviour
{
    #region References
    [SerializeField] private Player player;
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private LevelManager levelManager;

    [Header("Debug")]
    [ShowInInspector] private Logger logger;
    #endregion
    [ShowInInspector] private readonly Stack<Point> undoStack = new();
    
    public event Action<Point,int> OnUndoAddPositionToHintDictionary;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        logger = GameObject.Find("SystemLogger").GetComponent<Logger>();
    }

    #region Subscribing / Unsubscribing to events
    private void Start() 
    {
        player.OnNumberPlacedOnBoardStack += Player_OnNumberPlacedOnBoardStack;
        levelManager.OnLevelChange += LevelManager_OnLevelChange;
        levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
    }

    private void OnDisable() 
    {
        player.OnNumberPlacedOnBoardStack -= Player_OnNumberPlacedOnBoardStack;
        levelManager.OnLevelChange -= LevelManager_OnLevelChange;
        levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
    }
    #endregion

    
    private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
    {
        logger.Log("OnReloadLevelDataAfterSolution: RemoveAllPointsFromUndoStack", this);
        RemoveAllPointsFromUndoStack();
    }

    private void LevelManager_OnLevelChange(SudokuDataContainer sudokuDataContainer)
    {
        logger.Log("OnLevelChange: RemoveAllPointsFromUndoStack", this);
        RemoveAllPointsFromUndoStack();
    }

    private void RemoveAllPointsFromUndoStack()
    {
        undoStack.Clear();
    }


    // this is getting called from the UNDO button
    public void UndoAction()
    {
        logger.Log("Undo", this);
        if(undoStack.Count == 0)
        {
            logger.Log("Nothing to undo", this);
            return;
        }

        Point point = undoStack.Pop();
        logger.Log("Undo: " + point.ToString(), this);

        gridSystem.ResetCubeOnUndo(point);

        // undo = return the last number placed on the board to the hint dictionary
        int number = levelManager.GetCellValueFromSolutionBoard(point);
        OnUndoAddPositionToHintDictionary?.Invoke(point, number);
    }

    private void Player_OnNumberPlacedOnBoardStack(Point point)
    {
        AddPointToUndoStack(point);
    }

    public void AddPointToUndoStack(Point point)
    {
        logger.Log("AddPointToUndoStack: " + point.ToString(), this);
        undoStack.Push(point);
    }

}

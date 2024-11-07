using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class Hint : MonoBehaviour
{
    #region References
    [SerializeField] private Player player;
    [SerializeField] private Undo undo;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private Submit submitButton;
    #endregion

    [ShowInInspector] private Dictionary<Point, int> hintMap = new();
    [ShowInInspector] private List<Point> hintList = new();
    private readonly int originalHintCounter = 40;
    private int hintCounter = 0;
    public event Action OnHintUsed;

    [Header("Debug")]
    [ShowInInspector] private Logger logger;

    private void Awake()
    {
        Init();
        hintCounter = originalHintCounter;
    }

    private void Init()
    {
        logger = GameObject.Find("SystemLogger").GetComponent<Logger>();
    }

    private void Start() 
    {
        CreateDictionary();
    }

    #region Subscribing / Unsubscribing to events
    private void OnEnable()
    {
        player.OnNumberPlacedOnBoardDeletedPosition += Player_OnNumberPlacedOnBoardDeletedPosition;
        undo.OnUndoAddPositionToHintDictionary += Undo_OnUndoAddPositionToHintDictionary;
        levelManager.OnChangeHintDictionary += LevelManager_OnChangeLevelChangeHintDictionary;
        levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
    }

    private void OnDisable()
    {
        player.OnNumberPlacedOnBoardDeletedPosition -= Player_OnNumberPlacedOnBoardDeletedPosition;
        undo.OnUndoAddPositionToHintDictionary -= Undo_OnUndoAddPositionToHintDictionary;
        levelManager.OnChangeHintDictionary -= LevelManager_OnChangeLevelChangeHintDictionary;
        levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
    }
    #endregion

    #region Getters and Setters
    public int GetHintCounter() => hintCounter;
    public int GetMaxHintCounter() => originalHintCounter;
    #endregion

    private void Undo_OnUndoAddPositionToHintDictionary(Point point, int number)
    {
        AddHintToDictionaryFromUndoStack(point, number);
    }

    public void AddHintToDictionaryFromUndoStack(Point point, int number)
    {
        if(!hintMap.ContainsKey(point))
        {
            hintMap.Add(point, number);
            if(!hintList.IsNullOrEmpty())
            {
                hintList.Add(point);
                logger.Log($"hintList is not empty. Count: {hintList.Count}, point added: {point}", this);
            }
            else
            {
                // Convert the dictionary keys to a list
                hintList = new List<Point>(hintMap.Keys);
                logger.Log("AddHintToDictionaryFromUndoStack   hintList: Created!. Count: " + hintList.Count, this);
            }

            logger.Log("AddHintToDictionary: " + point.ToString(), this);
            logger.Log("AddHintToHintList: " + point.ToString(), this);
        }
    }

    private void CreateDictionary()
    {
        hintMap = levelManager.GetCurrentLevelDeletedPositionsDictionary();
    }

    // this is getting called from the Hint button
    public void ShowHintAndRemoveFromDictionary()
    {
        Point point = GetRandomPointFromDictionary();
        if(point == null || !hintMap.ContainsKey(point))
        {
            logger.Log("Can't find hint on this position", this);
            return;
        }
        if(hintCounter == 0)
        {
            logger.Log("hintCounter is 0, can't get any more hints", this);
            return;
        }

        gridSystem.PlaceHintOnBoard(point, hintMap[point]);
         
        hintMap.Remove(point);
        hintCounter--;

        if(hintMap.Count > 0 && hintCounter == 0)
        {
            hintMap.Clear();
            logger.Log("hintMap cleared", this);
        }

        // update the hint counter UI
        OnHintUsed?.Invoke();

        logger.Log("hintCounter: " + hintCounter, this);
    }

    private void Player_OnNumberPlacedOnBoardDeletedPosition(Point point)
    {
        RemoveHintFromDictionary(point);
    }
    
    private void RemoveHintFromDictionary(Point point)
    {
        logger.Log("RemoveHintFromDictionary: " + point.X + ", " + point.Y, this);
        hintMap.Remove(point);

        if(!hintList.IsNullOrEmpty())
                hintList.Remove(point);
    }

    private Point GetRandomPointFromDictionary()
    {
        if (hintMap.Count == 0)
        {
            logger.Log("The hintMap is empty. Cannot get a random point.", this);
            return null; 
        }

        // Convert the dictionary keys to a list
        if(hintList.IsNullOrEmpty())
        {
            hintList = new List<Point>(hintMap.Keys);
            logger.Log("hintList: Created!. Count: " + hintList.Count, this) ;
        }
        
        int randomIndex = UnityEngine.Random.Range(0, hintList.Count);


        Point result = hintList[randomIndex];
        hintList.RemoveAt(randomIndex);

        return result;
    }

    private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
    {
        logger.Log($"OnReloadLevelDataAfterSolution: reset hintMap with the current level: {container.GetLevel()}", this);
        hintMap.Clear();
        hintList.Clear();
        hintMap = container.GetDeletedPositionsDictionary();
        logger.Log("hintMap: " + hintMap.Count, this);
        hintCounter = originalHintCounter;
    }
     

    [Button("Change hint map")]
    public void LevelManager_OnChangeLevelChangeHintDictionary(Dictionary<Point, int> deletedPositions)
    {
        hintMap = deletedPositions;
    }
}

using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour
{
    [ShowInInspector] private int numberToPlaceOnBoard = 0;
    public event Action<int> OnNumberPlacedOnBoard;
    public event Action<Point> OnNumberPlacedOnBoardStack;
    public event Action<Point> OnNumberPlacedOnBoardDeletedPosition;

    [Header("Debug")]
    [ShowInInspector] private Logger logger;
    private GridSystem gridSystem;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        logger = GameObject.Find("PlayerLogger").GetComponent<Logger>();
        gridSystem = FindObjectOfType<GridSystem>();
    }

    public void SetNumberToPlaceOnBoard(int number) 
    {
        numberToPlaceOnBoard = number;
        logger.Log("SetNumberToPlaceOnBoard(): " + numberToPlaceOnBoard, this);
    }

    public void ResetNumber() => numberToPlaceOnBoard = 0;

    public void PlaceNumberOnBoard()
    {
        GridPosition currentPressedCube = gridSystem.GetCurrentPressedCube();
        Point currentPressedCubePoint = currentPressedCube.GetPoint();
        if(numberToPlaceOnBoard != 0 && currentPressedCube.IsEmpty())
        {
            OnNumberPlacedOnBoard?.Invoke(numberToPlaceOnBoard);
            OnNumberPlacedOnBoardStack?.Invoke(currentPressedCubePoint);
            OnNumberPlacedOnBoardDeletedPosition?.Invoke(currentPressedCubePoint);

            ResetNumber();
        }
    }
}

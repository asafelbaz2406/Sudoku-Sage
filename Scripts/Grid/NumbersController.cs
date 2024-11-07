using UnityEngine;

public class NumbersController : MonoBehaviour
{
    [SerializeField] private GridPosition gridPosition;
    private Player player;
    private GridSystem gridSystem;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        player = FindObjectOfType<Player>();
        gridSystem = FindObjectOfType<GridSystem>();
    }

    // this is getting called upon pressing on the numbers buttons
    public void OnPress()
    {
        gridSystem.SetCurrentPressedCube(gridPosition);
        
        gridPosition.GetLogger().Log(gridPosition.GetNum().ToString(), this);

        player.SetNumberToPlaceOnBoard(gridPosition.GetNum());

        ChangeColor();

        ToString();       
    }

    public override string ToString()
    {
        string newString = "NumbersController :: This is a pressable number";
        gridPosition.GetLogger().Log(newString, this);   
        return newString;
    }

    private void ChangeColor()
    {
        gridPosition.ChangeColor();
    }
}

using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Color32 pressedColor;
    [SerializeField] private Color32 unpressedColor;
    [SerializeField] private Color32 hintColor;
    private bool isPressed = false;
    private Player player;
    
    [Header("Debug")]
    [ShowInInspector] private Logger logger;
    
    [ShowInInspector] private int Row {get; set;}
    [ShowInInspector] private int Col {get; set;}
    [Header("Data")]
    [ShowInInspector] private int num;
    private GridSystem gridSystem;
    private bool isTweening = false;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        player = FindObjectOfType<Player>();
        logger = GameObject.Find("MapLogger").GetComponent<Logger>();
        gridSystem = FindObjectOfType<GridSystem>();
    }

    private void Start() 
    {
        backgroundImage.color = unpressedColor;
    }

    #region Getters and Setters
    public Logger GetLogger() => logger;
    public int GetNum() => num; 
    public bool IsEmpty() => num == 0;
    public Point GetPoint() => new(Row, Col);
    public void SetRowAndCol(int row, int col)
    {
        Row = row;
        Col = col;
    }
    #endregion

    #region Cancel Tween
    void OnDestroy()
    {
        CancelTween();
    }

    public void CancelTween()
    {
        if (backgroundImage != null && isTweening)
        {
            backgroundImage.DOKill(); // Cancels any active tween on backgroundImage

            backgroundImage.color = unpressedColor;
            logger.Log("Canceled tween", this);
        }
    }
    #endregion

    public void SetNumText(int number)
    {
        num = number;
        if(number == 0)
        {
            numberText.text = "";
        }
        else
        {
            numberText.text = number.ToString();
        }
    }
    
    public override string ToString()
    {
        string positionData = "Row: " + Row + ", Col: " + Col + ", Num: " + num;
        logger.Log(positionData, this);   
        return positionData;
    }

    // this is getting called upon pressing on the grid position
    public void ChangeColorOnPress()
    {
        gridSystem.SetCurrentPressedCube(this);

        player.PlaceNumberOnBoard();

        ChangeColor();
        ToString();       
    }

    public void ChangeColor()
    {
        isPressed = !isPressed;
        logger.Log($"isPressed: {isPressed}", this);
        if(isPressed)
        {
            backgroundImage.color = pressedColor;
        }
        else
        {
            ResetColor();
        }
    }

    public void ResetColor()
    {
        isPressed = false;
        backgroundImage.color = unpressedColor;
    }

    public void LerpColorOnHintEffect()
    {
        if(backgroundImage != null && !isTweening)
        {
            isTweening = true;

            float duration = 0.75f; // duration for each color change
            Color32 startColor = backgroundImage.color; // the original color of the background

            // Animate the background color to targetColor and back to startColor three times
            backgroundImage.DOColor(hintColor, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(6, LoopType.Yoyo) // 6 loops (3 cycles back and forth)
            .OnComplete(() => ResetColorOnTweenComplete(startColor)); // Reset to original color

            logger.Log("Lerping color", this);
        }
    }

    private void ResetColorOnTweenComplete(Color32 startColor)
    {
        if (backgroundImage != null)
            backgroundImage.color = startColor;
    }

}

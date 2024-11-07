using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelMapCreator : MonoBehaviour
{   
    private const int width = 5, height = 8;

    #region References
    [Header("References")]
    [SerializeField] private Transform levelMap;
    [SerializeField] private Level levelImage;
    [SerializeField] private Logger logger;
    [SerializeField] private Fader fader;
    #endregion

    [Header("Level Map Properties")]
    [SerializeField] private int firstRowYPos = 700;
    [SerializeField] private int firstXPos = -400;
    [SerializeField] private int widthGapBetweenLevels = 200;
    [SerializeField] private int heightGapBetweenLevels = 175;

    private const int MaxLevelMapCounter = 5;
    [SerializeField] private int leveMapCounter = 1, levelCounter = 1;

    private readonly Level[] levels = new Level[width * height];
    private readonly Tweenable[] tweenables = new Tweenable[width * height];

    public event Action<LevelChange> OnMapLevelChange;


    void Start()
    {
        CreateBoardsOnScreen();
    }

    #region Getters and Setters
    public int GetLevelMapCounter() => leveMapCounter;
    public int GetMaxLevelCounter() => MaxLevelMapCounter;
    #endregion
    
    public void CreateBoardsOnScreen()
    {
        int levelsCompleted = GameManager.Instance.GetPlayableLevels();

        // creates the board
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Level level = Instantiate(levelImage, levelMap);
                float xPos = firstXPos + j * widthGapBetweenLevels;
                float yPos = firstRowYPos - i * heightGapBetweenLevels;
                level.transform.localPosition = new Vector3(xPos, yPos, 0);
                
                level.SetLevelNumberText(levelCounter);

                level.SetAvailable(levelCounter <= levelsCompleted);

                levels[levelCounter - 1] = level;

                tweenables[levelCounter - 1] = level.GetComponentInChildren<Tweenable>();

                level.ChangeStars(GameManager.Instance.GetStarsInSpecificLevel(levelCounter));

                levelCounter++;
            }
        }
    }

    // this is getting called from the -> button
    public void NextLevelMap()
    {
        if(!fader.GetCanTween()) 
        {
            logger.Log("fader.GetCanTween() == false", this);
            return;
        }

        if(leveMapCounter > 4)
        {
            logger.Log("Max level reached: " + (levelCounter + ((leveMapCounter - 1) * width * height)) + " " + 
                                              (levelCounter + ((leveMapCounter - 1) * width * height) == 201), this);
            return;
        } 

        OnMapLevelChange?.Invoke(LevelChange.next);

        FadeInBeforeMapTween(LevelChange.next);
        leveMapCounter++;
    }

    // this is getting called from the <- button
    public void PreviousLevelMap()
    {
        if(!fader.GetCanTween()) 
        {
            logger.Log("fader.GetCanTween() == false", this);
            return;
        }

        if(leveMapCounter <= 1)
        {
            logger.Log("leveMapCounter >= 1", this);
            return;
        }

        OnMapLevelChange?.Invoke(LevelChange.previous);

        FadeInBeforeMapTween(LevelChange.previous);
        leveMapCounter--;
    }

    public void FadeInBeforeMapTween(LevelChange levelChange)
    {
        if(fader != null)
        {
            fader.FadeOutLevelMap(ChangeLevelsLoop, levelChange);
        }
    }

    private void ChangeLevelsLoop(LevelChange levelChange)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            ChangeLevels(i, levelChange);
        }
    }

    private void ChangeLevels(int i, LevelChange levelChange)
    {
        int levelsCompleted = GameManager.Instance.GetPlayableLevels();
        int number = levels[i].GetLevelNumberText();

        int addOrSubMultiplier = levelChange == LevelChange.next? 1 : -1;
        int gridSize = width * height;

        int newNumber = number + (addOrSubMultiplier * gridSize);

        levels[i].SetLevelNumberText(newNumber);
        levels[i].SetAvailable(newNumber <= levelsCompleted);
    }
}

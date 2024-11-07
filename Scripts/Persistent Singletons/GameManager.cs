using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    static public GameManager Instance => instance;
    //[ShowInInspector] private Dictionary<int, LevelDetails> levelDic = new();
    [ShowInInspector] private Dictionary<int, int> levelStarsDic = new();
    [SerializeField] private int playableLevels = 1;
    [ShowInInspector] private int levelToLoad = 0;
    [ShowInInspector] private int totalStarsCollected = 0;

    [ShowInInspector] private List<SudokuDataContainer> sudokuDataContainersList = new(); 

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("THERES MORE THAN 1 GAME MANAGER");   
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() 
    {
        sudokuDataContainersList = SavingSystem.Instance.LoadLevelsData();
        levelToLoad = 1;
        
        levelStarsDic = SavingSystem.Instance.LoadLevelDictionary();
        playableLevels = levelStarsDic?.Count > 0 ?
                         levelStarsDic.Max(x => x.Key) + 1 :
                         1;

        totalStarsCollected = CalculateTotalStarsInLevelStarDic();
    }

    #region Getters and Setters

    public int GetLevelToLoad() => levelToLoad;
    public int GetPlayableLevels() => playableLevels;
    public void SetPlayableLevels(int playableLevels) => this.playableLevels = playableLevels;
    public Dictionary<int, int> GetLevelDictionary() => new(levelStarsDic);
    public List<SudokuDataContainer> GetAllSudokuDataContainers() => new(sudokuDataContainersList); 
    public void SetStarsInSpecificLevel(int level, int stars) => levelStarsDic[level] = stars;
    public int GetStarsInSpecificLevel(int level) => levelStarsDic.ContainsKey(level) ? levelStarsDic[level] : 0;
    
    #endregion

    public void IncrementPlayableLevles() => playableLevels++;
    public void IncrementTotalStarsCollected(int starsCollected) => totalStarsCollected += starsCollected; 
    public int GetTotalStarsCollected() => totalStarsCollected;

    public void SetLevelToLoad(int level)
    {
        if(level > playableLevels)
        {
            Debug.Log($"This Level is invalid! {level} > {playableLevels}", this);
            return;
        }

        levelToLoad = level;
        
        if(levelStarsDic.ContainsKey(level))
        {
            Debug.Log("levelStarsDic contains key: " + level);
        }
        else
        {
            levelStarsDic.Add(level, 0);
        }

        Fader fader = FindObjectOfType<Fader>();
        if(fader != null)
        {
            fader.FadeOutBetweenLevels(LevelChange.next);
            return;
        }
        SceneManager.LoadSceneAsync(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex + 1);
    }

    // this is getting called after winning a level
    public void BackToMenuOnWin()
    {
        IncrementPlayableLevles();

        // The Gamemanager is living in all the scenes, so we have to find the current active Fader
        Fader fader = FindObjectOfType<Fader>();
        if(fader != null)
        {
            fader.FadeOutBetweenLevels(LevelChange.next);
            return;
        }

        SceneManager.LoadSceneAsync(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex - 1);
    }

    private int CalculateTotalStarsInLevelStarDic()
    {
        int total = 0;
        foreach(var level in levelStarsDic)
        {
            total += level.Value;
        }

        return total;
    }
}

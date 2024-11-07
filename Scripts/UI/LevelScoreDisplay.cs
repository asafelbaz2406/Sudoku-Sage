using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class LevelScoreDisplay : MonoBehaviour
{
    #region References
    [SerializeField, Required] private LevelManager levelManager;
    [SerializeField, Required] private TextMeshProUGUI scoreText;
    #endregion

    #region Subscribing And Unsubscribing To Events
    private void OnEnable() 
    {
        levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
        levelManager.OnLevelChange += LevelManager_OnLevelChange;
    }

    private void OnDisable() 
    {
        levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
        levelManager.OnLevelChange -= LevelManager_OnLevelChange;
    }
    #endregion

    private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
    {
        SetScoreText(container.GetLevel());
    }
    
    private void LevelManager_OnLevelChange(SudokuDataContainer container)
    {
        SetScoreText(container.GetLevel());
    }

    private void SetScoreText(int level) 
    {
        int showLevel = level + 1; // + 1 because the level is 0-indexed
        scoreText.text = $"Level: {showLevel}";
    }
}

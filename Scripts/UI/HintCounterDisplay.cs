using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class HintCounterDisplay : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] private Hint hint;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private TextMeshProUGUI hintCounterText;
    #endregion

    [Header("Properties")]
    [ShowInInspector] private int maxHintCounter = 0;
    [ShowInInspector] private int currentHintcounter = 0;

    private void Start() 
    {
        if (hint == null) return;

        maxHintCounter = hint.GetMaxHintCounter();
        currentHintcounter = hint.GetHintCounter();

        UpdateHintCounterUI();
    }

    #region Subscribing and Unsubscribing to Events
    private void OnEnable() 
    {
        hint.OnHintUsed += Hint_OnHintUsed;
        levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
    }

    private void OnDisable()
    {
        hint.OnHintUsed -= Hint_OnHintUsed;
        levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
    }
    #endregion
    
    private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
    {
        // reset the hint counter
        currentHintcounter = maxHintCounter;
        UpdateHintCounterUI();
    }

    private void Hint_OnHintUsed()
    {
        currentHintcounter--;
        UpdateHintCounterUI();
    }

    private void UpdateHintCounterUI()
    {
        hintCounterText.text = $"{currentHintcounter}/{maxHintCounter}";
    }
}

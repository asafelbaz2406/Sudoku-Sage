using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class MapCounterDisplay : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] private LevelMapCreator levelMapCreator;    
    [SerializeField] private TextMeshProUGUI mapCounterText;
    [SerializeField] private Fader fader;
    #endregion

    [Header("Properties")]
    [ShowInInspector] private int maxMapCounter = 0;
    [ShowInInspector] private int currentMapCounter = 0;

    private void Start() 
    {
        if (levelMapCreator == null) return;

        maxMapCounter = levelMapCreator.GetMaxLevelCounter();
        currentMapCounter = levelMapCreator.GetLevelMapCounter();

        UpdateHintCounterUI();
    }

    #region Subscribing and Unsubscribing to Events
    private void OnEnable() 
    {
        fader.OnFadeCompleteLevelChange += Fader_OnFadeCompleteLevelChange;
    }

    private void OnDisable()
    {
        fader.OnFadeCompleteLevelChange -= Fader_OnFadeCompleteLevelChange;
    }

    #endregion

    private void Fader_OnFadeCompleteLevelChange(LevelChange levelChange)
    {
        if(levelChange == LevelChange.next)
                currentMapCounter++;
        else
                currentMapCounter--;

        UpdateHintCounterUI();
    }
    private void UpdateHintCounterUI()
    {
        mapCounterText.text = $"MAP: {currentMapCounter}/{maxMapCounter}";
    }

    // x = 350
    // width = 400
    // font = 60
    // MAP: 1/5
}

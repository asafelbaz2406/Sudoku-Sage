using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fader : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeImageCanvasGroup;
    private readonly float tweenDuration = 0.4f;
    private bool canTween = true;
    public event Action<LevelChange> OnFadeCompleteLevelChange;

    private void Start() 
    {
        fadeImageCanvasGroup.alpha = 1;
        FadeInAfterMapTween(tweenDuration);
    }

    #region Destroy Tweens OnDestroy
    private void OnDestroy()
    {
        DOTween.Kill(fadeImageCanvasGroup);
    }
    #endregion

    #region Getters and Setters
    public bool GetCanTween() => canTween;
    #endregion

    // callback1 = OnLevelCompleteRightSolution / OnLevelCompleteWrongSolution
    // callback2 = OnLevelCompleteSolutionUIPopUpTweenBack
    // rightSolution = true / false
    public void FadeOnSubmit(Action callback1, Action<bool> callback2, bool rightSolution)
    {
        if(!canTween) return;

        canTween = false;
        
        fadeImageCanvasGroup.DOFade(1, tweenDuration)
            .OnComplete(() =>
            {
                callback1?.Invoke();

                callback2?.Invoke(rightSolution);
                 
                FadeInAfterMapTween(tweenDuration);
            });
    }

    public void FadeOutLevelMap(Action<LevelChange> callback, LevelChange levelChange)
    {
        if(!canTween) return;

        canTween = false;
        
        fadeImageCanvasGroup.DOFade(1, tweenDuration)
            .OnComplete(() =>
            {
                callback?.Invoke(levelChange);
                OnFadeCompleteLevelChange?.Invoke(levelChange);
                FadeInAfterMapTween(tweenDuration);
            });
    }

    public void FadeOutBetweenLevels(LevelChange levelChange)
    {
        if(!canTween) return;

        int levelToLoad = levelChange == LevelChange.next
                          ? SceneManager.GetActiveScene().buildIndex + 1 
                          : SceneManager.GetActiveScene().buildIndex - 1;

        // Invalid level
        if(levelToLoad < 0 || levelToLoad >= SceneManager.sceneCountInBuildSettings) return;

        canTween = false;

        fadeImageCanvasGroup.DOFade(1, tweenDuration)
            .OnComplete(() =>
            {
                SceneManager.LoadSceneAsync(sceneBuildIndex: levelToLoad);
            });
    }

    private void FadeInAfterMapTween(float tweenDuration)
    {
        // fade in after switching the level
        fadeImageCanvasGroup.blocksRaycasts = true;
        fadeImageCanvasGroup.DOFade(0, tweenDuration)
            .OnComplete(() => UnblockRayCastsOnFadeComplete());
    }

    private void UnblockRayCastsOnFadeComplete()
    {
        fadeImageCanvasGroup.blocksRaycasts = false;
        canTween = true;
        Debug.Log("FADER:: canTween: " + canTween);  
        Debug.Log("FADER:: blocksRaycasts: " + fadeImageCanvasGroup.blocksRaycasts); 
    }
}
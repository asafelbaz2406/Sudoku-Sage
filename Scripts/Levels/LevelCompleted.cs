using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleted : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Submit submitButton;
    [SerializeField] private Hint hint;
    [SerializeField] private Logger logger;
    [SerializeField] private Image[] starsChildren;
    #endregion

    private const float duration = 1f;
    private int hintsRemaining;
    [SerializeField] private bool isLevelCompleted = false;


    #region Subscribing and Unsubscribing to submit.OnLevelCompleteRightSolution
    private void OnEnable() 
    {
        submitButton.OnLevelCompleteRightSolutionUIPopUp += SubmitButton_OnLevelCompleteRightSolutionUIPopUp;  
        submitButton.OnLevenCompleteWrongSolutionUIPopUp += SubmitButton_OnLevenCompleteWrongSolutionUIPopUp;  
        submitButton.OnLevelCompleteSolutionUIPopUpTweenBackToStart += SubmitButton_OnLevelCompleteSolutionUIPopUpTweenBackToStart;
        
    }

    private void OnDisable()
    {
        submitButton.OnLevelCompleteRightSolutionUIPopUp -= SubmitButton_OnLevelCompleteRightSolutionUIPopUp;
        submitButton.OnLevenCompleteWrongSolutionUIPopUp -= SubmitButton_OnLevenCompleteWrongSolutionUIPopUp;  
        submitButton.OnLevelCompleteSolutionUIPopUpTweenBackToStart -= SubmitButton_OnLevelCompleteSolutionUIPopUpTweenBackToStart;
    }
    #endregion

    #region Stopping all tweens
    private void OnDestroy()
    {
        // Stops all tweens associated with this GameObject
        DOTween.Kill(gameObject);
        DOTween.Kill(rectTransform);
    }
    #endregion

    private void UpdateStarsWithHintsValues()
    {
        hintsRemaining = hint.GetHintCounter();

        // update stars
        for (int i = 0; i < starsChildren.Length; i++)
        {
            if (!starsChildren[i].TryGetComponent<Star>(out var star)) continue;

            star.ChangeSprite(i < hintsRemaining);
        }


        int currentLevel = GameManager.Instance.GetLevelToLoad();
        // this equals 0 if the level isn't solved yet
        int previousStarsIfLevelAlreadySolved = GameManager.Instance.GetStarsInSpecificLevel(currentLevel);

        // save the highest score of the level
        if(hintsRemaining > previousStarsIfLevelAlreadySolved)
        {
            logger.Log($"BEFORE    Total stars collected: {GameManager.Instance.GetTotalStarsCollected()}", this);
            int newScore = hintsRemaining - previousStarsIfLevelAlreadySolved;
            GameManager.Instance.IncrementTotalStarsCollected(newScore);

            // set new record
            logger.Log($"AFTER    Total stars collected: {GameManager.Instance.GetTotalStarsCollected()}", this);
        }

        GameManager.Instance.SetStarsInSpecificLevel(currentLevel, hintsRemaining);
    }

    /// <summary>
    ///  Since there are 2 UI popups, we need to tween them back to their start positions
    ///  On the Win UI popup, isLevelCompleted is true
    ///  On the Lose UI popup, isLevelCompleted is false
    /// </summary>
    private void SubmitButton_OnLevelCompleteRightSolutionUIPopUp()
    {
        if(isLevelCompleted == false) return; // only the win screen is getting called after this line of code

        float centerXPos = 0f;
        rectTransform.DOLocalMoveX(centerXPos, duration).SetEase(Ease.OutBounce);

        UpdateStarsWithHintsValues();
    }
    
    private void SubmitButton_OnLevelCompleteSolutionUIPopUpTweenBackToStart(bool rightSolution)
    {
        float outOfScreenXPos = -1000f;
        if(rightSolution == true && isLevelCompleted == true) // right solution
        {
            rectTransform.DOLocalMoveX(outOfScreenXPos, duration).SetEase(Ease.OutBack);
        }
        else if(rightSolution == false && isLevelCompleted == false) // wrong solution
        {
            rectTransform.DOLocalMoveX(-outOfScreenXPos, duration).SetEase(Ease.OutBack);
        }
    }
    
    private void SubmitButton_OnLevenCompleteWrongSolutionUIPopUp()
    {
        if(isLevelCompleted == true) return;  // only the lose screen is getting called after this line of code,
                                              // the isLevelCompleted is assigned in the inspector

        float centerXPos = 0f;
        rectTransform.DOLocalMoveX(centerXPos, duration).SetEase(Ease.OutBounce);
    }

}

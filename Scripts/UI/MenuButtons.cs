using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void StartGame()
    { 
        SceneManager.LoadSceneAsync(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackToPreviousScene()
    {
        Debug.Log("Back To Previous Scene");
        Fader fader = FindObjectOfType<Fader>();
        if (fader != null)
        {
            fader.FadeOutBetweenLevels(LevelChange.previous);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}

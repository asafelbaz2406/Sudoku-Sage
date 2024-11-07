using TMPro;
using UnityEngine;

public class TotalStarsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalStarsText;

    private void Start()
    {
        // this is okay because the game manager will always be initialized in previous scenes
        int totalStarsCollected = GameManager.Instance.GetTotalStarsCollected();
        int numberOfLevels = 200;
        int maxStars = numberOfLevels * 3; // 3 stars per level
        totalStarsText.text = $"{totalStarsCollected}/{maxStars}";
    }
}

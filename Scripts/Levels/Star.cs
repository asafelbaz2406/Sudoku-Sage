using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    [SerializeField] private Sprite availableStar, unavailableStar;

    private Image starImage;

    private void Awake() 
    {
        starImage = GetComponent<Image>();    
    }

    public void ChangeSprite(bool available)
    {
        starImage.sprite = available ? availableStar : unavailableStar;
    }
}

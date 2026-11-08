using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("UI Text References")]
    public TextMeshProUGUI collectibleText;
    public TextMeshProUGUI livesText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateLives(int currentLives)
    {
        if (livesText != null)
        {
            livesText.text = currentLives.ToString();
        }
    }

    public void UpdateCollectibles(int currentCollectibles)
    {
        if (collectibleText != null)
        {
            collectibleText.text = currentCollectibles.ToString();
        }
    }
}

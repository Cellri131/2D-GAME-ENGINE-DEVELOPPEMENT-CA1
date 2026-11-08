using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    [Header("Victory Settings")]
    [Tooltip("Exact name of the Victory scene or next level to load.")]
    public string nextSceneName = "VictoryScene";

    [Tooltip("Delay in seconds before switching scenes.")]
    public float loadDelay = 0.5f;

    private bool isLevelCompleted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering the zone is the Player and prevent double triggers
        if (!isLevelCompleted && collision.CompareTag("Player"))
        {
            isLevelCompleted = true;
            Debug.Log("Level Completed!");
            
            // Trigger the victory action after a short delay
            Invoke(nameof(CompleteLevel), loadDelay);
        }
    }

    private void CompleteLevel()
    {
        // Load the victory screen or next level if a name is provided
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // Fallback: freeze the game if no scene is set yet
            Time.timeScale = 0f;
        }
    }
}

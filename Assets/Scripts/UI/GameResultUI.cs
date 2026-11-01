using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] private GameStateController gameState;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject panel;
    private void RefreshResult(LevelState value)
    {
        if (value == LevelState.Defeat)
        {
            panel.SetActive(true);
            resultText.text = "Defeat";
        }else if (value == LevelState.Victory)
        {
            panel.SetActive(true);
            resultText.text = "Victory!";
        }
        else
        {
            panel.SetActive(false);
            resultText.text = "";
        }
    }
    private void OnEnable()
    {
        gameState.StateChanged += RefreshResult;
        RefreshResult(gameState.State);
    }
    private void OnDisable()
    {
        if (gameState != null)
        {
            gameState.StateChanged -= RefreshResult;
        }
    }
    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.path);
    }
}

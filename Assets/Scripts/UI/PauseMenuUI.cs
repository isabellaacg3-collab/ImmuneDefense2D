using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameStateController gameState;
    [SerializeField] private PausePolicyConfig policy;
    [SerializeField] private GameObject panel;
    private int pauseID = 0;
    private void Awake()
    {
        panel.SetActive(false);
    }
    public void Pause()
    {
        if(pauseID != 0)
        {
            return;
        }
        if(gameState.State == LevelState.Victory || gameState.State == LevelState.Defeat)
        {
            return;
        }
        pauseID = gameState.RequestPause(policy);
        panel.SetActive(true);
    }
    public void Resume()
    {
        if(pauseID == 0)
        {
            return;
        }
        gameState.ReleasePause(pauseID);
        pauseID = 0;
        panel.SetActive(false);
    }
    public void ReturnToLevelSelect()
    {
        if (pauseID != 0 && gameState.State != LevelState.Victory && gameState.State != LevelState.Defeat)
        {
            SceneManager.LoadScene("LevelSelect");
        }
    }

    private void Update()
    {
        if(Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }
        if(pauseID == 0)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }
}

using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] private GameStateController gameState;
    [SerializeField] private TMP_Text resultText;
    private void RefreshResult(LevelState value)
    {
        if (value == LevelState.Defeat)
        {
            resultText.text = "Defeat";
        }else if (value == LevelState.Victory)
        {
            resultText.text = "Victory!";
        }
        else
        {
            resultText.text = "";
        }
    }
    private void OnEnable()
    {
        gameState.StateChanged += RefreshResult;
    }
    private void OnDisable()
    {
        if (gameState != null)
        {
            gameState.StateChanged -= RefreshResult;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

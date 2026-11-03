using UnityEngine;

public class KnowledgeCardController : MonoBehaviour
{
    [SerializeField] private TutorialCardUI cardUI;
    [SerializeField] private GameStateController gameState;
    [SerializeField] private PausePolicyConfig pausePolicy;
    private int pauseId = 0;
    public bool IsOpen
    {
        get
        {
            return cardUI.IsOpen;
        }
    }
    public void Open(TutorialCardConfig config)
    {
        if(config == null)
        {
            return;
        }
        if(pauseId != 0)
        {
            return;
        }
        pauseId = gameState.RequestPause(pausePolicy);
        cardUI.Open(config);
    }
    private void HandleCardClosed()
    {
        if(pauseId == 0)
        {
            return;
        }
        gameState.ReleasePause(pauseId);
        pauseId = 0;
    }
    private void OnEnable()
    {
        cardUI.Closed += HandleCardClosed;
    }
    private void OnDisable()
    {
        if(cardUI != null)
        {
        cardUI.Closed -= HandleCardClosed;
        }
        if(gameState != null)
        {
            HandleCardClosed();
        }
    }
}

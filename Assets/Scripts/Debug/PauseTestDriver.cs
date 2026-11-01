using UnityEngine;

public class PauseTestDriver : MonoBehaviour
{
    [SerializeField] private GameStateController gameState;
    [SerializeField] private PausePolicyConfig policy;
    private int pauseId = 0;
    [ContextMenu("Request Pause")]
    private void RequestTestPause()
    {
        if(pauseId == 1)
        {
            return;
        }
        pauseId = gameState.RequestPause(policy);
    }
    [ContextMenu("Release Pause")]
    private void ReleaseTestPause()
    {
        gameState.ReleasePause(pauseId);
        pauseId = 0;
    }
}

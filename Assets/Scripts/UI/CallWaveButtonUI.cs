using UnityEngine;
using UnityEngine.UI;

public class CallWaveButtonUI : MonoBehaviour
{
    [SerializeField] private WaveController waveController;
    [SerializeField] private Button callWaveButton;
    private void Update()
    {
        callWaveButton.interactable = waveController.CanCallNextWaveEarly;
    }
    public void CallNextWave()
    {
        waveController.TryCallNextWaveEarly();
    }
}

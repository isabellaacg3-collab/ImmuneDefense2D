using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class WaveController : MonoBehaviour
{
    [SerializeField] private GameStateController gameState;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private List<WaveConfig> waves;
    private float elapsedSeconds = 0f;
    private int nextWaveIndex = 0;
    void Start()
    {
        
    }

    void Update()
    {
        if (!gameState.IsPlaying)
        {
            return;
        }
        elapsedSeconds += Time.deltaTime;
        while(nextWaveIndex < waves.Count && elapsedSeconds >= waves[nextWaveIndex].StartAtSeconds)
        {
            spawner.StartWave(waves[nextWaveIndex]);
            nextWaveIndex++;
        }
        if(nextWaveIndex >= waves.Count && !spawner.IsSpawning && spawner.AliveCount == 0)
        {
            gameState.Win();
        }
    }
}

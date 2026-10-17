using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class WaveController : MonoBehaviour
{
    [SerializeField] private GameStateController gameState;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private LevelConfig levelConfig;
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
        while(nextWaveIndex < levelConfig.Waves.Count && elapsedSeconds >= levelConfig.Waves[nextWaveIndex].StartAtSeconds)
        {
            spawner.StartWave(levelConfig.Waves[nextWaveIndex]);
            nextWaveIndex++;
        }
        if(nextWaveIndex >= levelConfig.Waves.Count && !spawner.IsSpawning && spawner.AliveCount == 0)
        {
            gameState.Win();
        }
    }
}

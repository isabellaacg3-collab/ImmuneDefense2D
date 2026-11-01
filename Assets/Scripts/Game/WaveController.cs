using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class WaveController : MonoBehaviour
{
    [SerializeField] private GameStateController gameState;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private LevelFlowController levelFlow;
    private float elapsedSeconds = 0f;
    private int nextWaveIndex = 0;
    private int lastClearedWave = 0;
    private float nextWaveReadySeconds = 0f;
    private void CheckClearedWaves()
    {
        if(spawner.IsSpawning || spawner.AliveCount > 0)
        {
            return;
        }
        while (lastClearedWave < nextWaveIndex)
        {
            lastClearedWave++;
            levelFlow.HandleWaveCleared(lastClearedWave);
        }
    }
    void Update()
    {
        if (!gameState.CanSimulate)
        {
            return;
        }
        elapsedSeconds += Time.deltaTime;
        CheckClearedWaves();
        if (!gameState.CanSimulate)
        {
            return;
        }
        if (nextWaveIndex < levelConfig.Waves.Count && levelFlow.CanStartWave(nextWaveIndex + 1))
        {
            WaveConfig wave = levelConfig.Waves[nextWaveIndex];
            bool previousWaveReady = wave.DelayAfterPreviousWave <= 0f || lastClearedWave == nextWaveIndex;
            if (previousWaveReady)
            {
                nextWaveReadySeconds += Time.deltaTime;
                if (elapsedSeconds >= wave.StartAtSeconds && nextWaveReadySeconds >= wave.DelayAfterPreviousWave)
                {
                    spawner.StartWave(wave);
                    nextWaveIndex++;
                    nextWaveReadySeconds = 0f;
                }
            }
            else
            {
                nextWaveReadySeconds = 0f;
            }
        }
        else
        {
            nextWaveReadySeconds = 0f;
        }
        if (nextWaveIndex >= levelConfig.Waves.Count && !spawner.IsSpawning && spawner.AliveCount == 0 && levelFlow.AreAllWaveEventsCompleted())
        {
            gameState.Win();
        }
    }
}

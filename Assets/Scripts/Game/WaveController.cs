using UnityEngine;
using System;

public class WaveController : MonoBehaviour
{
    [SerializeField] private GameStateController gameState;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private LevelFlowController levelFlow;
    private float elapsedSeconds = 0f;
    private int nextWaveIndex = 0;
    private float lastWaveStartSeconds = 0f;
    private bool startRequested = false;
    private float earlyCallSecondsSaved;
    public event Action<float> WaveCalledEarly;
    public bool CanCallNextWaveEarly
    {
        get
        {
            if (!(gameState.CanSimulate && gameState.CanBuild
                && !startRequested
                && nextWaveIndex > 0 && nextWaveIndex < levelConfig.Waves.Count))
            {
                return false;
            }
            WaveConfig wave = levelConfig.Waves[nextWaveIndex];
            float remaining = GetRemainingWaitSeconds(wave);
            return wave.EarlyCallEnabled && remaining > 0f
                && (wave.EarlyCallWindowSeconds <= 0f || remaining <= wave.EarlyCallWindowSeconds);
        }
    }
    private float GetRemainingWaitSeconds(WaveConfig wave)
    {
        float earliestStart = wave.StartAtSeconds;
        if (nextWaveIndex > 0)
        {
            earliestStart = Mathf.Max(earliestStart, lastWaveStartSeconds + wave.StartIntervalSeconds);
        }
        return Mathf.Max(0f, earliestStart - elapsedSeconds);
    }
    public bool TryCallNextWaveEarly()
    {
        if (!CanCallNextWaveEarly)
        {
            return false;
        }
        startRequested = true;
        earlyCallSecondsSaved = GetRemainingWaitSeconds(levelConfig.Waves[nextWaveIndex]);
        return true;
    }
    private void ProcessWaveStartRequest()
    {
        if (!startRequested)
        {
            return;
        }

        if (!levelFlow.TryPrepareWave(nextWaveIndex + 1))
        {
            return;
        }

        if (!gameState.CanSimulate)
        {
            return;
        }

        float secondsSaved = earlyCallSecondsSaved;
        startRequested = false;
        earlyCallSecondsSaved = 0f;

        StartNextWave();

        if (secondsSaved > 0f)
        {
            WaveCalledEarly?.Invoke(secondsSaved);
        }
    }
    private void StartNextWave()
    {
        WaveConfig wave = levelConfig.Waves[nextWaveIndex];
        lastWaveStartSeconds = elapsedSeconds;
        spawner.StartWave(wave);
        nextWaveIndex++;
    }
    void Update()
    {
        if (!gameState.CanSimulate)
        {
            return;
        }
        elapsedSeconds += Time.deltaTime;

        if (!startRequested && nextWaveIndex < levelConfig.Waves.Count
            && GetRemainingWaitSeconds(levelConfig.Waves[nextWaveIndex]) <= 0f)
        {
            startRequested = true;
            earlyCallSecondsSaved = 0f;
        }

        // One shared preparation attempt per frame, so even overdue waves cannot chain-start.
        ProcessWaveStartRequest();
        if (!gameState.CanSimulate || startRequested)
        {
            return;
        }
        if (nextWaveIndex >= levelConfig.Waves.Count && !spawner.IsSpawning && spawner.AliveCount == 0
            && levelFlow.AreAllWaveStartEventsCompleted())
        {
            gameState.Win();
        }
    }
}

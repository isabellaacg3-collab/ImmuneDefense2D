using System.Collections.Generic;
using UnityEngine;
using System;

public enum LevelState
{
    Playing,
    Victory,
    Defeat,
    Preparing
}
public class GameStateController : MonoBehaviour
{
    [SerializeField] private HostHealth host;
    [SerializeField] private LevelState state = LevelState.Preparing;
    private Dictionary<int, PausePolicyConfig> activePauses = new Dictionary<int, PausePolicyConfig>();
    private int nextPauseId = 1;
    private readonly HashSet<TowerConfig> unlockedTowerUpgrades = new HashSet<TowerConfig>();
    public bool IsPlaying
    {
        get
        {
            return state == LevelState.Playing;
        }
    }
    public LevelState State
    {
        get
        {
            return state;
        }
    }
    public void UnlockTowerUpgrade(TowerConfig targetConfig)
    {
        if(targetConfig != null)
        {
            unlockedTowerUpgrades.Add(targetConfig);
        }
    }
    public bool IsTowerUpgradeUnlocked(TowerConfig targetConfig)
    {
        if(targetConfig != null && unlockedTowerUpgrades.Contains(targetConfig))
        {
            return true;
        }
        return false;
    }
    public bool CanSimulate
    {
        get
        {
            if(state != LevelState.Playing)
            {
                return false;
            }
            foreach (PausePolicyConfig policy in activePauses.Values)
            {
                if (policy.PauseSimulation)
                {
                    return false;
                }
            }
            return true;
        }
    }
    public bool CanBuild
    {
        get
        {
            if (state != LevelState.Preparing && state != LevelState.Playing)
            {
                return false;
            }
            foreach(PausePolicyConfig policy in activePauses.Values)
            {
                if(policy.BlockWorldInput)
                {
                    return false;
                }
            }
            return true;
        }
    }
    public bool CanDeploy
    {
        get
        {
            return state == LevelState.Playing && CanBuild;
        }
    }
    public void BeginBattle()
    {
        if (state != LevelState.Preparing)
        {
            return;
        }
        state = LevelState.Playing;
        StateChanged?.Invoke(state);
    }
    public void Win()
    {
        if (state != LevelState.Playing)
        {
            return;
        }
        state = LevelState.Victory;
        StateChanged?.Invoke(State);
    }
    public event Action<LevelState> StateChanged;
    private void HandleHostDied()
    {
        if (state != LevelState.Playing)
        {
            return;
        }
        state = LevelState.Defeat;
        StateChanged?.Invoke(State);
    }
    private void OnEnable()
    {
        host.Died += HandleHostDied;
    }
    private void OnDisable()
    {
        if (host != null)
        {
            host.Died -= HandleHostDied;
        }
    }
    public int RequestPause(PausePolicyConfig policy)
    {
        if(policy == null)
        {
            return 0;
        }
        int id = nextPauseId;
        nextPauseId++;
        activePauses.Add(id, policy);
        return id;
    }
    public void ReleasePause(int id)
    {
        activePauses.Remove(id);
    }
}

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
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private LevelState state = LevelState.Preparing;
    private Dictionary<int, PausePolicyConfig> activePauses = new Dictionary<int, PausePolicyConfig>();
    private int nextPauseId = 1;
    private MonoBehaviour roadSkillSelection;
    private readonly HashSet<TowerConfig> unlockedTowerUpgrades = new HashSet<TowerConfig>();
    private readonly Dictionary<TowerInstance, List<TowerInstance>> cappedTowers =
        new Dictionary<TowerInstance, List<TowerInstance>>();
    public LevelConfig LevelConfig => levelConfig;
    public bool TowerSaleUnlocked { get; private set; }

    public void UnlockTowerSale()
    {
        TowerSaleUnlocked = true;
    }

    public void RegisterBuiltTower(TowerInstance prefab, TowerInstance tower)
    {
        if (levelConfig == null || levelConfig.GetTowerPlacementLimit(prefab) <= 0) return;

        if (!cappedTowers.TryGetValue(prefab, out List<TowerInstance> towers))
        {
            towers = new List<TowerInstance>();
            cappedTowers.Add(prefab, towers);
        }
        towers.Add(tower);
    }

    public void UnregisterBuiltTower(TowerInstance prefab, TowerInstance tower)
    {
        if (prefab != null && cappedTowers.TryGetValue(prefab, out List<TowerInstance> towers))
        {
            towers.Remove(tower);
        }
    }

    public int CountBuiltTowers(TowerInstance prefab)
    {
        if (!cappedTowers.TryGetValue(prefab, out List<TowerInstance> towers)) return 0;

        towers.RemoveAll(tower => tower == null);
        return towers.Count;
    }

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
    public bool HasRoadSkillSelection => roadSkillSelection != null;

    public bool IsSelectingRoadSkill(MonoBehaviour skill)
    {
        return roadSkillSelection != null && roadSkillSelection == skill;
    }

    public void BeginRoadSkillSelection(MonoBehaviour skill)
    {
        if (CanDeploy && skill != null) roadSkillSelection = skill;
    }

    public void EndRoadSkillSelection(MonoBehaviour skill)
    {
        if (IsSelectingRoadSkill(skill)) roadSkillSelection = null;
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

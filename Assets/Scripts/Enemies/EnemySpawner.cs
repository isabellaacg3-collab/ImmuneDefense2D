using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private HostHealth hostHealth;
    [SerializeField] private GameStateController gameState;
    [SerializeField] private List<ScenePointBinding> RealSpawnPoints = new List<ScenePointBinding>();
    [SerializeField] private List<ScenePointBinding> RealHostPoints = new List<ScenePointBinding>();
    [SerializeField] private EnemyPathOffsetConfig pathOffsetConfig;
    private int spawningGroupCount = 0;
    private List<EnemyHealth> spawnedEnemies = new List<EnemyHealth>();
    public event Action<EnemyConfig> EnemyEnteredCombat;
    public bool IsSpawning
    {
        get
        {
            return spawningGroupCount > 0;
        }
    }
    public int AliveCount
    {
        get
        {
            int count = 0;
            foreach(EnemyHealth enemy in spawnedEnemies)
            {
                if(enemy != null && enemy.IsAlive && enemy.GetComponent<EnemyMovement>().CountsForVictory)
                {
                    count++;
                }
            }
            return count;
        }
    }
    public IReadOnlyList<Transform> Waypoints
    {
        get 
        { 
            return waypoints; 
        }
    }
    public IReadOnlyList<ScenePointBinding> SpawnPoints
    {
        get
        {
            return RealSpawnPoints;
        }
    }
    public IReadOnlyList<ScenePointBinding> HostPoints
    {
        get
        {
            return RealHostPoints;
        }
    }
    private Transform FindPoint(List<ScenePointBinding> bindings, string id)
    {
        foreach (ScenePointBinding binding in bindings)
        {
            if (binding.ID == id)
            {
                return binding.Point;
            }
        }
        return null;
    }
    private void SpawnEnemy(GameObject prefab, Transform spawnPoint, Transform exitPoint)
    {
        GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        spawnedEnemies.Add(health);
        health.Died += HandleEnemyDied;
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        float lateralOffset = UnityEngine.Random.Range(-pathOffsetConfig.MaxLateralOffset,pathOffsetConfig.MaxLateralOffset);
        movement.Initialize(waypoints,hostHealth,transform,exitPoint, gameState,lateralOffset,pathOffsetConfig.TransitionDistance);
        movement.EnteredCombat += HandleEnemyEnteredCombat;
    }
    private void HandleEnemyDied(EnemyHealth enemy)
    {
        EnemyConfig config = enemy.Config;
        EnemyMovement parent = enemy.GetComponent<EnemyMovement>();
        if (!parent.CanBeTargeted || config.SplitChildPrefab == null || config.SplitCount <= 0)
        {
            return;
        }

        float laneLimit = pathOffsetConfig.MaxLateralOffset;
        for (int i = 0; i < config.SplitCount; i++)
        {
            float lane = config.SplitCount == 1 ? 0f : Mathf.Lerp(-laneLimit, laneLimit, (float)i / (config.SplitCount - 1));
            EnemyMovement child = Instantiate(config.SplitChildPrefab, enemy.transform.position, Quaternion.identity);
            child.InitializeFromParent(parent, lane);
            spawnedEnemies.Add(child.GetComponent<EnemyHealth>());
        }
    }
    public void StartWave(WaveConfig wave)
    {
        foreach(SpawnGroupConfig group in wave.Groups)
        {
            spawningGroupCount++;
            StartCoroutine(SpawnGroup(group));
        }
    }
    private IEnumerator SpawnGroup(SpawnGroupConfig group)
    {
        Transform spawnPoint = FindPoint(RealSpawnPoints, group.SpawnPointId);
        Transform exitPoint = FindPoint(RealHostPoints, group.HostPointId);
        if(spawnPoint == null || exitPoint == null)
        {
            Debug.LogError($"找不到入口或出口：{group.SpawnPointId} / {group.HostPointId}", this);
            spawningGroupCount--;
            yield break;
        }
        if (group.StartOffsetSeconds > 0f)
        {
            yield return WaitForSimulationSeconds(group.StartOffsetSeconds);
        }
        for(int tempCount = 0;tempCount < group.EnemyCount;tempCount++)
        {
            while (gameState.IsPlaying && !gameState.CanSimulate)
            {
                yield return null;
            }
            if (!gameState.IsPlaying)
            {
                spawningGroupCount--;
                yield break;
            }
            SpawnEnemy(group.EnemyPrefab,spawnPoint,exitPoint);
            if(tempCount < group.EnemyCount - 1)
            {
                yield return WaitForSimulationSeconds(group.SpawnInterval);
            }
        }
        spawningGroupCount--;
    }
    private IEnumerator WaitForSimulationSeconds(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration || !gameState.CanSimulate)
        {
            if (!gameState.IsPlaying)
            {
                yield break;
            }
            if (gameState.CanSimulate)
            {
                elapsed += Time.deltaTime;
            }
            yield return null;
        }
    }
    private void HandleEnemyEnteredCombat(EnemyConfig config)
    {
        EnemyEnteredCombat?.Invoke(config);
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private readonly List<EnemyHealth> spawnedEnemies = new List<EnemyHealth>();
    private ReadOnlyCollection<EnemyHealth> spawnedEnemiesView;
    public event Action<EnemyConfig> EnemyEnteredCombat;
    public bool IsSpawning
    {
        get
        {
            return spawningGroupCount > 0;
        }
    }
    public IReadOnlyList<EnemyHealth> SpawnedEnemies
    {
        get
        {
            spawnedEnemies.RemoveAll(enemy => enemy == null || !enemy.IsAlive);
            return spawnedEnemiesView ??= spawnedEnemies.AsReadOnly();
        }
    }
    public int AliveCount
    {
        get
        {
            int count = 0;
            foreach (EnemyHealth enemy in SpawnedEnemies)
            {
                if (enemy.GetComponent<EnemyMovement>().CountsForVictory)
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
    private void SpawnEnemy(GameObject prefab, Transform spawnPoint, Transform exitPoint, float gatherTravelSeconds)
    {
        GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        float lateralOffset = UnityEngine.Random.Range(-pathOffsetConfig.MaxLateralOffset,pathOffsetConfig.MaxLateralOffset);
        movement.Initialize(waypoints, hostHealth, this, exitPoint, gameState, lateralOffset,
            pathOffsetConfig.TransitionDistance, gatherTravelSeconds);
        movement.EnteredCombat += HandleEnemyEnteredCombat;
        spawnedEnemies.Add(enemy.GetComponent<EnemyHealth>());
    }
    public void SpawnChildren(EnemyMovement prefab, EnemyMovement parent, int count)
    {
        if (prefab == null || parent == null || parent.Spawner != this || count <= 0)
        {
            return;
        }

        float laneLimit = pathOffsetConfig.MaxLateralOffset;
        for (int i = 0; i < count; i++)
        {
            float lane = count == 1 ? 0f : Mathf.Lerp(-laneLimit, laneLimit, (float)i / (count - 1));
            EnemyMovement child = Instantiate(prefab, parent.transform.position, Quaternion.identity);
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
            SpawnEnemy(group.EnemyPrefab, spawnPoint, exitPoint, group.GatherTravelSeconds);
            if (tempCount < group.EnemyCount - 1 && group.SpawnInterval > 0f)
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

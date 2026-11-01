using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private HostHealth hostHealth;
    [SerializeField] private GameStateController gameState;
    [SerializeField] private List<ScenePointBinding> RealSpawnPoints = new List<ScenePointBinding>();
    [SerializeField] private List<ScenePointBinding> RealHostPoints = new List<ScenePointBinding>();
    private int spawningGroupCount = 0;
    private List<EnemyHealth> spawnedEnemies = new List<EnemyHealth>();
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
                if(enemy != null && enemy.GetComponent<EnemyMovement>().CountsForVictory)
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
        spawnedEnemies.Add(enemy.GetComponent<EnemyHealth>());
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();

        movement.Initialize(waypoints,hostHealth,transform,exitPoint, gameState);
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
}

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private HostHealth hostHealth;
    [SerializeField] private GameStateController gameState;
    private int spawningWaveCount = 0;
    private List<EnemyHealth> spawnedEnemies = new List<EnemyHealth>();
    public bool IsSpawning
    {
        get
        {
            return spawningWaveCount > 0;
        }
    }
    public int AliveCount
    {
        get
        {
            int count = 0;
            foreach(EnemyHealth enemy in spawnedEnemies)
            {
                if(enemy != null)
                {
                    count++;
                }
            }
            return count;
        }
    }
    private void SpawnEnemy(GameObject prefab)
    {
        GameObject enemy = Instantiate(prefab, transform.position, Quaternion.identity);
        spawnedEnemies.Add(enemy.GetComponent<EnemyHealth>());
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();

        movement.Initialize(waypoints, hostHealth,gameState);
    }
    public void StartWave(WaveConfig wave)
    {
        spawningWaveCount++;
        StartCoroutine(SpawnWave(wave));
    }
    private IEnumerator SpawnWave(WaveConfig wave)
    {
        for(int tempCount = 0;tempCount < wave.EnemyCount;tempCount++)
        {
            if (!gameState.IsPlaying)
            {
                spawningWaveCount--;
                yield break;
            }
            SpawnEnemy(wave.EnemyPrefab);
            if(tempCount < wave.EnemyCount - 1)
            {
                yield return new WaitForSeconds(wave.SpawnInterval);
            }
        }
        spawningWaveCount--;
    }
}

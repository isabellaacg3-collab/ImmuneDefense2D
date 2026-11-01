using UnityEngine;

[System.Serializable]
public class SpawnGroupConfig
{
    [SerializeField] private float startOffsetSeconds = 0f;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 5;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private string spawnPointId = "";
    [SerializeField] private string hostPointId = "";
    public float StartOffsetSeconds
    {
        get
        {
            return startOffsetSeconds;
        }
    }
    public GameObject EnemyPrefab
    {
        get
        {
            return enemyPrefab;
        }
    }
    public int EnemyCount
    {
        get
        {
            return enemyCount;
        }
    }
    public float SpawnInterval
    {
        get
        {
            return spawnInterval;
        }
    }
    public string SpawnPointId
    {
        get
        {
            return spawnPointId;
        }
    }
    public string HostPointId
    {
        get
        {
            return hostPointId;
        }
    }
} 

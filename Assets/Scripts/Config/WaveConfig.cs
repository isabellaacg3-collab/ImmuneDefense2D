using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] float startAtSeconds = 5f;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int enemyCount = 5;
    [SerializeField] float spawnInterval = 2f;
    public float StartAtSeconds
    {
        get
        {
            return startAtSeconds;
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
}

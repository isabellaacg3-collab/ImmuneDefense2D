using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class EnemyDensityDefenseController : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameStateController gameState;
    private float refreshTimer = 0f;
    [SerializeField, Min(0.01f)] private float refreshInterval = 0.2f;
    [SerializeField, Min(0f)] private float armorHoldSeconds = 2f;
    private float simulationTime;
    public EnemySpawner EnemySpawner => enemySpawner;
    public GameStateController GameState => gameState;
    private int CountNearbySameType(EnemyHealth target, float radius, IReadOnlyList<EnemyHealth> enemies)
    {
        int count = 0;
        foreach(EnemyHealth candidate in enemies)
        {
            if(candidate == null || !candidate.IsAlive || candidate.Config != target.Config || !candidate.GetComponent<EnemyMovement>().CanBeTargeted) 
            {
                continue;
            }
            if(Vector3.Distance(target.transform.position, candidate.transform.position) <= radius)
            {
                count++;
            }
            
        }
        return count;
    }
    private void  RefreshDensityDefense()
    {
        var enemies = enemySpawner.SpawnedEnemies;
        foreach(EnemyHealth enemy in enemies)
        {
            if (!enemy.TryGetComponent(out DensityDefense density))
            {
                continue;
            }

            if (!enemy.GetComponent<EnemyMovement>().CanBeTargeted)
            {
                density.IsActive = false;
                density.ArmorExpiresAt = 0f;
                continue;
            }

            bool hasEnoughNeighbors = CountNearbySameType(enemy, density.Radius, enemies) >= density.MinimumEnemyCount;

            if (hasEnoughNeighbors)
            {
                density.ArmorExpiresAt = simulationTime + armorHoldSeconds;
            }

            density.IsActive = hasEnoughNeighbors || simulationTime < density.ArmorExpiresAt;
        }
    }
    private void Update()
    {
        if(enemySpawner == null || GameState == null || !gameState.CanSimulate)
        {
            return;
        }
        simulationTime += Time.deltaTime;
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshInterval)
        {
            refreshTimer = 0f;
            RefreshDensityDefense();
        }
    }
}

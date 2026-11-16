using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RedBloodCellTransportUnit : MonoBehaviour
{
    private const float CatchDistance = 0.25f;
    private static readonly List<RedBloodCellTransportUnit> ActiveUnits = new List<RedBloodCellTransportUnit>();

    [SerializeField, Min(1)] private int maxHealth = 3;
    [SerializeField, Min(0.01f)] private float hitInterval = 1f;
    [SerializeField] private EnemyConfig basicEnemyConfig;

    private Vector3[] destinations;
    private int nextDestination;
    private GameStateController gameState;
    private ImmunePoints immunePoints;
    private float moveSpeed;
    private int deliveryReward;
    private float pathDistance;
    private bool delivered;
    private int health;
    private EnemyMovement blockedEnemy;
    private EnemyHealth attacker;
    private float hitTimer;

    public void Initialize(Vector3[] route, float entryDistance, GameStateController state, ImmunePoints points, float speed, int reward)
    {
        destinations = route;
        pathDistance = entryDistance;
        gameState = state;
        immunePoints = points;
        moveSpeed = speed;
        deliveryReward = reward;
        health = maxHealth;
    }

    // Check forward travel along the shared route so nearby bends never count as an interception.
    public static bool TryIntercept(EnemyMovement enemy, GameStateController state, float distance, float step, out float allowedStep)
    {
        allowedStep = step;
        RedBloodCellTransportUnit target = null;
        float nearest = step + CatchDistance;
        foreach (var unit in ActiveUnits)
        {
            float gap = unit.pathDistance - distance;
            if (unit.gameState != state || unit.nextDestination == 0 || unit.delivered || unit.health <= 0
                || unit.blockedEnemy != null || gap < 0f || gap > nearest)
            {
                continue;
            }
            target = unit;
            nearest = gap;
        }

        if (target == null || !enemy.TryGetComponent(out EnemyHealth enemyHealth) || !enemyHealth.IsAlive || !enemy.TryBlock(target))
        {
            return false;
        }

        target.blockedEnemy = enemy;
        target.attacker = enemyHealth;
        target.hitTimer = 0f;
        allowedStep = Mathf.Max(0f, nearest - CatchDistance);
        return true;
    }

    private void OnEnable()
    {
        ActiveUnits.Add(this);
    }

    private void OnDisable()
    {
        ActiveUnits.Remove(this);
        ReleaseBlock();
    }

    private void ReleaseBlock()
    {
        if (blockedEnemy != null)
        {
            blockedEnemy.ReleaseBlock(this);
        }
        blockedEnemy = null;
        attacker = null;
    }

    private bool UpdateInterception()
    {
        if (blockedEnemy == null || !blockedEnemy.CanBeTargeted || attacker == null || !attacker.IsAlive)
        {
            ReleaseBlock();
            return false;
        }

        hitTimer += Time.deltaTime;
        while (hitTimer >= hitInterval)
        {
            hitTimer -= hitInterval;
            if (--health > 0) continue;

            if (basicEnemyConfig != null) attacker.Heal(basicEnemyConfig.MaxHealth / 10);
            ReleaseBlock();
            destinations = null;
            Destroy(gameObject);
            break;
        }
        return true;
    }

    private void Update()
    {
        if (destinations == null || gameState == null || !gameState.CanSimulate || UpdateInterception())
        {
            return;
        }

        float remaining = moveSpeed * Time.deltaTime;
        while (nextDestination < destinations.Length)
        {
            Vector3 target = destinations[nextDestination];
            float distance = Vector3.Distance(transform.position, target);
            float travel = Mathf.Min(remaining, distance);
            if (nextDestination > 0 && !delivered) pathDistance += travel;

            if (distance > remaining)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, remaining);
                return;
            }

            transform.position = target;
            remaining -= distance;
            nextDestination++;

            // Spread is the last waypoint before Host. The remaining leg is outbound only.
            if (nextDestination == destinations.Length - 1)
            {
                delivered = true;
                ReleaseBlock();
                if (immunePoints != null) immunePoints.Grant(deliveryReward);
                if (!gameState.CanSimulate) return;
            }
        }

        destinations = null;
        Destroy(gameObject);
    }
}

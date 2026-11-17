using UnityEngine;

public class ReinforcementUnit : MonoBehaviour
{
    [SerializeField] private ReinforcementConfig config;
    [SerializeField] private GameStateController gameState;
    [SerializeField] private EnemyHealth target;
    private float elapsedSeconds = 0;
    private EnemyMovement blockedEnemy;
    private float attackTimer = 0f;
    private Vector3 deploymentPosition;
    public void Initialize(GameStateController controller)
    {
        gameState = controller;
        deploymentPosition = transform.position;
    }
    private void ReleaseBlockedEnemy()
    {
        if(blockedEnemy != null)
        {
            blockedEnemy.ReleaseBlock(this);
        }
        blockedEnemy = null;
    }
    private void FindTarget()
    {
        ReleaseBlockedEnemy();
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        target = null;
        float nearestDistance = config.DetectionRange;
        foreach (EnemyHealth enemy in enemies)
        {
            if (!enemy.GetComponent<EnemyMovement>().CanBeTargeted)
            {
                continue;
            }
            float tempDis = Vector3.Distance(deploymentPosition, enemy.transform.position);
            if (tempDis <= nearestDistance)
            {
                target = enemy;
                nearestDistance = tempDis;
            }
        }
    }
    private EnemyHealth FindUnblockedTarget()
    {
        EnemyHealth candidate = null;
        float nearestDistance = config.DetectionRange;
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        foreach (EnemyHealth enemy in enemies)
        {
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if(enemy == target || !movement.CanBeTargeted || movement.BlockerCount > 0)
            {
                continue;
            }
            float tempDis = Vector3.Distance(deploymentPosition, enemy.transform.position);
            if (tempDis <= nearestDistance)
            {
                candidate = enemy;
                nearestDistance = tempDis;
            }
        }
        return candidate;
    }
    private void Update()
    {
        if (!gameState.CanSimulate)
        {
            return;
        }
        elapsedSeconds += Time.deltaTime;
        attackTimer += Time.deltaTime;
        if(elapsedSeconds >= config.Duration)
        {
            Destroy(gameObject);
            return;
        }
        if (target == null || !target.GetComponent<EnemyMovement>().CanBeTargeted || 
            Vector3.Distance(deploymentPosition, target.transform.position) > config.DetectionRange)
        {
            FindTarget();
            if (target == null)
            {
                transform.position = Vector3.MoveTowards(transform.position, deploymentPosition, config.MoveSpeed * Time.deltaTime);
                return;
            }
        }
        if(blockedEnemy != null && blockedEnemy.BlockerCount > 1)
        {
            EnemyHealth candidate = FindUnblockedTarget();
            if(candidate != null)
            {
                ReleaseBlockedEnemy();
                target = candidate;
            }
        }
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if(distance >= config.AttackRange)
        {
            ReleaseBlockedEnemy();
            transform.position = Vector3.MoveTowards(transform.position,target.transform.position, 
                Mathf.Min(config.MoveSpeed * Time.deltaTime,distance - config.AttackRange));
        }
        distance = Vector3.Distance(transform.position, target.transform.position);
        if(distance <= config.AttackRange + 0.001f)
        {
            EnemyMovement movement = target.GetComponent<EnemyMovement>();
            if (movement.TryBlock(this))
            {
                blockedEnemy = movement;
                if(attackTimer >= config.AttackInterval)
                {
                    target.TakeDamage(config.AttackDamage,EnemyDamageType.Swallow);
                    attackTimer = 0;
                }
            }
        }
    }
    private void OnDisable()
    {
        ReleaseBlockedEnemy();
    }
}

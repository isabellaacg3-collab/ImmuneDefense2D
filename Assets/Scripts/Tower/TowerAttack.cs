using System;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [SerializeField] TowerConfig config;
    [SerializeField] private EnemyHealth target;
    [SerializeField] private float attackInterval;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private GameStateController gameState;
    float attackTimer = 0f;
    public EnemyHealth CurrentTarget => target;
    public GameStateController GameState => gameState;
    public bool HasValidTarget => IsValidTarget(target);
    public event Action<EnemyHealth> TargetChanged;
    public event Action<EnemyVisualSnapshot> TargetKilled;
    public TowerConfig Config
    {
        get
        {
            return config;
        }
    }
    public void Initialize(GameStateController controller)
    {
        gameState = controller;
    }
    [ContextMenu("Test TowerDamage")]
    private void Attack()
    {
        if (gameState == null || !gameState.CanSimulate)
        {
            return;
        }
        if (!IsValidTarget(target))
        {
            return;
        }
        bool killed = target.TryTakeDamage(attackDamage, out EnemyVisualSnapshot deathVisual);
        attackTimer = 0;
        if (killed)
        {
            TargetKilled?.Invoke(deathVisual);
            SetTarget(null);
        }
    }
    private bool IsValidTarget(EnemyHealth enemy)
    {
        if (enemy == null || !enemy.IsAlive || !enemy.isActiveAndEnabled)
        {
            return false;
        }
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        return movement != null && movement.CanBeTargeted
            && Vector3.Distance(transform.position, enemy.transform.position) <= attackRange;
    }
    private void SetTarget(EnemyHealth enemy)
    {
        if (ReferenceEquals(target, enemy))
        {
            return;
        }
        target = enemy;
        TargetChanged?.Invoke(target);
    }
    private void FindTarget()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        EnemyHealth candidate = null;
        float nearestDistance = attackRange;
        foreach(EnemyHealth enemy in enemies)
        {
            if (!IsValidTarget(enemy))
            {
                continue; 
            }
            float tempDis = Vector3.Distance(transform.position, enemy.transform.position);
            if(tempDis <= nearestDistance)
            {
                candidate = enemy;
                nearestDistance = tempDis;
            }
        }
        SetTarget(candidate);
    }
    private void Awake()
    {
        attackDamage = config.AttackDamage;
        attackInterval = config.AttackInterval;
        attackRange = config.AttackRange;
    }
    void Start()
    {
        attackTimer = attackInterval;
    }
    void Update()
    {
        if (gameState == null || !gameState.CanSimulate)
        {
            return;
        }
        attackTimer += Time.deltaTime;
        if (!IsValidTarget(target))
        {
            FindTarget();
            if(target == null)
            {
                return;
            }
        }
        if(attackTimer >= attackInterval)
        { 
            Attack();
         }
    }
    private void OnDisable()
    {
        SetTarget(null);
    }
}

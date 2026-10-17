using JetBrains.Annotations;
using System;
using Unity.VisualScripting;
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
        if (!gameState.IsPlaying)
        {
            return;
        }
        if (target == null)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if(distance > attackRange)
        {
            return;
        }
        target.TakeDamage(attackDamage);
        attackTimer = 0;
    }

    private void FindTarget()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        target = null;
        float nearestDistance = attackRange;
        foreach(EnemyHealth enemy in enemies)
        {
            float tempDis = Vector3.Distance(transform.position, enemy.transform.position);
            if(tempDis <= nearestDistance)
            {
                target = enemy;
                nearestDistance = tempDis;
            }
        }
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
        if (!gameState.IsPlaying)
        {
            return;
        }
        attackTimer += Time.deltaTime;
        if (target == null || Vector3.Distance(transform.position,target.transform.position) > attackRange)
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
}

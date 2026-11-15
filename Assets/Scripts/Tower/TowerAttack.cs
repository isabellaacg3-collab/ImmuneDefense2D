using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [SerializeField] TowerConfig config;
    [SerializeField] private EnemyHealth target;
    [SerializeField] private float attackInterval;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private GameStateController gameState;
    private float attackTimer = 0f;
    private class TargetLink
    {
        public EnemyHealth Target;
        public float SlowTimer;
        public TargetLink(EnemyHealth target)
        {
            Target = target;
            SlowTimer = 0f;
        }
    }
    private List<TargetLink> links = new List<TargetLink>();
    public EnemyHealth CurrentTarget => target;
    public int LinkedTargetCount => links.Count;
    public GameStateController GameState => gameState;
    public bool HasValidTarget => IsValidTarget(target);
    public event Action<EnemyHealth> TargetChanged;
    public event Action<EnemyHealth, EnemyVisualSnapshot> TargetKilled;
    public TowerConfig Config
    {
        get
        {
            return config;
        }
    }
    public EnemyHealth GetLinkedTarget(int index)
    {
        if(index < 0 || index >= links.Count)
        {
            return null;
        }
        return links[index].Target;
    }
    public void Initialize(GameStateController controller)
    {
        gameState = controller;
    }
    private void Attack()
    {
        if (gameState == null || !gameState.CanSimulate)
        {
            return;
        }
        for (int i = links.Count - 1; i >= 0; i--)
        {
            EnemyHealth enemy = links[i].Target;
            if (!IsValidTarget(enemy))
            {
                continue;
            }
            bool killed = enemy.TryTakeDamage(attackDamage, out EnemyVisualSnapshot deathVisual);
            if (killed)
            {
                TargetKilled?.Invoke(enemy, deathVisual);
                RemoveLinkAt(i);
            }
        }
        attackTimer = 0f;
        SetTarget(links.Count > 0 ? links[0].Target : null);
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
    private bool HasLinkTo(EnemyHealth enemy)
    {
        foreach (TargetLink link in links)
        {
            if(link.Target == enemy)
            {
                return true;
            }
        }
        return false;
    }
    private EnemyHealth FindNearestUnlinkedTarget()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        EnemyHealth candidate = null;
        float nearestDistance = attackRange;
        foreach (EnemyHealth enemy in enemies)
        {
            if (!IsValidTarget(enemy) || HasLinkTo(enemy))
            {
                continue;
            }
            float tempDis = Vector3.Distance(transform.position, enemy.transform.position);
            if (tempDis <= nearestDistance)
            {
                candidate = enemy;
                nearestDistance = tempDis;
            }
        }
        return candidate;
    }
    private void FillEmptyLinks()
    {
        while (links.Count < config.MaxLinkedTargets)
        {
            EnemyHealth enemy = FindNearestUnlinkedTarget();
            if(enemy == null)
            {
                break;
            }
            links.Add(new TargetLink(enemy));
        }
    }
    private void UpdateSlow(TargetLink link)
    {
        EnemyHealth enemy = link.Target;
        if (!IsValidTarget(enemy))
        {
            return;
        }
        link.SlowTimer += Time.deltaTime;

        float progress = 0f;
        if(config.SlowRampSeconds <= 0f)
        {
            progress = 1f;
        }
        else
        {
            progress = Mathf.Clamp01(link.SlowTimer / config.SlowRampSeconds);
        }
        float multiplier = 1f - Mathf.Clamp01(config.MaximumSlowPercent) * progress;

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if(movement != null)
        {
            movement.SetSlowMultiplier(this, multiplier);
        }
    }
    private void RemoveLinkAt(int index)
    {
        EnemyHealth enemy = links[index].Target;
        if (enemy != null)
        {
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.RemoveSlowMultiplier(this);
            }
        }
        links.RemoveAt(index);
    }
    private void RemoveInvalidLinks()
    {
        for (int i = links.Count - 1; i >= 0; i--)
        {
            if (!IsValidTarget(links[i].Target))
            {
                RemoveLinkAt(i);
            }
        }
    }
    private void ClearAllLinks()
    {
        while (links.Count > 0)
        {
            RemoveLinkAt(links.Count - 1);
        }
    }
    public bool TryApplyUpgrade()
    {
        TowerConfig nextConfig = config.UpgradeTarget;
        if(nextConfig == null || nextConfig == config)
        {
            return false;
        }
        ClearAllLinks();
        SetTarget(null);
        ApplyConfig(nextConfig);
        return true;
    }
    private void ApplyConfig(TowerConfig newConfig)
    {
        config = newConfig;

        attackDamage = config.AttackDamage;
        attackInterval = config.AttackInterval;
        attackRange = config.AttackRange;

    }
    private void Awake()
    {
        ApplyConfig(config);
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
        RemoveInvalidLinks();
        FillEmptyLinks();
        SetTarget(links.Count > 0 ? links[0].Target : null);
        if (target == null)
        {
            return;
        }
        foreach (TargetLink link in links)
        {
            UpdateSlow(link);
        }
        if (attackTimer >= attackInterval)
        { 
            Attack();
         }
    }
    private void OnDisable()
    {
        ClearAllLinks();
        SetTarget(null);
    }
}

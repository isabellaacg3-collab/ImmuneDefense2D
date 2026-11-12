using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Tower Config")]
public class TowerConfig : ScriptableObject
{
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private int buildCost = 100;
    [SerializeField] private TutorialCardConfig knowledgeCard;
    [SerializeField] private float slowRampSeconds = 1.5f;
    [SerializeField] private float maximumSlowPercent = 0.2f;
    [SerializeField] TowerConfig upgradeTarget;
    [SerializeField] private int upgradeCost;
    public int AttackDamage
    {
        get
        {
            return attackDamage;
        }
    }
    public float AttackInterval
    {
        get
        {
            return attackInterval;
        }
    }
    public float AttackRange
    {
        get
        {
            return attackRange;
        }
    }
    public int BuildCost
    {
        get
        {
            return buildCost;
        }
    }
    public TutorialCardConfig KnowledgeCard
    {
        get
        {
            return knowledgeCard;
        }
    }
    public float SlowRampSeconds
    {
        get
        {
            return slowRampSeconds;
        }
    }
    public float MaximumSlowPercent
    {
        get
        {
            return maximumSlowPercent;
        }
    }
    public TowerConfig UpgradeTarget
    {
        get
        {
            return upgradeTarget;
        }
    }
    public int UpgradeCost
    {
        get
        {
            return upgradeCost;
        }
    }
}

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
}

using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Tower Config")]
public class TowerConfig : ScriptableObject
{
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackInterval = 1f;
    [SerializeField] float attackRange = 5f;
    [SerializeField] int buildCost = 100;
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
}

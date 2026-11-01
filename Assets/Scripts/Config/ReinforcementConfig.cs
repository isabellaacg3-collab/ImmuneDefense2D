using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Reinforcement Config")]
public class ReinforcementConfig : ScriptableObject
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float detectionRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackInterval;
    [SerializeField] private int attackDamage;
    [SerializeField] private int health;
    [SerializeField] private float duration;
    //armor
    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
    }
    public float DetectionRange
    {
        get
        {
            return detectionRange;
        }
    }
    public float AttackRange
    {
        get
        {
            return attackRange;
        }
    }
    public float AttackInterval
    {
        get
        {
            return attackInterval;
        }
    }
    public int AttackDamage
    {
        get
        {
            return attackDamage;
        }
    }
    public int Health
    {
        get
        {
            return health;
        }
    }
    public float Duration
    {
        get
        {
            return duration;
        }
    }
}

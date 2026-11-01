using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [SerializeField] int maxHealth;
    [SerializeField] float moveSpeed;
    [SerializeField] int hostDamage;

    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }
    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
    }
    public int HostDamage
    {
        get
        {
            return hostDamage;
        }
    }
}

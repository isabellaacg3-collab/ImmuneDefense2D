using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [SerializeField] int maxHealth = 3;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] int hostDamage = 1;

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

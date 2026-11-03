using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [SerializeField] private int maxHealth;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int hostDamage;
    [SerializeField] private TutorialCardConfig firstAppearanceCard;
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
    public TutorialCardConfig FirstAppearanceCard
    {
        get
        {
            return firstAppearanceCard;
        }
    }
}

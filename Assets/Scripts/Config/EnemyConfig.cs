using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [SerializeField] private int maxHealth;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int hostDamage;
    [SerializeField, Range(0f, 0.95f)] private float swallowArmor;
    [SerializeField, Range(0f, 0.95f)] private float antibodyArmor;
    [SerializeField] private TutorialCardConfig firstAppearanceCard;

    public float SwallowArmor => swallowArmor;
    public float AntibodyArmor => antibodyArmor;

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

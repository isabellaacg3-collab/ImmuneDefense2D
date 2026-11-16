using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [SerializeField] private int maxHealth;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int hostDamage;
    [SerializeField] private TutorialCardConfig firstAppearanceCard;
    [SerializeField] private EnemyMovement splitChildPrefab;
    [SerializeField, Min(0)] private int splitCount;

    public EnemyMovement SplitChildPrefab => splitChildPrefab;
    public int SplitCount => splitCount;

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

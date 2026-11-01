using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Reinforcement Deployment Config")]
public class ReinforcementDeploymentConfig : ScriptableObject
{
    [SerializeField] private ReinforcementUnit unitPrefab;
    [SerializeField] private int unitCount = 2;
    [SerializeField] private float cooldownSeconds = 20;
    [SerializeField] private float maxDisFromPath = 0.6f;
    [SerializeField] private float spawnForwardOffset = 0.2f;
    [SerializeField] private float spawnSideOffset = 0.15f;
    [SerializeField] private SpriteRenderer previewPrefab;
    public ReinforcementUnit UnitPrefab
    {
        get
        {
            return unitPrefab;
        }
    }
    public int UnitCount
    {
        get
        {
            return unitCount;
        }
    }
    public float CooldownSeconds
    {
        get
        {
            return cooldownSeconds;
        }
    }
    public float MaxDisFromPath
    {
        get
        {
            return maxDisFromPath;
        }
    }
    public float SpawnForwardOffset
    {
        get
        {
            return spawnForwardOffset;
        }
    }
    public float SpawnSideOffset
    {
        get
        {
            return spawnSideOffset;
        }
    }
    public SpriteRenderer PreviewPrefab
    {
        get
        {
            return previewPrefab;
        }
    }
}

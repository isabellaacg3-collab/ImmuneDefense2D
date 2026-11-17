using UnityEngine;

[DisallowMultipleComponent]
public sealed class DensityDefense : MonoBehaviour
{
    [SerializeField, Min(0f)] private float radius = 1f;
    [SerializeField, Min(1), Tooltip("Includes this enemy.")] private int minimumEnemyCount = 3;
    [SerializeField, Range(0f, 0.95f)] private float antibodyArmorBonus = 0.2f;

    public float Radius => radius;
    public int MinimumEnemyCount => minimumEnemyCount;
    public bool IsActive { get; set; }
    public float AntibodyArmorBonus => isActiveAndEnabled && IsActive ? antibodyArmorBonus : 0f;
    public float ArmorExpiresAt { get; set; }
}

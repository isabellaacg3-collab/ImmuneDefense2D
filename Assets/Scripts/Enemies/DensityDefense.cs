using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class DensityDefense : MonoBehaviour
{
    [SerializeField, Min(0f)] private float radius = 1f;
    [SerializeField, Min(1), Tooltip("Includes this enemy.")] private int minimumEnemyCount = 3;
    [SerializeField, Range(0f, 0.95f)] private float antibodyArmorBonus = 0.2f;

    public float Radius => radius;
    public int MinimumEnemyCount => minimumEnemyCount;
    private bool isActive;
    public event Action Activated;
    public bool IsActive
    {
        get => isActive;
        set
        {
            if (isActive == value) return;

            isActive = value;
            if (isActive) Activated?.Invoke();
        }
    }
    public float AntibodyArmorBonus => isActiveAndEnabled && IsActive ? antibodyArmorBonus : 0f;
    public float ArmorExpiresAt { get; set; }
}

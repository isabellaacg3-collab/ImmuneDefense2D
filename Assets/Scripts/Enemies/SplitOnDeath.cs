using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyHealth), typeof(EnemyMovement))]
public sealed class SplitOnDeath : MonoBehaviour
{
    [SerializeField] private EnemyMovement childPrefab;
    [SerializeField, Min(1)] private int childCount = 2;

    private EnemyHealth health;
    private EnemyMovement movement;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        movement = GetComponent<EnemyMovement>();
    }

    private void OnEnable()
    {
        health.Died += HandleDied;
    }

    private void OnDisable()
    {
        health.Died -= HandleDied;
    }

    private void HandleDied(EnemyHealth sender)
    {
        if (movement.CanBeTargeted && movement.Spawner != null)
        {
            movement.Spawner.SpawnChildren(childPrefab, movement, childCount);
        }
    }
}

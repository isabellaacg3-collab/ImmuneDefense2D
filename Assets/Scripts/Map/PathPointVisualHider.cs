using UnityEngine;

[DisallowMultipleComponent]
public class PathPointVisualHider : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private SpriteRenderer background;

    private void Awake()
    {
        if (spawner == null)
        {
            Debug.LogWarning("PathPointVisualHider requires an EnemySpawner reference.", this);
            return;
        }

        HidePoint(spawner.transform);
        foreach (Transform waypoint in spawner.Waypoints)
        {
            HidePoint(waypoint);
        }
        foreach (ScenePointBinding binding in spawner.SpawnPoints)
        {
            HidePoint(binding?.Point);
        }
        foreach (ScenePointBinding binding in spawner.HostPoints)
        {
            HidePoint(binding?.Point);
        }
    }

    private void HidePoint(Transform point)
    {
        if (point == null)
        {
            return;
        }

        SpriteRenderer marker = point.GetComponent<SpriteRenderer>();
        if (marker == null)
        {
            return;
        }

        if (background != null)
        {
            marker.sortingLayerID = background.sortingLayerID;
            marker.sortingOrder = background.sortingOrder - 1;
        }
        marker.enabled = false;
    }
}

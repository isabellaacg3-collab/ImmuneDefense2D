using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RedBloodCellTransportRoute : MonoBehaviour
{
    [SerializeField] private EnemySpawner pathSource;
    [SerializeField] private ImmunePoints immunePoints;
    [SerializeField] private Transform hostExit;

    public ImmunePoints ImmunePoints => immunePoints;

    public bool TryCreateRoute(Vector3 origin, out Vector3[] destinations, out float entryDistance)
    {
        destinations = null;
        entryDistance = 0f;
        if (pathSource == null || pathSource.Waypoints == null || hostExit == null)
        {
            return false;
        }

        var path = new List<Vector3>(pathSource.Waypoints.Count + 1) { pathSource.transform.position };
        foreach (Transform waypoint in pathSource.Waypoints)
        {
            if (waypoint == null)
            {
                return false;
            }
            path.Add(waypoint.position);
        }

        if (!PathGeometryUtility.TryProjectPointToPath(origin, path, out PathProjectionResult projection))
        {
            return false;
        }

        // Start at the projection, then follow the segment's end and every remaining waypoint.
        destinations = new Vector3[path.Count - projection.SegmentIndex + 1];
        destinations[0] = projection.ProjectedPosition;
        for (int i = 1; i < destinations.Length - 1; i++)
        {
            destinations[i] = path[projection.SegmentIndex + i];
        }
        destinations[destinations.Length - 1] = hostExit.position;

        for (int i = 0; i < projection.SegmentIndex; i++)
        {
            entryDistance += Vector3.Distance(path[i], path[i + 1]);
        }
        entryDistance += Vector3.Distance(path[projection.SegmentIndex], projection.ProjectedPosition);
        return true;
    }
}

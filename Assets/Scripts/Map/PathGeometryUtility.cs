using UnityEngine;
using System.Collections.Generic;

public static class PathGeometryUtility
{
    public static Vector3 ClosestPointOnSegment(Vector3 point, Vector3 start, Vector3 end)
    {
        Vector3 segment = end - start;
        float lengthSquared = segment.sqrMagnitude;
        if(lengthSquared == 0f)
        {
            return start;
        }
        float t = Vector3.Dot(point - start, segment) / lengthSquared;
        t = Mathf.Clamp01(t);
        return start + segment * t;
    }
    public static bool TryProjectPointToPath(Vector3 point,IReadOnlyList<Vector3> path,out PathProjectionResult result)
    {
        result = default;
        result.Distance = float.PositiveInfinity;
        bool found = false;
        if(path == null || path.Count < 2)
        {
            return false;
        }
        for(int i = 0;i < path.Count - 1; i++)
        {
            Vector3 start = path[i];
            Vector3 end = path[i + 1];
            Vector3 segment = end - start;
            if(segment.sqrMagnitude == 0f)
            {
                continue;
            }
            Vector3 closest = ClosestPointOnSegment(point, start, end);
            float distance = Vector3.Distance(point, closest);
            if(distance < result.Distance)
            {
                result.Distance = distance;
                result.ProjectedPosition = closest;
                result.Forward = segment.normalized;
                result.SegmentIndex = i;
                found = true;
            }
        }
        return found;
    }
}

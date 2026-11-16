using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public static class RoadSkillTargeting
{
    private static readonly List<RaycastResult> UiHits = new List<RaycastResult>();
    private static readonly Plane MapPlane = new Plane(Vector3.forward, Vector3.zero);

    public static bool TryGetPointerPosition(Camera worldCamera, out Vector3 position)
    {
        position = default;
        return Mouse.current != null && TryGetWorldPosition(worldCamera, Mouse.current.position.ReadValue(), out position);
    }

    public static bool TryGetWorldPosition(Camera worldCamera, Vector2 screen, out Vector3 position)
    {
        position = default;
        if (worldCamera == null) return false;

        if (EventSystem.current != null)
        {
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = screen }, UiHits);
            bool overUi = UiHits.Exists(hit => hit.module is GraphicRaycaster);
            UiHits.Clear();
            if (overUi)
            {
                return false;
            }
        }

        Ray ray = worldCamera.ScreenPointToRay(screen);
        if (!MapPlane.Raycast(ray, out float distance))
        {
            return false;
        }

        position = ray.GetPoint(distance);
        return true;
    }

    public static bool TryProject(EnemySpawner pathSource, Vector3 position, float maxDistance, out PathProjectionResult projection)
    {
        projection = default;
        if (pathSource == null || pathSource.Waypoints == null)
        {
            return false;
        }

        var path = new List<Vector3>(pathSource.Waypoints.Count + 1) { pathSource.transform.position };
        foreach (Transform waypoint in pathSource.Waypoints)
        {
            if (waypoint == null) return false;
            path.Add(waypoint.position);
        }

        return PathGeometryUtility.TryProjectPointToPath(position, path, out projection)
            && projection.Distance <= maxDistance;
    }
}

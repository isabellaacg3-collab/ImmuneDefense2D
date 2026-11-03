using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyConfig config;
    private float moveSpeed;
    private int hostDamage;
    private List<Transform> waypoints;
    private Transform gatherPoint;
    private Transform exitPoint;
    private HostHealth hostHealth;
    private MovePhase phase = MovePhase.Incoming;
    private int currentWaypointIndex = 0;
    private GameStateController gameState;
    private HashSet<ReinforcementUnit> blockers = new HashSet<ReinforcementUnit>();
    private float lateralOffset;
    private float transitionDistance;
    private Vector3 pathPosition;
    private float combatPathLength;
    private float combatDistance;
    private enum MovePhase
    {
        Incoming,
        Combat,
        Outgoing
    }
    public bool CanBeTargeted
    {
        get
        {
            return phase == MovePhase.Combat;
        }
    }
    public bool CountsForVictory
    {
        get
        {
            return phase != MovePhase.Outgoing;
        }
    }
    public int BlockerCount
    {
        get
        {
            return blockers.Count;
        }
    }
    public event Action<EnemyConfig> EnteredCombat;
    private void ReachHost()
    {
        phase = MovePhase.Outgoing;
        hostHealth.TakeDamage(hostDamage);
    }
    public void Initialize(List<Transform>path,HostHealth host, Transform gatherPoint, Transform exitPoint, GameStateController gameState,
        float lateralOffset, float transitionDistance)
    {
        phase = MovePhase.Incoming;
        waypoints = path;
        hostHealth = host;
        this.gatherPoint = gatherPoint;
        this.exitPoint = exitPoint;
        this.gameState = gameState;
        this.lateralOffset = lateralOffset;
        this.transitionDistance = transitionDistance;
        pathPosition = transform.position;
        combatPathLength = 0f;
        combatDistance = 0f;
        Vector3 previousPoint = gatherPoint.position;
        foreach(Transform waypoint in waypoints)
        {
            combatPathLength += Vector3.Distance(previousPoint, waypoint.position);
            previousPoint = waypoint.position;
        }
        currentWaypointIndex = 0;
    }
    private Vector3 GetPathPoint(int index)
    {
        if (index < 0)
        {
            return gatherPoint.position;
        }
        return waypoints[index].position;
    }
    private Vector3 GetPathSide(int index)
    {
        Vector3 point = GetPathPoint(index);
        Vector3 previous = GetPathPoint(Mathf.Max(-1, index - 1));
        Vector3 next = GetPathPoint(Mathf.Min(waypoints.Count - 1, index + 1));

        Vector3 incoming = (point - previous).normalized;
        Vector3 outgoing = (next - point).normalized;
        Vector3 forward = (incoming + outgoing).normalized;

        return new Vector3(-forward.y, forward.x, 0f);
    }
    private void Awake()
    {
        moveSpeed = config.MoveSpeed;
        hostDamage = config.HostDamage;
    }
    void Update()
    {
        if (!gameState.CanSimulate || blockers.Count > 0)
        {
            return;
        }

        Transform target;
        if (phase == MovePhase.Incoming)
        {
            target = gatherPoint;
        }else if(phase == MovePhase.Combat)
        {
            if (currentWaypointIndex >= waypoints.Count)
            {
                return;
            }
            target = waypoints[currentWaypointIndex];
        }
        else if(phase == MovePhase.Outgoing)
        {
            target = exitPoint;
        }
        else
        {
            return;
        }
        Vector3 previousPosition = pathPosition;

        pathPosition = Vector3.MoveTowards(
            pathPosition,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(pathPosition, target.position) < 0.01f)
        {
            pathPosition = target.position;
        }

        transform.position = pathPosition;

        if (phase == MovePhase.Combat)
        {
            combatDistance += Vector3.Distance(previousPosition, pathPosition);

            int startIndex = currentWaypointIndex - 1;
            Vector3 start = GetPathPoint(startIndex);
            float segmentLength = Vector3.Distance(start, target.position);
            float progress = Mathf.InverseLerp(0f,segmentLength,Vector3.Distance(start, pathPosition));

            Vector3 side = Vector3.Lerp(
                GetPathSide(startIndex),
                GetPathSide(currentWaypointIndex),
                progress
            ).normalized;

            float edgeDistance = Mathf.Min(combatDistance,combatPathLength - combatDistance);

            float fade = transitionDistance > 0f
                ? Mathf.SmoothStep(0f, 1f, edgeDistance / transitionDistance)
                : 1f;

            transform.position = pathPosition + side * lateralOffset * fade;
        }

        if (Vector3.Distance(pathPosition,target.position) < 0.01f)
        {
            switch (phase)
            {
                case MovePhase.Incoming:
                    phase = MovePhase.Combat;
                    EnteredCombat?.Invoke(config);
                    break;
                case MovePhase.Combat:
                    currentWaypointIndex++;
                    if(currentWaypointIndex >= waypoints.Count)
                    {
                        ReachHost();
                    }
                    break;
                case MovePhase.Outgoing:
                    Destroy(gameObject);
                    break;
            }
        }
    }
    public bool TryBlock(ReinforcementUnit unit)
    {
        if(unit == null || CanBeTargeted == false)
        {
            return false;
        }
        blockers.Add(unit);
        return true;
    }
    public void ReleaseBlock(ReinforcementUnit unit)
    {
        blockers.Remove(unit);
    }
}

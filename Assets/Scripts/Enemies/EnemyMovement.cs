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
    private HashSet<Component> blockers = new HashSet<Component>();
    private float lateralOffset;
    private float transitionDistance;
    private Vector3 pathPosition;
    private float combatPathLength;
    private float combatDistance;
    private Dictionary<TowerAttack, float> slowMultipliers = new Dictionary<TowerAttack, float>();
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
    public void InitializeFromParent(EnemyMovement parent, float childLaneOffset)
    {
        waypoints = parent.waypoints;
        gatherPoint = parent.gatherPoint;
        exitPoint = parent.exitPoint;
        hostHealth = parent.hostHealth;
        gameState = parent.gameState;
        pathPosition = parent.pathPosition;
        currentWaypointIndex = parent.currentWaypointIndex;
        combatPathLength = parent.combatPathLength;
        combatDistance = parent.combatDistance;
        transitionDistance = parent.transitionDistance;
        lateralOffset = Mathf.Lerp(parent.lateralOffset, childLaneOffset, 0.4f);
        phase = MovePhase.Combat;
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
    private float GetSlowMultiplier()
    {
        float result = 1.0f;
        foreach(float multiplier in slowMultipliers.Values)
        {
            result = Mathf.Min(result, multiplier);
        }
        return result;
    }
    public bool TryBlock(Component unit)
    {
        if(unit == null || CanBeTargeted == false)
        {
            return false;
        }
        blockers.Add(unit);
        return true;
    }
    public void ReleaseBlock(Component unit)
    {
        blockers.Remove(unit);
    }
    public void SetSlowMultiplier(TowerAttack source, float multiplier)
    {
        if(source == null)
        {
            return;
        }
        slowMultipliers[source] = Mathf.Clamp01(multiplier);
    }
    public void RemoveSlowMultiplier(TowerAttack source)
    {
        if(source == null)
        {
            return;
        }
        slowMultipliers.Remove(source);
    }
    private void Awake()
    {
        moveSpeed = config.MoveSpeed;
        hostDamage = config.HostDamage;
    }
    private bool TryInterceptTransport(Vector3 previousPosition)
    {
        float step = Vector3.Distance(previousPosition, pathPosition);
        if (step <= 0f ||
            !RedBloodCellTransportUnit.TryIntercept(this, gameState, combatDistance, step, out float allowedStep))
        {
            return false;
        }

        pathPosition = Vector3.MoveTowards(previousPosition, pathPosition, allowedStep);
        return true;
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
        }
        else if (phase == MovePhase.Combat)
        {
            if (currentWaypointIndex >= waypoints.Count)
            {
                return;
            }
            target = waypoints[currentWaypointIndex];
        }
        else if (phase == MovePhase.Outgoing)
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
            moveSpeed * Time.deltaTime * GetSlowMultiplier()
        );

        if (Vector3.Distance(pathPosition, target.position) < 0.01f)
        {
            pathPosition = target.position;
        }

        bool intercepted = phase == MovePhase.Combat && TryInterceptTransport(previousPosition);
        transform.position = pathPosition;

        if (phase == MovePhase.Combat)
        {
            combatDistance += Vector3.Distance(previousPosition, pathPosition);

            int startIndex = currentWaypointIndex - 1;
            Vector3 start = GetPathPoint(startIndex);
            float segmentLength = Vector3.Distance(start, target.position);
            float progress = Mathf.InverseLerp(0f, segmentLength, Vector3.Distance(start, pathPosition));

            Vector3 side = Vector3.Lerp(
                GetPathSide(startIndex),
                GetPathSide(currentWaypointIndex),
                progress
            ).normalized;

            float edgeDistance = Mathf.Min(combatDistance, combatPathLength - combatDistance);

            float fade = transitionDistance > 0f
                ? Mathf.SmoothStep(0f, 1f, edgeDistance / transitionDistance)
                : 1f;

            transform.position = pathPosition + side * lateralOffset * fade;
        }

        if (!intercepted && Vector3.Distance(pathPosition, target.position) < 0.01f)
        {
            switch (phase)
            {
                case MovePhase.Incoming:
                    phase = MovePhase.Combat;
                    EnteredCombat?.Invoke(config);
                    break;
                case MovePhase.Combat:
                    currentWaypointIndex++;
                    if (currentWaypointIndex >= waypoints.Count)
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
}

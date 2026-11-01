using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

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
    private void ReachHost()
    {
        phase = MovePhase.Outgoing;
        hostHealth.TakeDamage(hostDamage);
    }
    public void Initialize(List<Transform>path,HostHealth host, Transform gatherPoint, Transform exitPoint, GameStateController gameState)
    {
        phase = MovePhase.Incoming;
        waypoints = path;
        hostHealth = host;
        this.gatherPoint = gatherPoint;
        this.exitPoint = exitPoint;
        this.gameState = gameState;
        currentWaypointIndex = 0;
    }
    private void Awake()
    {
        moveSpeed = config.MoveSpeed;
        hostDamage = config.HostDamage;
    }
    void Start()
    {
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
        
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position,target.position) < 0.01f)
        {
            switch (phase)
            {
                case MovePhase.Incoming:
                    phase = MovePhase.Combat;
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

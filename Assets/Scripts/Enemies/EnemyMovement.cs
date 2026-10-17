using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyConfig config;
    private List<Transform> waypoints;
    [SerializeField] private float moveSpeed;
    private HostHealth hostHealth;
    [SerializeField] private int hostDamage;
    private int currentWaypointIndex = 0;private GameStateController gameState;
    private void ReachHost()
    {
        hostHealth.TakeDamage(hostDamage);
        Destroy(gameObject);
    }

    public void Initialize(List<Transform>path,HostHealth host,GameStateController gameState)
    {
        waypoints = path;
        hostHealth = host;
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
        if (!gameState.IsPlaying)
        {
            return;
        }
        if (currentWaypointIndex >= waypoints.Count)
        {
            return;
        }

        Transform target = waypoints[currentWaypointIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position,target.position) < 0.01f)
        {
            currentWaypointIndex++;
            if(currentWaypointIndex >= waypoints.Count)
            {
                ReachHost();
            }
        }
    }
}

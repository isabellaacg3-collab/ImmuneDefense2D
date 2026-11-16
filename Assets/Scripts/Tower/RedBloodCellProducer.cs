using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TowerInstance))]
public class RedBloodCellProducer : MonoBehaviour
{
    [SerializeField] private RedBloodCellTransportUnit unitPrefab;
    [SerializeField, Min(0.01f)] private float dispatchInterval = 8f;
    [SerializeField, Min(1)] private int maxInTransit = 1;
    [SerializeField, Min(0.01f)] private float moveSpeed = 0.9f;
    [SerializeField, Min(0)] private int deliveryReward = 25;

    private readonly List<RedBloodCellTransportUnit> inTransit = new List<RedBloodCellTransportUnit>();
    private GameStateController gameState;
    private RedBloodCellTransportRoute route;
    private float timeUntilDispatch;

    private void Start()
    {
        gameState = GetComponent<TowerInstance>().GameState;
        if (gameState != null)
        {
            gameState.TryGetComponent(out route);
        }
        timeUntilDispatch = dispatchInterval;
    }

    private void Update()
    {
        if (gameState == null || !gameState.CanSimulate || route == null || route.ImmunePoints == null
            || unitPrefab == null || dispatchInterval <= 0f || moveSpeed <= 0f)
        {
            return;
        }

        timeUntilDispatch = Mathf.Max(0f, timeUntilDispatch - Time.deltaTime);
        if (timeUntilDispatch > 0f)
        {
            return;
        }

        inTransit.RemoveAll(unit => unit == null);
        if (inTransit.Count >= maxInTransit)
        {
            return;
        }

        if (!route.TryCreateRoute(transform.position, out Vector3[] destinations, out float entryDistance))
        {
            timeUntilDispatch = dispatchInterval;
            return;
        }

        RedBloodCellTransportUnit unit = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        unit.Initialize(destinations, entryDistance, gameState, route.ImmunePoints, moveSpeed, deliveryReward);
        inTransit.Add(unit);
        timeUntilDispatch = dispatchInterval;
    }

    private void OnDisable()
    {
        foreach (RedBloodCellTransportUnit unit in inTransit)
        {
            if (unit == null) continue;
            unit.gameObject.SetActive(false);
            Destroy(unit.gameObject);
        }
        inTransit.Clear();
    }
}

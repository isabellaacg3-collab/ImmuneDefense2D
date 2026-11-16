using UnityEngine;

[DisallowMultipleComponent]
public class RedBloodCellTransportUnit : MonoBehaviour
{
    private Vector3[] destinations;
    private int nextDestination;
    private GameStateController gameState;
    private ImmunePoints immunePoints;
    private float moveSpeed;
    private int deliveryReward;

    public void Initialize(Vector3[] route, GameStateController state, ImmunePoints points, float speed, int reward)
    {
        destinations = route;
        gameState = state;
        immunePoints = points;
        moveSpeed = speed;
        deliveryReward = reward;
    }

    private void Update()
    {
        if (destinations == null || gameState == null || !gameState.CanSimulate)
        {
            return;
        }

        float remaining = moveSpeed * Time.deltaTime;
        while (nextDestination < destinations.Length)
        {
            Vector3 target = destinations[nextDestination];
            float distance = Vector3.Distance(transform.position, target);
            if (distance > remaining)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, remaining);
                return;
            }

            transform.position = target;
            remaining -= distance;
            nextDestination++;
        }

        // Mark completion before Grant, which can notify listeners synchronously.
        destinations = null;
        Destroy(gameObject);
        if (immunePoints != null)
        {
            immunePoints.Grant(deliveryReward);
        }
    }
}

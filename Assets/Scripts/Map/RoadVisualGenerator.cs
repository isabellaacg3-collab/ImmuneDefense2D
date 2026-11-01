using UnityEngine;

public class RoadVisualGenerator : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float sampleSpace = 0.3f;
    [SerializeField] private float roadWidth = 1.2f;
    [SerializeField] private int seed = 1001;
    [SerializeField] private float branchDensity = 0.9f;
    [SerializeField] private float centerDensity = 0.1f;
    [SerializeField] private float edgeDensity = 0.9f;
    [SerializeField] private float gatherTransitionLength = 2f;
    private System.Random random;
    private float mainPathLength;
    private float CalculateMainPathLength()
    {
        float totalLength = 0f;
        Vector3 previous = spawner.transform.position;
        foreach (Transform waypoint in spawner.Waypoints)
        {
            totalLength += Vector3.Distance(previous, waypoint.position);
            previous = waypoint.position;
        }
        return totalLength;
    }
    private void GenerateSegment(Vector3 start,Vector3 end, bool isCombatPath, float startDistance = 0f)
    {
        float length = Vector3.Distance(start, end);
        if(length == 0)
        {
            return;
        }
        Vector3 direction = (end - start).normalized;
        Vector3 normal = new Vector3(-direction.y, direction.x, 0f);
        int steps = Mathf.CeilToInt(length / sampleSpace);
        int lateralSteps = Mathf.CeilToInt(roadWidth / sampleSpace);
        for (int i = 0;i <= steps; i++)
        {
            float t = (float)i / steps;
            float distanceAlongPath = startDistance + length * t;
            float gatherBlend = Mathf.SmoothStep(0f, 1f, distanceAlongPath / gatherTransitionLength);
            Vector3 position = Vector3.Lerp(start, end, t);
            for(int j = 0;j <= lateralSteps; j++)
            {
                float u = (float)j / lateralSteps;
                float edgeFactor = Mathf.Abs(2f * u - 1f);
                float offset = Mathf.Lerp(-roadWidth / 2f, roadWidth / 2f, u);
                float curvedFactor = Mathf.Pow(edgeFactor, 2f);
                float chance;
                if (isCombatPath)
                {
                    float combatChance = Mathf.Lerp(centerDensity, edgeDensity, curvedFactor);
                    chance = Mathf.Lerp(branchDensity, combatChance, gatherBlend);
                }
                else
                {
                    chance = branchDensity;
                }
                Vector3 tilePosition = position + normal * offset; if (random.NextDouble() < chance)
                {
                    Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                }
            }
        }
    }
    void Start()
    {
        mainPathLength = CalculateMainPathLength();
        random = new System.Random(seed);
        Vector3 previous = spawner.transform.position;
        foreach(ScenePointBinding binding in spawner.SpawnPoints)
        {
            GenerateSegment(binding.Point.position,spawner.transform.position,false);
        }
        float passedDistance = 0f;
        foreach(Transform waypoint in spawner.Waypoints)
        {
            GenerateSegment(previous, waypoint.position,true,passedDistance);
            passedDistance += Vector3.Distance(previous, waypoint.position);
            previous = waypoint.position;
        }
        foreach(ScenePointBinding binding in spawner.HostPoints)
        {
            GenerateSegment(previous,binding.Point.position,false);
        }
    }
}

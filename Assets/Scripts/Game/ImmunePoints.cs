using System;
using UnityEngine;

public class ImmunePoints : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] int points;
    public int Points
    {
        get
        {
            return points;
        }
    }
    public event Action<int> PointsChanged;
    private void Awake()
    {
        points = levelConfig.ResourceConfig.InitialImmunePoints;
        PointsChanged?.Invoke(points);
    }
    [ContextMenu("Test Spend")]
    private void TestSpend()
    {
        Debug.Log(TrySpend(60));
    }
    public bool TrySpend(int cost)
    {
        if(cost < 0)
        {
            return false;
        }
        if(cost > points)
        {
            return false;
        }
        else
        {
            points -= cost;
            PointsChanged?.Invoke(points);
            return true;
        }
    }
}

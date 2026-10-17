using UnityEngine;
using System;

public class HostHealth : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    private int health;
    public event Action<int> HealthChanged;
    public event Action Died;

    public int Health
    {
        get
        {
            return health;
        }
    }
    public void TakeDamage(int damage)
    {
        if(health <= 0)
        {
            return;
        }
        health -= damage;
        if(health <= 0)
        {
            health = 0;
        }
        HealthChanged?.Invoke(health);
        if(health == 0)
        {
            Died?.Invoke();
        }
        Debug.Log("Current health: " + health);
    }
    private void Awake()
    {
        health = levelConfig.InitialHostHealth;
        HealthChanged?.Invoke(health);
    }
}

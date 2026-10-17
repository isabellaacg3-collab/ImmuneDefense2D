using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "ImmuneDefense/Level Config")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private List<WaveConfig> waves = new List<WaveConfig>();
    [SerializeField] private ResourceConfig resourceConfig;
    [SerializeField] private int initialHostHealth = 20;
    public IReadOnlyList<WaveConfig> Waves
    {
        get
        {
            return waves;
        }
    }
    public ResourceConfig ResourceConfig
    {
        get
        {
            return resourceConfig;
        }
    }
    public int InitialHostHealth
    {
        get
        {
            return initialHostHealth;
        }
    }
}

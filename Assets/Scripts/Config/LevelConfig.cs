using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "ImmuneDefense/Level Config")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private List<WaveConfig> waves = new List<WaveConfig>();
    [SerializeField] private ResourceConfig resourceConfig;
    [SerializeField] private int initialHostHealth = 20;
    [SerializeField] private List<WaveStartEventConfig> waveStartEvents = new List<WaveStartEventConfig>();
    [SerializeField] private TutorialCardConfig openingTutorialCard;
    [SerializeField] private List<TowerInstance> buildableTowers = new List<TowerInstance>();

    public IReadOnlyList<TowerInstance> BuildableTowers => buildableTowers;

    public bool AllowsTower(TowerInstance prefab)
    {
        return buildableTowers.Contains(prefab);
    }

    public TutorialCardConfig OpeningTutorialCard
    {
        get
        {
            return openingTutorialCard;
        }
    }
    public IReadOnlyList<WaveStartEventConfig> WaveStartEvents
    {
        get
        {
            return waveStartEvents;
        }
    }
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

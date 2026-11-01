using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ImmuneDefense/Level Config")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private List<WaveConfig> waves = new List<WaveConfig>();
    [SerializeField] private ResourceConfig resourceConfig;
    [SerializeField] private int initialHostHealth = 20;
    [SerializeField] private List<WaveClearEventConfig> waveClearEvents = new List<WaveClearEventConfig>();
    [SerializeField] private TutorialCardConfig openingTutorialCard;
    public TutorialCardConfig OpeningTutorialCard
    {
        get
        {
            return openingTutorialCard;
        }
    }
    public IReadOnlyList<WaveClearEventConfig> WaveClearEvents
    {
        get
        {
            return waveClearEvents;
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

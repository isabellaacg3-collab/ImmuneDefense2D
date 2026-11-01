using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] private List<SpawnGroupConfig> groups = new List<SpawnGroupConfig>();
    [SerializeField] float startAtSeconds = 5f;
    [SerializeField] private float delayAfterPreviousWave = 0f;
    public IReadOnlyList<SpawnGroupConfig> Groups
    {
        get
        {
            return groups;
        }
    }
    public float StartAtSeconds
    {
        get
        {
            return startAtSeconds;
        }
    }
    // Additional simulation time after the previous wave and its flow event finish.
    public float DelayAfterPreviousWave
    {
        get
        {
            return delayAfterPreviousWave;
        }
    }
}

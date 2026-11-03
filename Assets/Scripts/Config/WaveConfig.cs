using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] private List<SpawnGroupConfig> groups = new List<SpawnGroupConfig>();
    [SerializeField] float startAtSeconds = 5f;
    [SerializeField, Min(0f)] private float startIntervalSeconds = 0f;
    [SerializeField] private bool earlyCallEnabled = false;
    [SerializeField, Min(0f)] private float earlyCallWindowSeconds = 0f;
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
    // Minimum simulation time since the previous wave actually started; ignored for wave one.
    public float StartIntervalSeconds
    {
        get
        {
            return startIntervalSeconds;
        }
    }
    public bool EarlyCallEnabled
    {
        get
        {
            return earlyCallEnabled;
        }
    }
    // Zero opens the whole countdown; positive values open only the final N seconds.
    public float EarlyCallWindowSeconds
    {
        get
        {
            return earlyCallWindowSeconds;
        }
    }
}

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Wave Start Event")]
public class WaveStartEventConfig : ScriptableObject
{
    [SerializeField] private int beforeWaveNumber = 2;
    [SerializeField] private int grantPoints = 100;
    [SerializeField] private int requiredTowerCount = 2;
    [SerializeField] private PausePolicyConfig pausePolicy;
    [SerializeField] private TutorialCardConfig tutorialCard;
    [SerializeField] private bool requireReinforcementDeployment = false;
    [SerializeField] private List<TowerConfig> unlockUpgradeTargets = new List<TowerConfig>();
    public IReadOnlyList<TowerConfig> UnlockUpgradeTargets
    {
        get
        {
            return unlockUpgradeTargets;
        }
    }
    public int BeforeWaveNumber
    {
        get
        {
            return beforeWaveNumber;
        }
    }
    public int GrantPoints
    {
        get
        {
            return grantPoints;
        }
    }
    public int RequiredTowerCount
    {
        get
        {
            return requiredTowerCount;
        }
    }
    public PausePolicyConfig PausePolicy
    {
        get
        {
            return pausePolicy;
        }
    }
    public TutorialCardConfig TutorialCard
    {
        get
        {
            return tutorialCard;
        }
    }
    public bool RequireReinforcementDeployment
    {
        get
        {
            return requireReinforcementDeployment;
        }
    }
}

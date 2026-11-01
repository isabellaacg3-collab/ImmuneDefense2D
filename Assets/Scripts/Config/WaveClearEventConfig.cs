using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Wave Clear Event")]
public class WaveClearEventConfig : ScriptableObject
{
    [SerializeField] private int afterWaveNumber = 1;
    [SerializeField] private int grantPoints = 100;
    [SerializeField] private int requiredTowerCount = 2;
    [SerializeField] private PausePolicyConfig pausePolicy;
    [SerializeField] private TutorialCardConfig tutorialCard;
    [SerializeField] private bool requireReinforcementDeployment = false;
    public int AfterWaveNumber
    {
        get
        {
            return afterWaveNumber;
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

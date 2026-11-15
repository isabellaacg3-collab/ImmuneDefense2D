using UnityEngine;
using System.Collections.Generic;

public class LevelFlowController : MonoBehaviour
{
    [SerializeField] private List<TowerSlot> towerSlots = new List<TowerSlot>();
    [SerializeField] private GameStateController gameState;
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private ImmunePoints immunePoints;
    [SerializeField] private TutorialCardUI tutorialCardUI;
    [SerializeField] private ReinforcementController reinforcementController;
    private WaveStartEventConfig activeWaveStartEvent = null;
    private int activePausedID = 0;
    private int placedTowerCount = 0;
    private HashSet<int> completedWaveStartEvents = new HashSet<int>();
    private bool hasDeploymentDuringEvent = false;
    private void HandleReinforcementDeployed()
    {
        if(activeWaveStartEvent == null)
        {
            return;
        }
        hasDeploymentDuringEvent = true;
        TryCompleteWaveStartEvent();
        Debug.Log("流程接收：援军部署");
    }
    public bool TryPrepareWave(int waveNumber)
    {
        WaveStartEventConfig waveStartEvent = FindWaveStartEvent(waveNumber);
        if(waveStartEvent == null)
        {
            return true;
        }
        if (completedWaveStartEvents.Contains(waveStartEvent.BeforeWaveNumber))
        {
            return true;
        }
        if(activeWaveStartEvent != null)
        {
            return false;
        }
        activeWaveStartEvent = waveStartEvent;
        hasDeploymentDuringEvent = false;
        if (waveStartEvent.RequireReinforcementDeployment)
        {
            reinforcementController.Unlock();
            reinforcementController.ResetCooldown();
        }
        foreach(TowerConfig target in waveStartEvent.UnlockUpgradeTargets)
        {
            gameState.UnlockTowerUpgrade(target);
        }
        immunePoints.Grant(waveStartEvent.GrantPoints);
        activePausedID = gameState.RequestPause(waveStartEvent.PausePolicy);
        tutorialCardUI.Open(waveStartEvent.TutorialCard);
        TryCompleteWaveStartEvent();
        return completedWaveStartEvents.Contains(waveStartEvent.BeforeWaveNumber);
    }
    private void TryCompleteWaveStartEvent()
    {
        if(activeWaveStartEvent == null)
        {
            return;
        }
        if(tutorialCardUI.IsOpen || placedTowerCount < activeWaveStartEvent.RequiredTowerCount)
        {
            return;
        }
        if(activeWaveStartEvent.RequireReinforcementDeployment && !hasDeploymentDuringEvent)
        {
            return;
        }
        completedWaveStartEvents.Add(activeWaveStartEvent.BeforeWaveNumber);
        gameState.ReleasePause(activePausedID);
        activePausedID = 0;
        activeWaveStartEvent = null;
    }
    public bool AreAllWaveStartEventsCompleted()
    {
        foreach(WaveStartEventConfig config in levelConfig.WaveStartEvents)
        {
            if (!completedWaveStartEvents.Contains(config.BeforeWaveNumber))
            {
                return false;
            }
        }
        return true;
    }
    private WaveStartEventConfig FindWaveStartEvent(int waveNumber)
    {
        foreach(WaveStartEventConfig config in levelConfig.WaveStartEvents)
        {
            if(config.BeforeWaveNumber == waveNumber)
            {
                return config;
            }
        }
        return null;
    }
    private void HandleTowerBuilt(TowerInstance tower)
    {
        placedTowerCount++;
        Debug.Log(placedTowerCount);
        if(placedTowerCount == 1)
        {
            gameState.BeginBattle();
        }
        TryCompleteWaveStartEvent();
    }
    private void OnEnable()
    {
        foreach(TowerSlot slot in towerSlots)
        {
            slot.TowerBuilt += HandleTowerBuilt;
        }
        tutorialCardUI.Closed += TryCompleteWaveStartEvent;
        reinforcementController.Deployed += HandleReinforcementDeployed; 
    }
    private void OnDisable()
    {
        foreach (TowerSlot slot in towerSlots)
        {
            slot.TowerBuilt -= HandleTowerBuilt;
        }
        tutorialCardUI.Closed -= TryCompleteWaveStartEvent;
        reinforcementController.Deployed -= HandleReinforcementDeployed;
    }
    private void Start()
    {
        tutorialCardUI.Open(levelConfig.OpeningTutorialCard);
    }
}

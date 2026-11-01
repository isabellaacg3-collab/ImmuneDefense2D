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
    private WaveClearEventConfig activeWaveEvent = null;
    private int activePausedID = 0;
    private int placedTowerCount = 0;
    private HashSet<int> completedWaveEvents = new HashSet<int>();
    private bool hasDeploymentDuringEvent = false;
    private void HandleReinforcementDeployed()
    {
        if(activeWaveEvent == null)
        {
            return;
        }
        hasDeploymentDuringEvent = true;
        TryCompleteWaveEvent();
        Debug.Log("流程接收：援军部署");
    }
    public void HandleWaveCleared(int waveNumber)
    {
        Debug.Log($"第 {waveNumber} 波清场");
        if(completedWaveEvents.Contains(waveNumber) || activeWaveEvent != null)
        {
            return;
        }
        WaveClearEventConfig waveClearEvent = FindWaveClearEvent(waveNumber);
        if(waveClearEvent == null)
        {
            return;
        }
        activeWaveEvent = waveClearEvent;
        hasDeploymentDuringEvent = false;
        if (waveClearEvent.RequireReinforcementDeployment)
        {
            reinforcementController.Unlock();
            reinforcementController.ResetCooldown();
        }
        immunePoints.Grant(waveClearEvent.GrantPoints);
        activePausedID = gameState.RequestPause(waveClearEvent.PausePolicy);
        tutorialCardUI.Open(waveClearEvent.TutorialCard);
        TryCompleteWaveEvent();
    }
    private void TryCompleteWaveEvent()
    {
        if(activeWaveEvent == null)
        {
            return;
        }
        if(tutorialCardUI.IsOpen || placedTowerCount < activeWaveEvent.RequiredTowerCount)
        {
            return;
        }
        if(activeWaveEvent.RequireReinforcementDeployment && !hasDeploymentDuringEvent)
        {
            return;
        }
        completedWaveEvents.Add(activeWaveEvent.AfterWaveNumber);
        gameState.ReleasePause(activePausedID);
        activePausedID = 0;
        activeWaveEvent = null;
    }
    public bool AreAllWaveEventsCompleted()
    {
        foreach(WaveClearEventConfig config in levelConfig.WaveClearEvents)
        {
            if (!completedWaveEvents.Contains(config.AfterWaveNumber))
            {
                return false;
            }
        }
        return true;
    }
    public bool CanStartWave(int waveNumber)
    {
        foreach(WaveClearEventConfig config in levelConfig.WaveClearEvents)
        {
            if(config.AfterWaveNumber < waveNumber && !completedWaveEvents.Contains(config.AfterWaveNumber))
            {
                return false;
            }
        }
        return true;
    }
    private WaveClearEventConfig FindWaveClearEvent(int waveNumber)
    {
        foreach(WaveClearEventConfig config in levelConfig.WaveClearEvents)
        {
            if(config.AfterWaveNumber == waveNumber)
            {
                return config;
            }
        }
        return null;
    }
    private void HandleTowerBuilt(TowerAttack tower)
    {
        placedTowerCount++;
        Debug.Log(placedTowerCount);
        if(placedTowerCount == 1)
        {
            gameState.BeginBattle();
        }
        TryCompleteWaveEvent();
    }
    private void OnEnable()
    {
        foreach(TowerSlot slot in towerSlots)
        {
            slot.TowerBuilt += HandleTowerBuilt;
        }
        tutorialCardUI.Closed += TryCompleteWaveEvent;
        reinforcementController.Deployed += HandleReinforcementDeployed; 
    }
    private void OnDisable()
    {
        foreach (TowerSlot slot in towerSlots)
        {
            slot.TowerBuilt -= HandleTowerBuilt;
        }
        tutorialCardUI.Closed -= TryCompleteWaveEvent;
        reinforcementController.Deployed -= HandleReinforcementDeployed;
    }
    private void Start()
    {
        tutorialCardUI.Open(levelConfig.OpeningTutorialCard);
    }
}

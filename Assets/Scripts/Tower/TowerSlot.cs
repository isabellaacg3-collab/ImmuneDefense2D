using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class TowerSlot : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private GameStateController gamestate;
    [SerializeField] private ImmunePoints immunePoints;
    [SerializeField] private TowerInstance towerPrefab;
    [SerializeField] private BuildMenuUI buildMenu;
    [SerializeField] private TowerActionUI towerActionUI;
    private TowerInstance currentTower;
    public IReadOnlyList<TowerInstance> BuildableTowers => gamestate.LevelConfig?.BuildableTowers;

    public bool CanUpgrade
    {
        get
        {
            if (!gamestate.CanBuild || currentTower == null)
            {
                return false;
            }
            TowerConfig config = currentTower.Config;
            if (config == null || config.UpgradeTarget == null || config.UpgradeTarget == config)
            {
                return false;
            }
            return gamestate.IsTowerUpgradeUnlocked(config.UpgradeTarget);
        }
    }
    public event Action<TowerInstance> TowerBuilt;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        OpenBuildMenu();
    }
    public void OpenBuildMenu()
    {
        if (!gamestate.CanBuild)
        {
            return;
        }
        if(currentTower != null)
        {
            buildMenu.Close();
            if (towerActionUI != null) towerActionUI.Open(currentTower, this);
            return;
        }
        else
        {
            if (towerActionUI != null) towerActionUI.Close();
            buildMenu.Open(this);
        }

    }
    public bool CanBuild(TowerInstance prefab)
    {
        if (!gamestate.CanBuild || currentTower != null || prefab == null || prefab.Config == null)
        {
            return false;
        }

        LevelConfig level = gamestate.LevelConfig;
        bool allowed = level != null ? level.AllowsTower(prefab) : prefab == towerPrefab;
        int cost = prefab.Config.BuildCost;
        return allowed && cost >= 0 && immunePoints.Points >= cost;
    }

    public bool TryBuild()
    {
        return TryBuild(towerPrefab);
    }

    public bool TryBuild(TowerInstance prefab)
    {
        if (!CanBuild(prefab) || !immunePoints.TrySpend(prefab.Config.BuildCost))
        {
            return false;
        }

        currentTower = Instantiate(prefab, transform.position, Quaternion.identity);
        currentTower.Initialize(gamestate);
        TowerBuilt?.Invoke(currentTower);
        return true;
    }
    public bool TryUpgrade()
    {
        if (!CanUpgrade)
        {
            return false;
        }
        TowerConfig config = currentTower.Config;
        int cost = config.UpgradeCost;
        if (!immunePoints.TrySpend(cost))
        {
            return false;
        }
        if (currentTower.TryApplyUpgrade())
        {
            return true;
        }
        else
        {
            immunePoints.Grant(cost);
            return false;
        }
    }
}

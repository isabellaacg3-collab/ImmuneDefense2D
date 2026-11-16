using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class TowerSlot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] private GameStateController gamestate;
    [SerializeField] private ImmunePoints immunePoints;
    [SerializeField] private TowerInstance towerPrefab;
    [SerializeField] private BuildMenuUI buildMenu;
    [SerializeField] private TowerActionUI towerActionUI;
    private TowerInstance currentTower;
    private TowerInstance builtFromPrefab;
    private int investedPoints;
    private bool pointerDownWhileTargeting;
    public IReadOnlyList<TowerInstance> BuildableTowers => gamestate.LevelConfig?.BuildableTowers;

    public bool IsUpgradeUnlocked
    {
        get
        {
            TowerConfig config = currentTower != null ? currentTower.Config : null;
            return config != null && config.UpgradeTarget != null && config.UpgradeTarget != config
                && gamestate.IsTowerUpgradeUnlocked(config.UpgradeTarget);
        }
    }
    public bool CanUpgrade => IsUpgradeUnlocked && gamestate.CanBuild && currentTower.Config.UpgradeCost >= 0
        && immunePoints.Points >= currentTower.Config.UpgradeCost;
    public bool IsSaleUnlocked => gamestate.TowerSaleUnlocked;
    public bool CanSell => currentTower != null && IsSaleUnlocked && gamestate.CanBuild;
    public int SaleRefund => investedPoints / 2;

    public event Action<TowerInstance> TowerBuilt;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            pointerDownWhileTargeting = gamestate.HasRoadSkillSelection;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || pointerDownWhileTargeting || gamestate.HasRoadSkillSelection)
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
    public bool TryGetTowerPlacementLimit(TowerInstance prefab, out int placed, out int maximum)
    {
        LevelConfig level = gamestate.LevelConfig;
        maximum = level != null ? level.GetTowerPlacementLimit(prefab) : 0;
        placed = maximum > 0 ? gamestate.CountBuiltTowers(prefab) : 0;
        return maximum > 0;
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
        bool atLimit = TryGetTowerPlacementLimit(prefab, out int placed, out int maximum) && placed >= maximum;
        return allowed && !atLimit && cost >= 0 && immunePoints.Points >= cost;
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
        builtFromPrefab = prefab;
        investedPoints = prefab.Config.BuildCost;
        gamestate.RegisterBuiltTower(prefab, currentTower);
        TowerBuilt?.Invoke(currentTower);
        return true;
    }
    public bool TrySell()
    {
        if (!CanSell) return false;

        TowerInstance soldTower = currentTower;
        int refund = SaleRefund;
        currentTower = null;
        investedPoints = 0;
        gamestate.UnregisterBuiltTower(builtFromPrefab, soldTower);
        builtFromPrefab = null;

        soldTower.gameObject.SetActive(false);
        Destroy(soldTower.gameObject);
        immunePoints.Grant(refund);
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
            investedPoints += cost;
            return true;
        }
        else
        {
            immunePoints.Grant(cost);
            return false;
        }
    }
}

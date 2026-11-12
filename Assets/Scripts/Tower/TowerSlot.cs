using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TowerSlot : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private GameStateController gamestate;
    [SerializeField] private ImmunePoints immunePoints;
    [SerializeField] private TowerAttack towerPrefab;
    [SerializeField] private BuildMenuUI buildMenu;
    [SerializeField] private TowerActionUI towerActionUI;
    private TowerAttack currentTower;
    public bool CanUpgrade
    {
        get
        {
            if (!gamestate.CanBuild || currentTower == null)
            {
                return false;
            }
            TowerConfig config = currentTower.Config;
            if (config.UpgradeTarget == null || config.UpgradeTarget == config)
            {
                return false;
            }
            return gamestate.IsTowerUpgradeUnlocked(config.UpgradeTarget);
        }
    }
    public event Action<TowerAttack> TowerBuilt;
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
            towerActionUI.Open(currentTower,this);
            return;
        }
        else
        {
            towerActionUI.Close();
            buildMenu.Open(this);
        }

    }
    public bool TryBuild()
    {
        if (!gamestate.CanBuild)
        {
            return false;
        }
        if(currentTower != null)
        {
            return false;
        }
        if (!immunePoints.TrySpend(towerPrefab.Config.BuildCost))
        {
            return false;
        }
        currentTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
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

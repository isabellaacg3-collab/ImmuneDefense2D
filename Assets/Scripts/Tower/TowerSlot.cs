using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TowerSlot : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private GameStateController gamestate;
    [SerializeField] private ImmunePoints immunePoints;
    [SerializeField] private TowerAttack towerPrefab;
    [SerializeField] private BuildMenuUI buildMenu;
    private TowerAttack currentTower;
    [ContextMenu("Test Build")]
    private void TestBuild()
    {
        Debug.Log(TryBuild());
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
            //tower panel
            return;
        }
        else
        {
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
}

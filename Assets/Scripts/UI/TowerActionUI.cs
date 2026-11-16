using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerActionUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private KnowledgeCardController knowledgeCardController;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    private TMP_Text upgradeLabel;
    private TMP_Text sellLabel;
    private TowerInstance selectedTower;
    private TowerSlot selectedSlot;
    private void Awake()
    {
        if (upgradeButton != null) upgradeLabel = upgradeButton.GetComponentInChildren<TMP_Text>(true);
        if (sellButton != null) sellLabel = sellButton.GetComponentInChildren<TMP_Text>(true);
        panel.SetActive(false);
    }
    public void Open(TowerInstance tower,TowerSlot slot)
    {
        if (tower == null || tower.Config == null)
        {
            return;
        }
        selectedSlot = slot;
        selectedTower = tower;
        TowerConfig config = tower.Config;
        if (upgradeLabel != null) upgradeLabel.text = $"升级 · {config.UpgradeCost} 点";
        if (sellLabel != null && slot != null) sellLabel.text = $"拆除 · 返还 {slot.SaleRefund} 点";
        titleText.text = config.DisplayName;
        bodyText.text = !string.IsNullOrWhiteSpace(config.RoleDescription)
            ? config.RoleDescription
            : $"攻击：{config.AttackDamage}   间隔：{config.AttackInterval:F1} 秒   范围：{config.AttackRange:F1}";
        RefreshActions();
        panel.SetActive(true);
    }
    private void Update()
    {
        if (!panel.activeSelf) return;
        if (selectedTower == null)
        {
            Close();
            return;
        }
        RefreshActions();
    }

    private void RefreshActions()
    {
        if (upgradeButton != null)
        {
            bool visible = selectedSlot != null && selectedSlot.IsUpgradeUnlocked;
            upgradeButton.interactable = visible && selectedSlot.CanUpgrade;
            upgradeButton.gameObject.SetActive(visible);
        }
        if (sellButton != null)
        {
            bool visible = selectedSlot != null && selectedSlot.IsSaleUnlocked;
            sellButton.interactable = visible && selectedSlot.CanSell;
            sellButton.gameObject.SetActive(visible);
        }
    }

    public void Close()
    {
        panel.SetActive(false);
        selectedSlot = null;
        selectedTower = null;
    }
    public void OpenDetails()
    {
        if(selectedTower == null)
        {
            return;
        }
        TutorialCardConfig card = selectedTower.Config.KnowledgeCard;
        if(card == null)
        {
            return;
        }
        knowledgeCardController.Open(card);
        Close();
    }
    public void SellSelectedTower()
    {
        if (selectedSlot != null && selectedSlot.TrySell())
        {
            Close();
        }
    }

    public void UpgradeSelectedTower()
    {
        if(selectedSlot == null)
        {
            return;
        }
        if (selectedSlot.TryUpgrade())
        {
            Open(selectedTower, selectedSlot);
        }
    }
}

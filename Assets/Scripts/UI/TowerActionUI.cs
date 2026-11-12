using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerActionUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private KnowledgeCardController knowledgeCardController;
    [SerializeField] Button upgradeButton;
    private TowerAttack selectedTower;
    private TowerSlot selectedSlot;
    private void Awake()
    {
        panel.SetActive(false);
    }
    public void Open(TowerAttack tower,TowerSlot slot)
    {
        if(tower == null)
        {
            return;
        }
        selectedSlot = slot;
        selectedTower = tower;
        if (upgradeButton != null)
        {
            upgradeButton.interactable = selectedSlot != null && selectedSlot.CanUpgrade;
        }
        TowerConfig config = tower.Config;
        titleText.text = config.KnowledgeCard.Title;
        bodyText.text = $"攻击：{config.AttackDamage}   间隔：{config.AttackInterval:F1} 秒   范围：{config.AttackRange:F1}";
        panel.SetActive(true);
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

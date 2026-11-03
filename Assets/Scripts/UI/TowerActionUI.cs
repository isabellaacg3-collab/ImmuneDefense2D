using TMPro;
using UnityEngine;

public class TowerActionUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private KnowledgeCardController knowledgeCardController;
    private TowerAttack selectedTower;
    private void Awake()
    {
        panel.SetActive(false);
    }
    public void Open(TowerAttack tower)
    {
        if(tower == null)
        {
            return;
        }
        selectedTower = tower;
        TowerConfig config = tower.Config;
        titleText.text = config.KnowledgeCard.Title;
        bodyText.text = $"攻击：{config.AttackDamage}   间隔：{config.AttackInterval:F1} 秒   范围：{config.AttackRange:F1}";
        panel.SetActive(true);
    }
    public void Close()
    {
        panel.SetActive(false);
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
}

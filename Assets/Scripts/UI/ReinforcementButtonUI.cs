using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReinforcementButtonUI : MonoBehaviour
{
    [SerializeField] private ReinforcementController controller;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button deployButton;
    [SerializeField] private GameStateController gameState;
    private void Refresh()
    {
        deployButton.interactable = controller.IsUnlocked && gameState.CanDeploy 
            && (controller.IsSelectingPosition || controller.CooldownRemaining <= 0f);
        if (!controller.IsUnlocked)
        {
            buttonText.text = "援军\n未解锁";
        }else if (controller.IsSelectingPosition)
        {
            buttonText.text = "取消部署";
        }else if(controller.CooldownRemaining > 0f)
        {
            buttonText.text = $"援军\n{Mathf.CeilToInt(controller.CooldownRemaining)} 秒";
        }
        else
        {
            buttonText.text = "部署援军";
        }
    }
    private void Update()
    {
        Refresh();
    }
}

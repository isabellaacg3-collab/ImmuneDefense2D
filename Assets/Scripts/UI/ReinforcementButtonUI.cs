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
            buttonText.text = "X";
        }else if (controller.IsSelectingPosition)
        {
            buttonText.text = "取消部署";
        }else if(controller.CooldownRemaining > 0f)
        {
            buttonText.text = $"{controller.CooldownRemaining:F1}";
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

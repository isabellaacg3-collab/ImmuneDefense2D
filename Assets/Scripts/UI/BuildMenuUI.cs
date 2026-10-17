using Unity.VisualScripting;
using UnityEngine;

public class BuildMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private TowerSlot selectedSlot;
    public void Open(TowerSlot slot)
    {
        selectedSlot = slot;
        panel.SetActive(true);
    }
    public void Close()
    {
        panel.SetActive(false);
        selectedSlot = null;
    }
    public void Build()
    {
        if(selectedSlot == null)
        {
            return;
        }
        if (selectedSlot.TryBuild())
        {
            Close();
        }
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button optionTemplate;

    private readonly List<(Button button, TowerInstance prefab)> options = new List<(Button, TowerInstance)>();
    private TowerSlot selectedSlot;

    public void Open(TowerSlot slot)
    {
        ClearOptions();
        selectedSlot = slot;
        if (optionTemplate != null)
        {
            BuildOptions(slot.BuildableTowers);
        }
        panel.SetActive(true);
        RefreshOptions();
    }

    public void Close()
    {
        selectedSlot = null;
        panel.SetActive(false);
        ClearOptions();
    }

    // Used by the original single-tower button in Level00_Test.
    public void Build()
    {
        if (selectedSlot != null && selectedSlot.TryBuild())
        {
            Close();
        }
    }

    private void BuildOptions(IReadOnlyList<TowerInstance> prefabs)
    {
        const float top = 66f;
        const float rowHeight = 56f;
        const float gap = 10f;
        const float bottom = 20f;

        if (prefabs != null)
        {
            foreach (TowerInstance prefab in prefabs)
            {
                if (prefab == null || prefab.Config == null) continue;

                Button button = Instantiate(optionTemplate, optionTemplate.transform.parent);
                button.name = prefab.name + " Option";
                var row = (RectTransform)button.transform;
                row.anchoredPosition = new Vector2(0f, -top - options.Count * (rowHeight + gap));
                row.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rowHeight);
                var label = button.GetComponentInChildren<TMP_Text>(true);
                label.text = $"<b>{prefab.Config.DisplayName}</b>   <color=#C6E7D6>{prefab.Config.BuildCost} 免疫点</color>";
                button.onClick.AddListener(() => Build(prefab));
                options.Add((button, prefab));
                button.gameObject.SetActive(true);
            }
        }

        float rowsHeight = options.Count * rowHeight + Mathf.Max(0, options.Count - 1) * gap;
        ((RectTransform)panel.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, top + rowsHeight + bottom);
    }

    private void Build(TowerInstance prefab)
    {
        if (selectedSlot != null && selectedSlot.TryBuild(prefab))
        {
            Close();
        }
    }

    private void Update()
    {
        if (selectedSlot != null) RefreshOptions();
    }

    private void RefreshOptions()
    {
        foreach (var option in options)
        {
            option.button.interactable = selectedSlot != null && selectedSlot.CanBuild(option.prefab);
        }
    }

    private void ClearOptions()
    {
        foreach (var option in options)
        {
            option.button.gameObject.SetActive(false);
            Destroy(option.button.gameObject);
        }
        options.Clear();
    }
}

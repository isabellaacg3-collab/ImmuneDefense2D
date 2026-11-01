using TMPro;
using UnityEngine;
using System;

public class TutorialCardUI : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text bodyText;
    public bool IsOpen
    {
        get
        {
            return panel.activeSelf;
        }
    }
    private void Awake()
    {
        panel.SetActive(false);
    }
    public void Open(TutorialCardConfig config)
    {
        if (config == null)
        {
            return;
        }
        titleText.text = config.Title;
        bodyText.text = config.Body;
        panel.SetActive(true);
    }
    public void Close()
    {
        if (!panel.activeSelf)
        {
            return;
        }
        panel.SetActive(false);
        Closed?.Invoke();
    }
    public event Action Closed;
}

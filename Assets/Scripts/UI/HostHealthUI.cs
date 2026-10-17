using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HostHealthUI : MonoBehaviour
{
    [SerializeField] private HostHealth host;
    [SerializeField] private TMP_Text healthText;

    private void RefreshHealth(int value)
    {
        healthText.text = "HP: " + value;
    }
    private void OnEnable()
    {
        host.HealthChanged += RefreshHealth;
        RefreshHealth(host.Health);
    }
    private void OnDisable()
    {
        if(host != null)
        {
            host.HealthChanged -= RefreshHealth;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

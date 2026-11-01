using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyConfig config;
    private int health;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            health = 0;
            Destroy(gameObject);
        }
        Debug.Log("Enemy current health:" + health);
    }

    [ContextMenu("Test Damage")]
    private void TestDamage()
    {
        TakeDamage(1);
    }
    private void Awake()
    {
        health = config.MaxHealth;
    }
}

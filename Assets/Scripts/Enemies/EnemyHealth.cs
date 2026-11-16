using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyConfig config;
    private int health;
    private bool isDead;
    public event System.Action<EnemyHealth> Died;
    public EnemyConfig Config => config;
    public bool IsAlive => !isDead && health > 0;

    public void Heal(int amount)
    {
        if (IsAlive && amount > 0)
        {
            health += Mathf.Min(amount, config.MaxHealth - health);
        }
    }

    public void TakeDamage(int damage)
    {
        TryTakeDamage(damage, out _);
    }

    // True only for the call that delivers lethal damage; capture visuals before Destroy.
    public bool TryTakeDamage(int damage, out EnemyVisualSnapshot deathVisual)
    {
        deathVisual = default;
        if (!IsAlive || damage <= 0)
        {
            return false;
        }
        health -= damage;
        if(health <= 0)
        {
            health = 0;
            isDead = true;
            SpriteRenderer visual = GetComponentInChildren<SpriteRenderer>();
            if (visual != null)
            {
                deathVisual = new EnemyVisualSnapshot(visual);
            }
            Died?.Invoke(this);
            Destroy(gameObject);
            return true;
        }
        Debug.Log("Enemy current health:" + health);
        return false;
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

// Display data only: no enemy instance survives in the swallow effect.
public readonly struct EnemyVisualSnapshot
{
    public readonly Sprite Sprite;
    public readonly Vector3 Position;
    public readonly Quaternion Rotation;
    public readonly Vector3 Scale;
    public readonly Color Color;
    public readonly bool FlipX;
    public readonly bool FlipY;
    public readonly SpriteDrawMode DrawMode;
    public readonly Vector2 Size;
    public readonly int SortingLayerID;
    public readonly int SortingOrder;
    public readonly Material Material;
    public EnemyVisualSnapshot(SpriteRenderer renderer)
    {
        Sprite = renderer.sprite;
        Position = renderer.transform.position;
        Rotation = renderer.transform.rotation;
        Scale = renderer.transform.lossyScale;
        Color = renderer.color;
        FlipX = renderer.flipX;
        FlipY = renderer.flipY;
        DrawMode = renderer.drawMode;
        Size = renderer.size;
        SortingLayerID = renderer.sortingLayerID;
        SortingOrder = renderer.sortingOrder;
        Material = renderer.sharedMaterial;
    }
}

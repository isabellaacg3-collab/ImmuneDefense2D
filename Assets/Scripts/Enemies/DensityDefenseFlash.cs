using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(DensityDefense))]
public sealed class DensityDefenseFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer edgeRenderer;
    [SerializeField, Min(0.01f)] private float duration = 0.4f;

    private DensityDefense defense;
    private float remaining;

    private void OnEnable()
    {
        defense = GetComponent<DensityDefense>();
        defense.Activated += Flash;
    }

    private void OnDisable()
    {
        defense.Activated -= Flash;
        remaining = 0f;
        if (edgeRenderer != null) edgeRenderer.enabled = false;
    }

    private void Flash()
    {
        if (edgeRenderer == null || duration <= 0f) return;

        remaining = duration;
        SetIntensity(1f);
    }

    private void LateUpdate()
    {
        if (remaining <= 0f) return;

        remaining = Mathf.Max(0f, remaining - Time.deltaTime);
        SetIntensity(remaining / duration);
    }

    private void SetIntensity(float intensity)
    {
        Color tint = edgeRenderer.color;
        tint.a = intensity;
        edgeRenderer.color = tint;
        edgeRenderer.enabled = intensity > 0f;
    }
}

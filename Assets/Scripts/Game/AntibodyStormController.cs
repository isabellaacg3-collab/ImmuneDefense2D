using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AntibodyStormController : MonoBehaviour
{
    private const float MaxEarlyWaveCooldownReduction = 30f;

    [SerializeField] private GameStateController gameState;
    [SerializeField] private WaveController waveController;
    [SerializeField] private EnemySpawner pathSource;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Button skillButton;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Sprite radiusSprite;
    [SerializeField] private Material radiusMaterial;
    [SerializeField, Min(0.01f)] private float radius = 0.8f;
    [SerializeField, Min(1)] private int damage = 10;
    [SerializeField, Min(0f)] private float cooldownSeconds = 50f;
    [SerializeField, Min(0f)] private float maxDistanceFromRoad = 0.6f;

    private SpriteRenderer preview;
    private bool unlocked;
    private float cooldownRemaining;

    public bool IsSelectingPosition => gameState != null && gameState.IsSelectingRoadSkill(this);
    private bool CanCast => unlocked && cooldownRemaining <= 0f && gameState != null
        && gameState.CanSimulate && gameState.CanDeploy;

    private void Awake()
    {
        if (radiusSprite != null)
        {
            preview = new GameObject("Antibody Storm Radius").AddComponent<SpriteRenderer>();
            preview.transform.SetParent(transform, false);
            preview.sprite = radiusSprite;
            if (radiusMaterial != null) preview.sharedMaterial = radiusMaterial;
            preview.color = new Color(0.48f, 0.74f, 1f, 0.35f);
            preview.sortingOrder = 1;
            preview.enabled = false;
        }
        RefreshButton();
    }

    public void Unlock()
    {
        unlocked = true;
        RefreshButton();
    }

    public void ToggleTargeting()
    {
        if (IsSelectingPosition)
        {
            CancelTargeting();
        }
        else if (CanCast)
        {
            gameState.BeginRoadSkillSelection(this);
        }
        RefreshButton();
    }

    private void CancelTargeting()
    {
        if (gameState != null) gameState.EndRoadSkillSelection(this);
        if (preview != null) preview.enabled = false;
    }

    public bool TryCast(Vector3 position)
    {
        if (!IsSelectingPosition || !CanCast ||
            !RoadSkillTargeting.TryProject(pathSource, position, maxDistanceFromRoad, out PathProjectionResult projection))
        {
            return false;
        }

        cooldownRemaining = cooldownSeconds;
        CancelTargeting();
        ApplyBlast(projection.ProjectedPosition, radius, damage);
        RefreshButton();
        return true;
    }

    public static void ApplyBlast(Vector3 center, float radius, int damage)
    {
        if (radius <= 0f || damage <= 0) return;

        float radiusSquared = radius * radius;
        var targets = new List<EnemyHealth>();
        foreach (EnemyHealth enemy in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            if (enemy.isActiveAndEnabled && enemy.IsAlive && enemy.TryGetComponent(out EnemyMovement movement)
                && movement.CanBeTargeted && (enemy.transform.position - center).sqrMagnitude <= radiusSquared)
            {
                targets.Add(enemy);
            }
        }

        // Lethal damage can spawn eligible children synchronously. Only hit the original snapshot.
        foreach (EnemyHealth enemy in targets)
        {
            if (enemy != null) enemy.TakeDamage(damage);
        }
    }

    private void Update()
    {
        if (gameState != null && gameState.CanSimulate)
        {
            cooldownRemaining = Mathf.Max(0f, cooldownRemaining - Time.deltaTime);
        }
        RefreshButton();
    }

    private void LateUpdate()
    {
        if (preview != null) preview.enabled = false;
        Pointer pointer = Pointer.current;
        if (!IsSelectingPosition || !CanCast || pointer == null) return;

        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelTargeting();
            RefreshButton();
            return;
        }

        if (!RoadSkillTargeting.TryGetWorldPosition(worldCamera, pointer.position.ReadValue(), out Vector3 position)
            || !RoadSkillTargeting.TryProject(pathSource, position, maxDistanceFromRoad, out PathProjectionResult projection))
        {
            return;
        }

        if (preview != null)
        {
            Vector3 size = radiusSprite.bounds.size;
            preview.transform.position = projection.ProjectedPosition;
            preview.transform.localScale = new Vector3(2f * radius / size.x, 2f * radius / size.y, 1f);
            preview.enabled = true;
        }

        if (pointer.press.wasPressedThisFrame) TryCast(position);
    }

    private void RefreshButton()
    {
        if (skillButton != null) skillButton.interactable = CanCast;
        if (buttonText == null) return;

        if (!unlocked)
            buttonText.text = "抗体风暴\n未解锁";
        else if (IsSelectingPosition)
            buttonText.text = "点击道路\n再次点击取消";
        else if (cooldownRemaining > 0f)
            buttonText.text = $"抗体风暴\n{Mathf.CeilToInt(cooldownRemaining)} 秒";
        else
            buttonText.text = "抗体风暴";
    }

    private void HandleEarlyWave(float secondsSaved)
    {
        float reduction = Mathf.Clamp(secondsSaved, 0f, MaxEarlyWaveCooldownReduction);
        cooldownRemaining = Mathf.Max(0f, cooldownRemaining - reduction);
        RefreshButton();
    }

    private void OnEnable()
    {
        if (waveController != null) waveController.WaveCalledEarly += HandleEarlyWave;
    }

    private void OnDisable()
    {
        if (waveController != null) waveController.WaveCalledEarly -= HandleEarlyWave;
        CancelTargeting();
    }
}

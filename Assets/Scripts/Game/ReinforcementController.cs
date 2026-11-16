using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ReinforcementController : MonoBehaviour
{
    [SerializeField] private ReinforcementDeploymentConfig config;
    [SerializeField] private GameStateController gameState;
    [SerializeField] private Transform testDeployPoint;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private WaveController waveController;
    private float cooldownRemaining = 0f;
    private bool isUnlocked = false;
    private List<SpriteRenderer> previewUnits = new List<SpriteRenderer>();
    public event Action Deployed;
    public bool IsUnlocked
    {
        get
        {
            return isUnlocked;
        }
    }
    public bool IsSelectingPosition => gameState != null && gameState.IsSelectingRoadSkill(this);
    public float CooldownRemaining
    {
        get
        {
            return cooldownRemaining;
        }
    }
    [ContextMenu("Test Deploy")]
    private void TestDeploy()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        TryDeploy(testDeployPoint.position);
    }
    public bool TryDeploy(Vector3 position)
    {
        if (!Application.isPlaying)
        {
            return false;
        }
        if (!gameState.CanDeploy)
        {
            return false;
        }
        if (cooldownRemaining > 0f)
        {
            return false;
        }
        if (!isUnlocked)
        {
            return false;
        }
        if (!TryGetDeploymentProjection(position, out PathProjectionResult projection))
        {
            return false;
        }
        for (int i = 0; i < config.UnitCount; i++)
        {
            Vector3 spawnPosition = GetUnitSpawnPosition(projection, i);
            ReinforcementUnit unit = Instantiate(config.UnitPrefab, spawnPosition, Quaternion.identity);
            unit.Initialize(gameState);
        }
        cooldownRemaining = config.CooldownSeconds;
        Deployed?.Invoke();
        return true;
    }
    public void BeginDeploySelection()
    {
        if (IsSelectingPosition)
        {
            CancelDeployment();
            return;
        }
        if(!gameState.CanDeploy || cooldownRemaining > 0f || !isUnlocked)
        {
            return;
        }
        gameState.BeginRoadSkillSelection(this);
        Debug.Log("可手动部署援军");
    }
    public void CancelDeployment()
    {
        if (gameState != null) gameState.EndRoadSkillSelection(this);
        SetPreviewVisible(false);
    }
    private bool TryGetDeploymentProjection(Vector3 position, out PathProjectionResult projection)
    {
        return RoadSkillTargeting.TryProject(enemySpawner, position, config.MaxDisFromPath, out projection);
    }
    private Vector3 GetUnitSpawnPosition(PathProjectionResult projection,int index)
    {

        Vector3 forward = projection.Forward;
        Vector3 side = new Vector3(-forward.y, forward.x, 0f);
        float factor = (index - (config.UnitCount - 1) / 2f) * 2f;
        return projection.ProjectedPosition + forward * config.SpawnForwardOffset * factor + side * config.SpawnSideOffset * factor;
    }
    private void SetPreviewVisible(bool visible)
    {
        foreach(SpriteRenderer preview in previewUnits)
        {
            preview.gameObject.SetActive(visible);
        }
    }
    public void ResetCooldown()
    {
        cooldownRemaining = 0f;
    }
    public void Unlock()
    {
        isUnlocked = true;
    }
    private void Awake()
    {
        for (int i = 0; i < config.UnitCount; i++)
        {
            SpriteRenderer preview = Instantiate(config.PreviewPrefab, transform);
            preview.gameObject.SetActive(false);
            previewUnits.Add(preview);
        }
    }
    private void LateUpdate()
    {
        if (!IsSelectingPosition || !gameState.CanDeploy)
        {
            SetPreviewVisible(false);
            return;
        }
        if (Mouse.current == null)
        {
            SetPreviewVisible(false);
            return;
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelDeployment();
            return;
        }
        if (!RoadSkillTargeting.TryGetPointerPosition(worldCamera, out Vector3 worldPosition)
            || !TryGetDeploymentProjection(worldPosition, out PathProjectionResult projection))
        {
            SetPreviewVisible(false);
            return;
        }
        for(int i = 0;i < config.UnitCount; i++)
        {
            previewUnits[i].transform.position = GetUnitSpawnPosition(projection, i);
        }
        SetPreviewVisible(true);
        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }
        if (TryDeploy(worldPosition))
        {
            CancelDeployment();
        }
    }
    private void Update()
    {
        if (!gameState.CanSimulate)
        {
            return;
        }
        cooldownRemaining = Mathf.Max(0f, cooldownRemaining - Time.deltaTime);
    }
    private void OnEnable()
    {
        waveController.WaveCalledEarly += ResetCooldown;
    }
    private void OnDisable()
    {
        CancelDeployment();
        if (waveController != null)
        {
            waveController.WaveCalledEarly -= ResetCooldown;
        }
    }
}

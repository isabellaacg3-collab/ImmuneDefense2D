using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ReinforcementController : MonoBehaviour
{
    [SerializeField] private ReinforcementDeploymentConfig config;
    [SerializeField] private GameStateController gameState;
    [SerializeField] private Transform testDeployPoint;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private EnemySpawner enemySpawner;
    private float cooldownRemaining = 0f;
    private bool isSelectingPosition = false;
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
    public bool IsSelectingPosition
    {
        get
        {
            return isSelectingPosition;
        }
    }
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
        if (isSelectingPosition)
        {
            CancelDeployment();
            return;
        }
        if(!gameState.CanDeploy || cooldownRemaining > 0f || !isUnlocked)
        {
            return;
        }
        isSelectingPosition = true;
        Debug.Log("可手动部署援军");
    }
    public void CancelDeployment()
    {
        isSelectingPosition = false;
        SetPreviewVisible(false);
    }
    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = screenPosition;
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, hits);
        foreach(RaycastResult hit in hits)
        {
            if(hit.module is GraphicRaycaster)
            {
                return true;
            }
        }
        return false;
    }
    private List<Vector3> BuildDeploymentPath()
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(enemySpawner.transform.position);
        foreach(Transform waypoint in enemySpawner.Waypoints)
        {
            points.Add(waypoint.position);
        }
        return points;
    }
    private bool TryGetDeploymentProjection(Vector3 position,out PathProjectionResult projection)
    {
        List<Vector3> path = BuildDeploymentPath();

        if (!PathGeometryUtility.TryProjectPointToPath(position, path, out projection))
        {
            return false;
        }

        if (projection.Distance > config.MaxDisFromPath)
        {
            return false;
        }

        return true;
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
        if (!isSelectingPosition || !gameState.CanDeploy)
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
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        if (IsPointerOverUI(screenPosition))
        {
            SetPreviewVisible(false);
            return;
        }
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0f));
        worldPosition.z = 0f;
        if(!TryGetDeploymentProjection(worldPosition,out PathProjectionResult projection))
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
}

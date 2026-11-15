using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(TowerAttack))]
public class NeutrophilLinkVisual : MonoBehaviour
{
    [SerializeField] private TowerAttack tower;
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private NeutrophilVisualConfig config;
    private NeutrophilVisualConfig settings;
    private bool ownsSettings;
    private bool subscribed;
    private GameObject runtimeRoot;
    private Material runtimeMaterial;
    private float visualClock;
    private Vector3[] positions;
    private readonly List<ActiveLink> activeLinks = new List<ActiveLink>();
    private readonly List<RetractingLink> retracting = new List<RetractingLink>();
    private readonly List<Swallow> swallows = new List<Swallow>();

    private sealed class ActiveLink
    {
        public EnemyHealth Target;
        public LineRenderer Line;
        public float Extension;
        public bool Seen;
    }
    private sealed class RetractingLink
    {
        public LineRenderer Line;
        public Vector3 End;
        public float Elapsed;
    }
    private sealed class Swallow
    {
        public SpriteRenderer Ghost;
        public LineRenderer Line;
        public Vector3 Start;
        public Vector3 InitialScale;
        public Color InitialColor;
        public float Elapsed;
    }
    private float TowerScale => Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y));
    private Vector3 Origin => (bodyRenderer != null ? bodyRenderer.transform : transform)
        .TransformPoint(new Vector3(settings.OriginLocalOffset.x, settings.OriginLocalOffset.y, 0f));

    private void OnEnable()
    {
        if (tower == null) tower = GetComponent<TowerAttack>();
        if (bodyRenderer == null) bodyRenderer = GetComponent<SpriteRenderer>();
        settings = config;
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<NeutrophilVisualConfig>();
            ownsSettings = true;
        }
        ApplyBodyArt();
        if (tower != null && !subscribed)
        {
            tower.TargetKilled += HandleTargetKilled;
            subscribed = true;
        }
    }
    private void ApplyBodyArt()
    {
        if (settings != null && settings.BodySprite != null && bodyRenderer != null)
        {
            bodyRenderer.sprite = settings.BodySprite;
            bodyRenderer.color = settings.BodyTint;
        }
    }
    private void EnsureRuntime()
    {
        if (runtimeRoot == null)
        {
            // Identity world-space parent preserves the captured enemy's world scale.
            runtimeRoot = new GameObject(name + " Visual Effects");
            runtimeRoot.layer = gameObject.layer;
        }
        if (runtimeMaterial == null)
        {
            if (settings.TentacleMaterial != null)
                runtimeMaterial = new Material(settings.TentacleMaterial);
            else
            {
                Shader shader = Shader.Find("ImmuneDefense/NeutrophilTentacle");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                if (shader != null) runtimeMaterial = new Material(shader);
            }
        }
        if (runtimeMaterial != null)
            runtimeMaterial.mainTexture = settings.TentacleTexture != null ? settings.TentacleTexture : Texture2D.whiteTexture;
    }
    private LineRenderer CreateLine()
    {
        EnsureRuntime();
        var go = new GameObject("Tentacle");
        go.layer = gameObject.layer;
        go.transform.SetParent(runtimeRoot.transform, false);
        var line = go.AddComponent<LineRenderer>();
        line.sharedMaterial = runtimeMaterial;
        line.useWorldSpace = true;
        line.alignment = LineAlignment.View;
        line.textureMode = LineTextureMode.Stretch;
        line.numCapVertices = 4;
        line.numCornerVertices = 4;
        line.shadowCastingMode = ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.sortingLayerID = string.IsNullOrEmpty(settings.SortingLayerName)
            ? (bodyRenderer != null ? bodyRenderer.sortingLayerID : 0)
            : SortingLayer.NameToID(settings.SortingLayerName);
        line.sortingOrder = (bodyRenderer != null ? bodyRenderer.sortingOrder : 0) + settings.TentacleSortingOffset;
        line.startColor = settings.RootColor;
        line.endColor = settings.TipColor;
        line.enabled = false;
        return line;
    }
    private void DrawLine(LineRenderer line, Vector3 end)
    {
        if (line == null) return;
        Vector3 start = Origin;
        Vector3 delta = end - start;
        float length = delta.magnitude;
        line.enabled = length > 0.001f;
        if (!line.enabled) return;
        int count = Mathf.Clamp(settings.Segments, 8, 64) + 1;
        if (positions == null || positions.Length != count) positions = new Vector3[count];
        Vector3 side = new Vector3(-delta.y, delta.x, 0f).normalized;
        float scale = TowerScale;
        float amplitude = scale * Mathf.Min(1f, length);
        for (int i = 0; i < count; i++)
        {
            float t = (float)i / (count - 1);
            float bend = Mathf.Sin(Mathf.PI * t) * (settings.CurveAmplitude
                + Mathf.Sin(visualClock * settings.SwaySpeed + t * Mathf.PI * 2f) * settings.SwayAmplitude);
            positions[i] = Vector3.Lerp(start, end, t) + side * (bend * amplitude);
        }
        // Explicit endpoints avoid any floating point sine error at the target.
        positions[0] = start;
        positions[count - 1] = end;
        line.positionCount = count;
        line.startWidth = settings.RootWidth * scale;
        line.endWidth = settings.TipWidth * scale;
        line.SetPositions(positions);
    }
    private int FindActiveLink(EnemyHealth target)
    {
        for (int i = 0; i < activeLinks.Count; i++)
        {
            if (ReferenceEquals(activeLinks[i].Target, target)) return i;
        }
        return -1;
    }
    private void UpdateActiveLinks(float dt)
    {
        float extensionStep = dt / Mathf.Max(.01f, settings.ExtendSeconds);
        for (int i = 0; i < tower.LinkedTargetCount; i++)
        {
            EnemyHealth target = tower.GetLinkedTarget(i);
            if (target == null || !target.IsAlive || !target.isActiveAndEnabled) continue;

            int index = FindActiveLink(target);
            ActiveLink item;
            if (index < 0)
            {
                item = new ActiveLink { Target = target, Line = CreateLine() };
                activeLinks.Add(item);
            }
            else item = activeLinks[index];

            item.Seen = true;
            item.Extension = Mathf.MoveTowards(item.Extension, 1f, extensionStep);
            DrawLine(item.Line, Vector3.Lerp(Origin, target.transform.position, item.Extension));
        }
        // A surviving target keeps its renderer and progress when the list order shifts.
        for (int i = activeLinks.Count - 1; i >= 0; i--)
        {
            ActiveLink item = activeLinks[i];
            if (item.Seen)
            {
                item.Seen = false;
                continue;
            }
            RetractLink(item.Line);
            activeLinks.RemoveAt(i);
        }
    }
    private void RetractLink(LineRenderer line)
    {
        if (line == null) return;
        if (line.enabled && line.positionCount > 0)
            retracting.Add(new RetractingLink { Line = line, End = line.GetPosition(line.positionCount - 1) });
        else Destroy(line.gameObject);
    }
    private void HandleTargetKilled(EnemyHealth target, EnemyVisualSnapshot snapshot)
    {
        if (snapshot.Sprite == null || settings == null || !isActiveAndEnabled) return;
        EnsureRuntime();
        var go = new GameObject("Swallow Ghost");
        go.layer = gameObject.layer;
        go.transform.SetParent(runtimeRoot.transform, false);
        go.transform.SetPositionAndRotation(snapshot.Position, snapshot.Rotation);
        go.transform.localScale = snapshot.Scale;
        var ghost = go.AddComponent<SpriteRenderer>();
        ghost.sprite = snapshot.Sprite;
        ghost.color = new Color(snapshot.Color.r, snapshot.Color.g, snapshot.Color.b,
            snapshot.Color.a * settings.GhostStartAlpha);
        ghost.flipX = snapshot.FlipX;
        ghost.flipY = snapshot.FlipY;
        ghost.drawMode = snapshot.DrawMode;
        ghost.size = snapshot.Size;
        ghost.sharedMaterial = snapshot.Material;
        ghost.sortingLayerID = string.IsNullOrEmpty(settings.SortingLayerName)
            ? snapshot.SortingLayerID : SortingLayer.NameToID(settings.SortingLayerName);
        ghost.sortingOrder = Mathf.Max(snapshot.SortingOrder, bodyRenderer != null ? bodyRenderer.sortingOrder : 0)
            + settings.GhostSortingOffset;
        int index = FindActiveLink(target);
        LineRenderer swallowedLink = index >= 0 ? activeLinks[index].Line : null;
        if (index >= 0) activeLinks.RemoveAt(index);
        if (swallowedLink == null) swallowedLink = CreateLine();
        DrawLine(swallowedLink, snapshot.Position);
        swallows.Add(new Swallow {Ghost = ghost, Line = swallowedLink, Start = snapshot.Position,
            InitialScale = snapshot.Scale, InitialColor = ghost.color});
    }
    private void LateUpdate()
    {
        if (tower == null || !tower.isActiveAndEnabled)
        {
            ClearEffects();
            return;
        }
        GameStateController state = tower.GameState;
        if (settings == null || state == null || !state.CanSimulate) return;
        ApplyBodyArt();
        float dt = Time.deltaTime;
        visualClock += dt;
        if (runtimeMaterial != null)
            runtimeMaterial.mainTexture = settings.TentacleTexture != null ? settings.TentacleTexture : Texture2D.whiteTexture;

        UpdateActiveLinks(dt);
        for (int i = retracting.Count - 1; i >= 0; i--)
        {
            var item = retracting[i];
            item.Elapsed += dt;
            float t = Mathf.Clamp01(item.Elapsed / Mathf.Max(.01f, settings.RetractSeconds));
            DrawLine(item.Line, Vector3.Lerp(item.End, Origin, Mathf.SmoothStep(0f, 1f, t)));
            if (t >= 1f)
            {
                if (item.Line != null) Destroy(item.Line.gameObject);
                retracting.RemoveAt(i);
            }
        }
        for (int i = swallows.Count - 1; i >= 0; i--)
        {
            var item = swallows[i];
            if (item.Ghost == null)
            {
                if (item.Line != null) Destroy(item.Line.gameObject);
                swallows.RemoveAt(i);
                continue;
            }
            item.Elapsed += dt;
            float t = Mathf.Clamp01(item.Elapsed / Mathf.Max(.01f, settings.SwallowSeconds));
            float eased = t * t;
            item.Ghost.transform.position = Vector3.Lerp(item.Start, Origin, eased);
            item.Ghost.transform.localScale = item.InitialScale * Mathf.Lerp(1f, settings.GhostEndScale, eased);
            Color color = item.InitialColor;
            color.a *= 1f - t;
            item.Ghost.color = color;
            DrawLine(item.Line, item.Ghost.transform.position);
            if (t >= 1f)
            {
                Destroy(item.Ghost.gameObject);
                if (item.Line != null) Destroy(item.Line.gameObject);
                swallows.RemoveAt(i);
            }
        }
    }
    private void ClearEffects()
    {
        if (runtimeRoot != null)
        {
            runtimeRoot.SetActive(false);
            Destroy(runtimeRoot);
        }
        if (runtimeMaterial != null) Destroy(runtimeMaterial);
        runtimeRoot = null;
        runtimeMaterial = null;
        activeLinks.Clear();
        swallows.Clear();
        retracting.Clear();
    }
    private void OnDisable()
    {
        if (subscribed && tower != null)
        {
            tower.TargetKilled -= HandleTargetKilled;
        }
        subscribed = false;
        ClearEffects();
        if (ownsSettings && settings != null) Destroy(settings);
        ownsSettings = false;
        settings = null;
    }
}

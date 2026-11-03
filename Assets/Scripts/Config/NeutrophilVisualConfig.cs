using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Neutrophil Visual Config")]
public class NeutrophilVisualConfig : ScriptableObject
{
    [Header("Art (optional: existing cell sprite and a solid tentacle are the fallback)")]
    [SerializeField] private Sprite bodySprite;
    [SerializeField] private Color bodyTint = Color.white;
    [SerializeField] private Texture2D tentacleTexture;
    [SerializeField] private Material tentacleMaterial;
    [Header("Tentacle (widths and offsets follow tower scale)")]
    [SerializeField] private Vector2 originLocalOffset = Vector2.zero;
    [SerializeField, Min(0.001f)] private float rootWidth = 0.16f;
    [SerializeField, Min(0.001f)] private float tipWidth = 0.045f;
    [SerializeField, Range(8, 64)] private int segments = 24;
    [SerializeField, Min(0f)] private float curveAmplitude = 0.14f;
    [SerializeField, Min(0f)] private float swayAmplitude = 0.035f;
    [SerializeField, Min(0f)] private float swaySpeed = 3f;
    [SerializeField, Min(0.01f)] private float extendSeconds = 0.18f;
    [SerializeField, Min(0.01f)] private float retractSeconds = 0.18f;
    [SerializeField] private Color rootColor = new Color(0.47f, 1f, 0.71f, 0.9f);
    [SerializeField] private Color tipColor = new Color(0.31f, 0.86f, 1f, 0.8f);
    [Header("Swallow")]
    [SerializeField, Min(0.01f)] private float swallowSeconds = 0.55f;
    [SerializeField, Range(0f, 1f)] private float ghostEndScale = 0.08f;
    [SerializeField, Range(0f, 1f)] private float ghostStartAlpha = 0.8f;
    [Header("Sorting (empty layer follows cell)")]
    [SerializeField] private string sortingLayerName = "";
    [SerializeField] private int tentacleSortingOffset = -1;
    [SerializeField] private int ghostSortingOffset = 1;

    public Sprite BodySprite => bodySprite;
    public Color BodyTint => bodyTint;
    public Texture2D TentacleTexture => tentacleTexture;
    public Material TentacleMaterial => tentacleMaterial;
    public Vector2 OriginLocalOffset => originLocalOffset;
    public float RootWidth => rootWidth;
    public float TipWidth => tipWidth;
    public int Segments => segments;
    public float CurveAmplitude => curveAmplitude;
    public float SwayAmplitude => swayAmplitude;
    public float SwaySpeed => swaySpeed;
    public float ExtendSeconds => extendSeconds;
    public float RetractSeconds => retractSeconds;
    public Color RootColor => rootColor;
    public Color TipColor => tipColor;
    public float SwallowSeconds => swallowSeconds;
    public float GhostEndScale => ghostEndScale;
    public float GhostStartAlpha => ghostStartAlpha;
    public string SortingLayerName => sortingLayerName;
    public int TentacleSortingOffset => tentacleSortingOffset;
    public int GhostSortingOffset => ghostSortingOffset;
}

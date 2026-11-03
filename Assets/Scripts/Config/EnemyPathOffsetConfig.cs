using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Enemy Path Offset Config")]
public class EnemyPathOffsetConfig : ScriptableObject
{
    [SerializeField] private float maxLateralOffset = 0.5f;
    [SerializeField] private float transitionDistance = 1.0f;
    public float MaxLateralOffset
    {
        get
        {
            return maxLateralOffset;
        }
    }
    public float TransitionDistance
    {
        get
        {
            return transitionDistance;
        }
    }
}

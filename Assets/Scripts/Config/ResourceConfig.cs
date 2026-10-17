using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Resource Config")]
public class ResourceConfig : ScriptableObject
{
    [SerializeField] private int initialImmunePoints = 100;
    public int InitialImmunePoints
    {
        get
        {
            return initialImmunePoints;
        }
    }
}

using UnityEngine;

[System.Serializable]
public class ScenePointBinding
{
    [SerializeField] private string id;
    [SerializeField] private Transform point;
    public string ID
    {
        get
        {
            return id;
        }
    }
    public Transform Point
    {
        get
        {
            return point;
        }
    }
}

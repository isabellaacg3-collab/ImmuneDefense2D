using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Tutorial Card Config")]
public class TutorialCardConfig : ScriptableObject
{
    [SerializeField] string title;
    [TextArea(3, 10)]
    [SerializeField] string body;
    public string Title
    {
        get
        {
            return title;
        }
    }
    public string Body
    {
        get
        {
            return body;
        }
    }
}

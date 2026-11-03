using TMPro;
using UnityEngine;

public class ImmunePointsUI : MonoBehaviour
{
    [SerializeField] ImmunePoints immunePoints;
    [SerializeField] TMP_Text pointsText;
    private void RefreshPoints(int value)
    {
        pointsText.text = "ID点：" + value;
    }
    private void OnEnable()
    {
        immunePoints.PointsChanged += RefreshPoints;
        RefreshPoints(immunePoints.Points);
    }
    private void OnDisable()
    {
        if(immunePoints != null)
        {
            immunePoints.PointsChanged -= RefreshPoints;
        }
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "ImmuneDefense/Pause Policy")]
public class PausePolicyConfig : ScriptableObject
{
    [SerializeField] private bool pauseSimulation = true;
    [SerializeField] private bool blockWorldInput = true;
    public bool PauseSimulation
    {
        get
        {
            return pauseSimulation;
        }
    }
    public bool BlockWorldInput
    {
        get
        {
            return blockWorldInput;
        }
    }
}

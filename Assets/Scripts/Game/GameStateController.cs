using Unity.VisualScripting;
using UnityEngine;
using System;


public enum LevelState
{
    Playing,
    Victory,
    Defeat
}
public class GameStateController : MonoBehaviour
{
    [SerializeField] private HostHealth host;
    [SerializeField] private LevelState state = LevelState.Playing;
    public bool IsPlaying
    {
        get
        {
            return state == LevelState.Playing;
        }
    }
    public LevelState State
    {
        get
        {
            return state;
        }
    }
    public void Win()
    {
        if(state != LevelState.Playing)
        {
            return;
        }
        state = LevelState.Victory;
        StateChanged?.Invoke(State);
    }
    public event Action<LevelState> StateChanged;
    private void HandleHostDied()
    {
        if(state != LevelState.Playing)
        {
            return;
        }
        state = LevelState.Defeat;
        StateChanged?.Invoke(State);
        //Debug.Log("Defeat!");
    }
    private void OnEnable()
    {
        host.Died += HandleHostDied;
    }
    private void OnDisable()
    {
        if(host != null)
        {
            host.Died -= HandleHostDied;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

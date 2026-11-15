using System;
using UnityEngine;

[DisallowMultipleComponent]
public class TowerInstance : MonoBehaviour
{
    [SerializeField] private TowerConfig config;
    [SerializeField] private GameStateController gameState;
    public TowerConfig Config => config;
    public GameStateController GameState => gameState;
    public event Action<TowerConfig> ConfigChanged;
    public void Initialize(GameStateController controller)
    {
        gameState = controller;
    }
    public bool TryApplyUpgrade()
    {
        TowerConfig nextConfig = config != null ? config.UpgradeTarget : null;
        if (nextConfig == null || nextConfig == config)
        {
            return false;
        }
        config = nextConfig;
        ConfigChanged?.Invoke(config);
        return true;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class EnemyFirstAppearanceController : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner; 
    [SerializeField] private KnowledgeCardController knowledgeCardController;
    [SerializeField] private GameStateController gameState;
    private HashSet<EnemyConfig> seenEnemies = new HashSet<EnemyConfig>();
    private Queue<TutorialCardConfig> pendingCards = new Queue<TutorialCardConfig>();
    private void HandleEnemyEnteredCombat(EnemyConfig config)
    {
        if(config == null)
        {
            return;
        }
        if (!seenEnemies.Add(config))
        {
            return;
        }
        Debug.Log($"首次出现：{config.name}");
        if(config.FirstAppearanceCard == null)
        {
            return;
        }
        pendingCards.Enqueue(config.FirstAppearanceCard);
    }
    private void Update()
    {
        if(pendingCards.Count == 0 || !gameState.CanSimulate || knowledgeCardController.IsOpen)
        {
            return;
        }
        TutorialCardConfig card = pendingCards.Dequeue();
        knowledgeCardController.Open(card);
    }
    private void OnEnable()
    {
        enemySpawner.EnemyEnteredCombat += HandleEnemyEnteredCombat;
    }
    private void OnDisable()
    {
        enemySpawner.EnemyEnteredCombat -= HandleEnemyEnteredCombat;
    }
}

using UnityEngine;

static class LevelProgress
{
    private const string CompletedPrefix = "ImmuneDefense.Completed.";
    private const string OutcomePrefix = "ImmuneDefense.LastOutcome.";

    public static bool IsCompleted(string sceneName)
    {
        return PlayerPrefs.GetInt(CompletedPrefix + sceneName, 0) == 1;
    }

    public static LevelState? GetLastOutcome(string sceneName)
    {
        return PlayerPrefs.GetString(OutcomePrefix + sceneName) switch
        {
            nameof(LevelState.Victory) => LevelState.Victory,
            nameof(LevelState.Defeat) => LevelState.Defeat,
            _ => null
        };
    }

    public static void RecordOutcome(string sceneName, LevelState outcome)
    {
        if ((sceneName != "Level01" && sceneName != "Level02") ||
            (outcome != LevelState.Victory && outcome != LevelState.Defeat))
        {
            return;
        }

        if (outcome == LevelState.Victory)
        {
            PlayerPrefs.SetInt(CompletedPrefix + sceneName, 1);
        }

        PlayerPrefs.SetString(OutcomePrefix + sceneName, outcome.ToString());
        PlayerPrefs.Save();
    }
}

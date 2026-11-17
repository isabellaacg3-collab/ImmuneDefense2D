using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [System.Serializable]
    private struct LevelDetails
    {
        public string title;
        public string location;
        [TextArea] public string summary;
        [TextArea] public string biology;
    }

    [SerializeField] private Button level01Button;
    [SerializeField] private Button level02Button;
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private TMP_Text detailTitle;
    [SerializeField] private TMP_Text detailLocation;
    [SerializeField] private TMP_Text detailSummary;
    [SerializeField] private TMP_Text detailBiology;
    [SerializeField] private TMP_Text detailStatus;
    [SerializeField] private Button enterButton;
    [SerializeField] private LevelDetails level01Details;
    [SerializeField] private LevelDetails level02Details;
    private string selectedScene;

    public void OpenLevel(string sceneName)
    {
        if (!CanEnter(sceneName))
        {
            return;
        }

        selectedScene = sceneName;
        LevelDetails details = sceneName == "Level01" ? level01Details : level02Details;
        detailTitle.text = details.title;
        detailLocation.text = "发生部位　" + details.location;
        detailSummary.text = details.summary;
        detailBiology.text = details.biology;

        string status = LevelProgress.IsCompleted(sceneName) ? "已完成" : "已开放";
        detailStatus.text = LevelProgress.GetLastOutcome(sceneName) switch
        {
            LevelState.Victory => status + "　·　上次胜利",
            LevelState.Defeat => status + "　·　上次失败",
            _ => status
        };

        detailPanel.SetActive(true);
        EventSystem.current?.SetSelectedGameObject(enterButton.gameObject);
    }

    public void CloseDetails()
    {
        if (selectedScene == null)
        {
            return;
        }

        Button selectedButton = selectedScene == "Level01" ? level01Button : level02Button;
        selectedScene = null;
        detailPanel.SetActive(false);
        EventSystem.current?.SetSelectedGameObject(selectedButton.gameObject);
    }

    public void EnterSelectedLevel()
    {
        if (!CanEnter(selectedScene))
        {
            return;
        }

        string sceneName = selectedScene;
        selectedScene = null;
        SceneManager.LoadScene(sceneName);
    }

    private static bool CanEnter(string sceneName)
    {
        return sceneName == "Level01" || (sceneName == "Level02" && LevelProgress.IsCompleted("Level01"));
    }

    private static void RefreshButton(Button button, bool unlocked, bool completed)
    {
        button.interactable = unlocked;
        var lockedIcon = button.transform.Find("Locked");
        var availableIcon = button.transform.Find("Available");
        var completedIcon = button.transform.Find("Completed");
        lockedIcon.gameObject.SetActive(!unlocked);
        availableIcon.gameObject.SetActive(unlocked && !completed);
        completedIcon.gameObject.SetActive(unlocked && completed);
        var activeIcon = !unlocked ? lockedIcon : completed ? completedIcon : availableIcon;
        button.targetGraphic = activeIcon.GetComponent<Image>();
    }

    private void Start()
    {
        detailPanel.SetActive(false);
        bool level01Completed = LevelProgress.IsCompleted("Level01");
        RefreshButton(level01Button, true, level01Completed);
        RefreshButton(level02Button, level01Completed, LevelProgress.IsCompleted("Level02"));
    }
}

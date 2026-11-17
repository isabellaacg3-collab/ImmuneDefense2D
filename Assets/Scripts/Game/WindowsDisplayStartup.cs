using UnityEngine;

internal static class WindowsDisplayStartup
{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Apply()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        Application.onBeforeRender += Report;
    }

    private static void Report()
    {
        if (Time.frameCount < 3) return;

        Application.onBeforeRender -= Report;
        Debug.Log($"Windows startup display: {Display.main.renderingWidth}x{Display.main.renderingHeight} ({Screen.fullScreenMode})");
    }
#endif
}

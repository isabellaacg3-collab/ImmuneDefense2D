using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEngine;

public static class WindowsBetaBuild
{
    private const string Version = "0.2.0-beta.1";
    private const string EditorVersion = "6000.3.19f1";
    private const string ProductName = "ImmuneDefense2D";
    private static readonly string[] ReleaseScenes =
    {
        "Assets/Scenes/LevelSelect.unity",
        "Assets/Scenes/Level01.unity",
        "Assets/Scenes/Level02.unity"
    };

    [Serializable]
    private sealed class BuildRecord
    {
        public string version;
        public string sourceCommit;
        public string unityVersion;
        public string result;
        public string outputPath;
        public string target;
        public string backend;
        public string options;
        public string[] scenes;
        public ulong totalSize;
        public double seconds;
        public int errors;
        public int warnings;
    }

    [MenuItem("Release/Build Windows Beta")]
    public static void Run()
    {
        try
        {
            BuildRelease();
            Debug.Log("WINDOWS_BETA_BUILD_SUCCEEDED");
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }
        catch (Exception error)
        {
            Debug.LogException(error);
            if (Application.isBatchMode) EditorApplication.Exit(1);
            else throw;
        }
    }

    private static void BuildRelease()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string output = Path.GetFullPath(Argument("-releaseOutput", Path.Combine(root, "Builds", "Windows", Version)));
        string records = Path.GetFullPath(Argument("-releaseRecordDir", Path.Combine(root, "BuildRecords", Version)));
        string source = Argument("-sourceCommit", "not supplied (menu build)");
        string recordPath = Path.Combine(records, "build-report.json");
        if (Application.unityVersion != EditorVersion || PlayerSettings.bundleVersion != Version ||
            PlayerSettings.productName != ProductName)
        {
            throw new InvalidOperationException("Unexpected Unity version, product name, or release version.");
        }

        var enabledScenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        if (!enabledScenes.SequenceEqual(ReleaseScenes))
        {
            throw new InvalidOperationException("Expected LevelSelect, Level01, and Level02 in this order.");
        }

        if ((Directory.Exists(output) && Directory.EnumerateFileSystemEntries(output).Any()) || File.Exists(recordPath))
        {
            throw new InvalidOperationException("Use an empty build directory and a new build record path.");
        }

        Directory.CreateDirectory(output);
        Directory.CreateDirectory(records);
        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.allowDebugging = false;
        EditorUserBuildSettings.connectProfiler = false;
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = enabledScenes,
            locationPathName = Path.Combine(output, ProductName + ".exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });

        var summary = report.summary;
        var record = new BuildRecord
        {
            version = Version,
            sourceCommit = source,
            unityVersion = Application.unityVersion,
            result = summary.result.ToString(),
            outputPath = summary.outputPath,
            target = summary.platform.ToString(),
            backend = PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone).ToString(),
            options = summary.options.ToString(),
            scenes = enabledScenes,
            totalSize = summary.totalSize,
            seconds = summary.totalTime.TotalSeconds,
            errors = summary.totalErrors,
            warnings = summary.totalWarnings
        };
        File.WriteAllText(recordPath, JsonUtility.ToJson(record, true));
        if (summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException("Windows Beta build failed. See " + recordPath);
        }

        var documentation = Path.Combine(root, "Documentation", "Release", Version);
        foreach (string file in new[] { "README.txt", "RELEASE_NOTES.md" })
        {
            File.Copy(Path.Combine(documentation, file), Path.Combine(output, file));
        }

        File.WriteAllText(Path.Combine(output, "BUILD_INFO.txt"),
            $"Version: {Version}\nSource commit: {source}\nUnity: {Application.unityVersion}\nTarget: Windows x64\nBackend: {record.backend}\n");
        CopyLicenses(root, output);
    }

    private static void CopyLicenses(string root, string output)
    {
        var licenses = Path.Combine(output, "Licenses");
        Directory.CreateDirectory(licenses);
        File.Copy(Path.Combine(root, "Assets", "Fonts", "LICENSE"), Path.Combine(licenses, "Noto-OFL-LICENSE.txt"));
        File.Copy(Path.Combine(root, "Assets", "TextMesh Pro", "Fonts", "LiberationSans - OFL.txt"),
            Path.Combine(licenses, "LiberationSans-OFL.txt"));

        foreach (var package in PackageInfo.GetAllRegisteredPackages())
        {
            if (!Directory.Exists(package.resolvedPath)) continue;
            foreach (string file in Directory.EnumerateFiles(package.resolvedPath))
            {
                string name = Path.GetFileName(file);
                if (name.StartsWith("LICENSE", StringComparison.OrdinalIgnoreCase) ||
                    name.StartsWith("THIRD PARTY NOTICES", StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(file, Path.Combine(licenses, package.name + "-" + name));
                }
            }
        }
    }

    private static string Argument(string name, string fallback)
    {
        var args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : fallback;
    }
}

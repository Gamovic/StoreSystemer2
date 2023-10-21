using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using static UnityEditor.Progress;

public class Builder
{
    private static void BuildEmbeddedLinux()
    {
        // Setup build options (e.g. scenes, build output location)
        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        buildOptions.scenes = new[] { "Assets/Scenes/AmqpDemo.unity" };
        buildOptions.locationPathName = "BuildsByCustomInspector/LinuxServer";
        buildOptions.target = BuildTarget.StandaloneLinux64;
        buildOptions.subtarget = (int)StandaloneBuildSubtarget.Server;

        var report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build successful - Build written to {buildOptions.locationPathName}");
        }
        else if (report.summary.result == BuildResult.Failed)
        {
            Debug.LogError($"Build failed");
        }

    }

    // This function will be called from the build process
    public static void Build()
    {
        BuildEmbeddedLinux();
    }
}

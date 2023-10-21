using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System;

public class CustomBuilderAndDockerButton : MonoBehaviour
{
    static string buildFolderPath = "BuildsByCustomInspector"; // Sti til build-mappen
    static string imageName = "linux_server";
    [MenuItem("Custom/Build new Docker image")]
    public static void BuildAndDocker()
    {
        BuildLinuxServer();
        BuildDockerImage();
    }

    [MenuItem("Custom/Build Docker image and Run")]
    public static void BuildAndDockerAndRun()
    {
        BuildLinuxServer();
        BuildDockerImage();
        RunDockerImage();
    }

    [MenuItem("Custom/Remove All Servers And Start New Server")]
    public static void RemoveAllServersAndStartNewServer()
    {
        RemoveAllServers();
        RunDockerImage();
    }

    [MenuItem("Custom/Remove All Servers")]
    public static void RemoveAllServersShortcut()
    {
        RemoveAllServers();
    }

    public static void BuildLinuxServer()
    {
        if (Directory.Exists(buildFolderPath))
        {
            DirectoryInfo directory = new DirectoryInfo(buildFolderPath);
            foreach (FileInfo file in directory.GetFiles())
                file.Delete();
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                subDirectory.Delete(true);
        }

        // Byg til Linux (headless server)
        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        buildOptions.scenes = new[] { "Assets/Scenes/AmqpDemo.unity" }; // Angiv dine scene stier
        buildOptions.locationPathName = buildFolderPath + "/LinuxServer";
        buildOptions.target = BuildTarget.StandaloneLinux64;
        buildOptions.subtarget = (int)StandaloneBuildSubtarget.Server;
        BuildPipeline.BuildPlayer(buildOptions);
    }
    public static void BuildDockerImage()
    {
        // Udfoer Docker-build
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "docker";
        startInfo.Arguments = $"build -t {imageName} .";
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        UnityEngine.Debug.Log("Docker build output: " + output);
    }

    public static void RunDockerImage()
    {
        // Udfoer Docker-build
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "docker";
        startInfo.Arguments = $"run -d -p 5672:5672 -p 15672:15672 {imageName}";
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        UnityEngine.Debug.Log("Docker build output: " + output);
    }

    public static void RemoveAllServers()
    {
        // Stop all servers...
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "docker";
        startInfo.Arguments = $"ps -a --filter ancestor={imageName}" + " --format \"{{.Names}}\"";
        UnityEngine.Debug.Log(startInfo.Arguments);
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        process.WaitForExit();
        string output = process.StandardOutput.ReadToEnd();
        string[] runningContainers = output.Split(new char[] { '\n', });
        for (int i = 0; i < runningContainers.Length; i++)
        {
            ProcessStartInfo startInfo3 = new ProcessStartInfo();
            startInfo3.FileName = "docker";
            startInfo3.Arguments = $"rm -f {runningContainers[i]}";
            startInfo3.RedirectStandardOutput = true;
            startInfo3.UseShellExecute = false;
            startInfo3.CreateNoWindow = true;

            Process process3 = new Process();
            process3.StartInfo = startInfo3;
            process3.Start();
            string output3 = process3.StandardOutput.ReadToEnd();  // Fixed: Use process3 here
            process3.WaitForExit();
        }
    }

    [MenuItem("Custom/Enter Play Mode with last build server", priority = 0)]
    static void EnterPlayMode()
    {
        if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.isPlaying = true;
            RemoveAllServersAndStartNewServer();
        }
    }

    [MenuItem("Custom/Enter Play Mode with Rebuilt new server", priority = 0)]
    static void EnterPlayModeWithRebuilt()
    {
        if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.isPlaying = true;
            RemoveAllServers();
            BuildAndDockerAndRun();
        }
    }
}

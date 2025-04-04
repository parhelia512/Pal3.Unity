﻿// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2025, Jiaqi (0x7c13) Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using Pal3.Game.Constants;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    [Flags]
    public enum Pal3BuildTarget
    {
        Windows_arm64    = 1 << 0,
        Windows_x86      = 1 << 1,
        Windows_x64      = 1 << 2,
        Linux_x86_x64    = 1 << 3,
        macOS_arm64_x64  = 1 << 4,
        Android          = 1 << 5,
        iOS              = 1 << 6,
    }

    public static class ReleaseBuildPipeline
    {
        private static readonly string[] BuildLevels = { "Assets/Scenes/Game.unity" };

        private static readonly char DirSeparator = Path.DirectorySeparatorChar;
        
        #if PAL3
        [MenuItem("PAL3/Build Pipelines/Build [Windows_arm64] IL2CPP Release Executable")]
        #elif PAL3A
        [MenuItem("PAL3A/Build Pipelines/Build [Windows_arm64] IL2CPP Release Executable")]
        #endif
        public static void Build_Windows_arm64()
        {
            BuildGame(Pal3BuildTarget.Windows_arm64);
        }
        
        #if PAL3
        [MenuItem("PAL3/Build Pipelines/Build [Windows_x86] IL2CPP Release Executable")]
        #elif PAL3A
        [MenuItem("PAL3A/Build Pipelines/Build [Windows_x86] IL2CPP Release Executable")]
        #endif
        public static void Build_Windows_x86()
        {
            BuildGame(Pal3BuildTarget.Windows_x86);
        }

        #if PAL3
        [MenuItem("PAL3/Build Pipelines/Build [Windows_x64] IL2CPP Release Executable")]
        #elif PAL3A
        [MenuItem("PAL3A/Build Pipelines/Build [Windows_x64] IL2CPP Release Executable")]
        #endif
        public static void Build_Windows_x64()
        {
            BuildGame(Pal3BuildTarget.Windows_x64);
        }

        #if PAL3
        [MenuItem("PAL3/Build Pipelines/Build [Linux_x86_x64] IL2CPP Release Executable")]
        #elif PAL3A
        [MenuItem("PAL3A/Build Pipelines/Build [Linux_x86_x64] IL2CPP Release Executable")]
        #endif
        public static void Build_Linux_x86_x64()
        {
            BuildGame(Pal3BuildTarget.Linux_x86_x64);
        }

        #if PAL3
        [MenuItem("PAL3/Build Pipelines/Build [macOS_arm64_x64] IL2CPP XCode Project")]
        #elif PAL3A
        [MenuItem("PAL3A/Build Pipelines/Build [macOS_arm64_x64] IL2CPP XCode Project")]
        #endif
        public static void Build_macOS_arm64_x64()
        {
            BuildGame(Pal3BuildTarget.macOS_arm64_x64);
        }

        #if PAL3
        [MenuItem("PAL3/Build Pipelines/Build [Android] IL2CPP Release APK")]
        #elif PAL3A
        [MenuItem("PAL3A/Build Pipelines/Build [Android] IL2CPP Release APK")]
        #endif
        public static void Build_Android()
        {
            BuildGame(Pal3BuildTarget.Android);
        }

        #if PAL3
        [MenuItem("PAL3/Build Pipelines/Build [iOS] IL2CPP XCode Project")]
        #elif PAL3A
        [MenuItem("PAL3A/Build Pipelines/Build [iOS] IL2CPP XCode Project")]
        #endif
        public static void Build_iOS()
        {
            BuildGame(Pal3BuildTarget.iOS);
        }

        #if PAL3
        [MenuItem("PAL3/Build Pipelines/Build All [Windows, Linux, macOS, Android, iOS]")]
        #elif PAL3A
        [MenuItem("PAL3A/Build Pipelines/Build All [Windows, Linux, macOS, Android, iOS]")]
        #endif
        public static void BuildAll()
        {
            BuildGame(Pal3BuildTarget.Windows_arm64 |
                      Pal3BuildTarget.Windows_x86 |
                      Pal3BuildTarget.Windows_x64 |
                      Pal3BuildTarget.Linux_x86_x64 |
                      Pal3BuildTarget.macOS_arm64_x64 |
                      Pal3BuildTarget.Android |
                      Pal3BuildTarget.iOS);
        }

        #if PAL3
        [MenuItem("PAL3/Build Pipelines/Zip Release files [Windows, Linux, Android]")]
        #elif PAL3A
        [MenuItem("PAL3A/Build Pipelines/Zip Release files [Windows, Linux, Android]")]
        #endif
        public static void ZipAll()
        {
            ZipReleaseFiles();
        }

        private static void BuildGame(Pal3BuildTarget buildTarget)
        {
            BuildTargetGroup targetGroupBeforeBuild = EditorUserBuildSettings.selectedBuildTargetGroup;
            BuildTarget targetBeforeBuild = EditorUserBuildSettings.activeBuildTarget;

            string buildOutputPath = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
            if (string.IsNullOrWhiteSpace(buildOutputPath)) return;

            buildOutputPath += $"{DirSeparator}{PlayerSettings.bundleVersion}{DirSeparator}";

            var buildConfigurations = new[]
            {
                new { Target = Pal3BuildTarget.Windows_x86, Extension = ".exe", NamedTarget = NamedBuildTarget.Standalone },
                new { Target = Pal3BuildTarget.Windows_x64, Extension = ".exe", NamedTarget = NamedBuildTarget.Standalone },
                new { Target = Pal3BuildTarget.Windows_arm64, Extension = ".exe", NamedTarget = NamedBuildTarget.Standalone },
                new { Target = Pal3BuildTarget.Linux_x86_x64, Extension = "", NamedTarget = NamedBuildTarget.Standalone},
                new { Target = Pal3BuildTarget.macOS_arm64_x64, Extension = "", NamedTarget = NamedBuildTarget.Standalone },
                new { Target = Pal3BuildTarget.Android, Extension = ".apk", NamedTarget = NamedBuildTarget.Android },
                new { Target = Pal3BuildTarget.iOS, Extension = "", NamedTarget = NamedBuildTarget.iOS },
            };

            List<Action> logActions = new();

            foreach (var config in buildConfigurations)
            {
                if (buildTarget.HasFlag(config.Target))
                {
                    Build(config.Target,
                        config.Extension,
                        config.NamedTarget,
                        buildOutputPath,
                        logActions);
                }
            }
            
            // Execute report log actions
            logActions.ForEach(action => action.Invoke());

            Debug.Log($"[{nameof(ReleaseBuildPipeline)}] Build for version {PlayerSettings.bundleVersion} complete! Output path: " +
                      $"{buildOutputPath}{GameConstants.AppName}");
            
            // Restore build target settings
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(targetGroupBeforeBuild, targetBeforeBuild);
        }

        private static void Build(Pal3BuildTarget target,
            string extension,
            NamedBuildTarget namedBuildTarget,
            string buildOutputPath,
            List<Action> logActions,
            bool deletePdbFiles = true)
        {
            string outputFolder = buildOutputPath + $"{GameConstants.AppName}{DirSeparator}" +
                                $"{target.ToString()}{DirSeparator}";

            BuildTarget buildTarget = BuildTarget.NoTarget;
            
            switch (target)
            {
                case Pal3BuildTarget.Windows_arm64:
                    buildTarget = BuildTarget.StandaloneWindows64;
                    outputFolder += $"{GameConstants.AppName}{DirSeparator}";
                    #if UNITY_2020_2_OR_NEWER && UNITY_EDITOR_WIN
                    UnityEditor.WindowsStandalone.UserBuildSettings.architecture = OSArchitecture.ARM64;
                    #endif
                    break;
                case Pal3BuildTarget.Windows_x86:
                    buildTarget = BuildTarget.StandaloneWindows;
                    outputFolder += $"{GameConstants.AppName}{DirSeparator}";
                    #if UNITY_2020_2_OR_NEWER && UNITY_EDITOR_WIN
                    UnityEditor.WindowsStandalone.UserBuildSettings.architecture = OSArchitecture.x86;
                    #endif
                    break;
                case Pal3BuildTarget.Windows_x64:
                    buildTarget = BuildTarget.StandaloneWindows64;
                    outputFolder += $"{GameConstants.AppName}{DirSeparator}";
                    #if UNITY_2020_2_OR_NEWER && UNITY_EDITOR_WIN
                    UnityEditor.WindowsStandalone.UserBuildSettings.architecture = OSArchitecture.x64;
                    #endif
                    break;
                case Pal3BuildTarget.Linux_x86_x64:
                    buildTarget = BuildTarget.StandaloneLinux64;
                    outputFolder += $"{GameConstants.AppName}{DirSeparator}";
                    break;
                case Pal3BuildTarget.macOS_arm64_x64:
                    buildTarget = BuildTarget.StandaloneOSX;
                    #if UNITY_2020_2_OR_NEWER && UNITY_EDITOR_OSX
                    UnityEditor.OSXStandalone.UserBuildSettings.architecture = OSArchitecture.x64ARM64;
                    UnityEditor.OSXStandalone.UserBuildSettings.createXcodeProject = true;
                    #endif
                    break;
                case Pal3BuildTarget.Android:
                    buildTarget = BuildTarget.Android;
                    break;
                case Pal3BuildTarget.iOS:
                    buildTarget = BuildTarget.iOS;
                    break;
            }

            string outputPath = outputFolder + $"{GameConstants.AppName}{extension}";

            PlayerSettings.SetScriptingBackend(namedBuildTarget, ScriptingImplementation.IL2CPP);

            BuildReport report = BuildPipeline.BuildPlayer(BuildLevels, outputPath, buildTarget, BuildOptions.CleanBuildCache);

            if (deletePdbFiles)
            {
                FileUtil.DeleteFileOrDirectory(outputFolder +
                                               $"{GameConstants.AppName}_BackUpThisFolder_ButDontShipItWithYourGame");
            }

            WriteBuildReport(report, logActions);
        }

        private static void WriteBuildReport(BuildReport report, List<Action> logActions)
        {
            switch (report.summary.result)
            {
                case BuildResult.Succeeded:
                    string successReport = $"[{nameof(ReleaseBuildPipeline)}] Build [{report.summary.platform}] succeeded. " +
                                           $"Finished in {report.summary.totalTime.TotalMinutes:F2} minutes. " +
                                           $"Build size: {(report.summary.totalSize / 1024f / 1024f):F2} MB";
                    logActions.Add(() => Debug.Log(successReport));
                    break;
                case BuildResult.Failed:
                    string errorReport = $"Build [{report.summary.platform}] failed.";
                    logActions.Add(() => Debug.LogError(errorReport));
                    break;
                case BuildResult.Cancelled:
                    string cancelReport = $"Build [{report.summary.platform}] cancelled.";
                    logActions.Add(() => Debug.LogWarning(cancelReport));
                    break;
                case BuildResult.Unknown:
                    string unknownReport = $"Build [{report.summary.platform}] skipped or not completed.";
                    logActions.Add(() => Debug.LogWarning(unknownReport));
                    break;
            }
        }

        private static void ZipReleaseFiles()
        {
            string buildOutputPath = EditorUtility.SaveFolderPanel("Choose Location of Built Game",
                $"{PlayerSettings.bundleVersion}", $"{PlayerSettings.bundleVersion}");
            if (string.IsNullOrWhiteSpace(buildOutputPath)) return;

            if (!buildOutputPath.EndsWith($"{PlayerSettings.bundleVersion}"))
            {
                Debug.LogError($"Invalid build output path: {buildOutputPath}. " +
                               $"Please choose the current release build folder \"{PlayerSettings.bundleVersion}\"");
                return;
            }

            string releaseDirPath = $"{buildOutputPath}{DirSeparator}Release";
            Directory.CreateDirectory(releaseDirPath);

            List<(string FolderPath, Pal3BuildTarget Target)> buildTargets = new()
            {
                ($"{GameConstants.AppName}{DirSeparator}{Pal3BuildTarget.Android.ToString()}{DirSeparator}{GameConstants.AppName}.apk", Pal3BuildTarget.Android),
                ($"{GameConstants.AppName}{DirSeparator}{Pal3BuildTarget.Windows_arm64.ToString()}", Pal3BuildTarget.Windows_arm64),
                ($"{GameConstants.AppName}{DirSeparator}{Pal3BuildTarget.Windows_x86.ToString()}", Pal3BuildTarget.Windows_x86),
                ($"{GameConstants.AppName}{DirSeparator}{Pal3BuildTarget.Windows_x64.ToString()}", Pal3BuildTarget.Windows_x64),
                ($"{GameConstants.AppName}{DirSeparator}{Pal3BuildTarget.Linux_x86_x64.ToString()}", Pal3BuildTarget.Linux_x86_x64)
            };

            foreach ((string folderPath, Pal3BuildTarget target) in buildTargets)
            {
                string fullPath = $"{buildOutputPath}{DirSeparator}{folderPath}";
                string zipFileName = $"{GameConstants.AppName}_v{PlayerSettings.bundleVersion}_{target.ToString()}.zip";
                string zipFilePath = $"{releaseDirPath}{DirSeparator}{zipFileName}";

                if (File.Exists(fullPath) || Directory.Exists(fullPath))
                {
                    File.Delete(zipFilePath);

                    if (target == Pal3BuildTarget.Android)
                    {
                        using FileStream fileStream = new(zipFilePath, FileMode.Create);
                        using ZipArchive archive = new(fileStream, ZipArchiveMode.Create);
                        archive.CreateEntryFromFile(fullPath, $"{GameConstants.AppName}.apk");
                    }
                    else
                    {
                        ZipFile.CreateFromDirectory(fullPath, zipFilePath);
                    }

                    Debug.Log($"{target.ToString()} build zipped to {zipFilePath}");
                }
                else
                {
                    Debug.LogWarning($"{target.ToString()} release folder not found at {fullPath}. Skipped.");
                }
            }
        }
    }
}
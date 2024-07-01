#if UNITY_EDITOR
using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace xyz.yewnyx.build
{
    public static class BuildScript {
        [PublicAPI]
        public static void SwitchProfile() {
            var currentProfile = BuildProfile.GetActiveBuildProfile();
            Debug.Log("Active Build Profile: " + currentProfile.name);
            EditorApplication.Exit(0);
        }
        
        [PublicAPI]
        public static void Build() {
            var currentProfile = BuildProfile.GetActiveBuildProfile();
            Debug.Log("Active Build Profile: " + currentProfile.name);
            
            var args = Environment.GetCommandLineArgs().ToList();
            var outputPathIndex = args.IndexOf("-outputPath");
            if (outputPathIndex == -1) { throw new Exception("-outputPath not found in command line arguments"); }
            var outputPath = args[outputPathIndex + 1];

            var bpwpo = new BuildPlayerWithProfileOptions {
                locationPathName = outputPath,
                // Rely on build script to launch this with correct build profile
                buildProfile = currentProfile
            };

            var report = BuildPipeline.BuildPlayer(bpwpo);
            var summary = report.summary;

            switch (summary.result) {
                case BuildResult.Succeeded:
                    Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                    break;
                case BuildResult.Failed:
                    Debug.Log("Build failed");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            EditorApplication.Exit(0);
        }
    }
}
#endif
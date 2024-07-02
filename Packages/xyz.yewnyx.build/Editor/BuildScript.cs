#if UNITY_EDITOR
using System;
using System.Linq;
using System.Text;
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

            var branchNameIndex = args.IndexOf("-branch");
            var commitIndex = args.IndexOf("-commit");

            var branchName = string.Empty;
            if (branchNameIndex != -1) { branchName = args[branchNameIndex + 1]; }

            var commit = string.Empty;
            if (commitIndex != -1) { commit = args[commitIndex + 1]; }

            var cleanVersionString = PlayerSettings.bundleVersion;
            var versionString = $"{cleanVersionString}-{branchName}-{commit}".Trim('-');
            PlayerSettings.bundleVersion = versionString;

            try {
                var bpwpo = new BuildPlayerWithProfileOptions
                {
                    locationPathName = outputPath,
                    // Rely on build script to launch this with correct build profile
                    buildProfile = currentProfile
                };

                var report = BuildPipeline.BuildPlayer(bpwpo);
                var summary = report.summary;

                switch (summary.result)
                {
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
            } finally {
                PlayerSettings.bundleVersion = versionString;
            }
        }
    }
}
#endif
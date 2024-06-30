#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace xyz.yewnyx.build
{
    public static class BuildScript {
        [MenuItem("yewnyx/Build/Find Build Profiles")]
        public static void FindBuildProfiles() {
            var profiles = AssetDatabase.FindAssets("t:UnityEditor.Build.Profile.BuildProfile", new []{"Assets/Settings/Build Profiles"});
            foreach (var profile in profiles) {
                Debug.Log(AssetDatabase.GUIDToAssetPath(profile));
            }
        }
        
        
        public static void SwitchProfile() {
            // Do stuff...?
            EditorApplication.Exit(0);
        }
        
        public static void Build() {
            // get output path from environment args
            var args = Environment.GetCommandLineArgs().ToList();
            var outputPathIndex = args.IndexOf("-outputPath");
            if (outputPathIndex == -1) { throw new Exception("-outputPath not found in command line arguments"); }
            var outputPath = args[outputPathIndex + 1];
            
            var buildPlayerOptions = new BuildPlayerOptions {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup,
                target = BuildTarget.StandaloneLinux64,
                subtarget = (int)StandaloneBuildSubtarget.Server,
                // TODO: Development builds
                options = BuildOptions.None,
                locationPathName = outputPath,
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
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
        }
    }
}
#endif
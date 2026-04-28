using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// build by script
/// https://docs.unity3d.com/ScriptReference/BuildPipeline.BuildPlayer.html
/// 
/// menu item shortcuts
/// https://docs.unity3d.com/ScriptReference/MenuItem.html
/// 
/// clear console
/// https://answers.unity.com/questions/707636/clear-console-window.html
/// 
/// what is supposed to be version number on ios
/// https://stackoverflow.com/questions/21125159/which-ios-app-version-build-numbers-must-be-incremented-upon-app-store-release/38009895#38009895
/// 
/// buils size
/// 
/// https://stackoverflow.com/questions/28100362/how-to-reduce-the-size-of-an-apk-file-in-unity
/// https://docs.unity3d.com/Manual/iphone-playerSizeOptimization.html
/// 
/// The pair(Version, Build number) must be unique.
/// The sequence is valid: (1.0.1, 12) -> (1.0.1, 13) -> (1.0.2, 13) -> (1.0.2, 14) ...
/// Version(CFBundleShortVersionString) must be in ascending sequential order.
/// Build number(CFBundleVersion) must be in ascending sequential order.
/// 
/// Based on the checklist, the following (Version, Build Number) sequence is valid too.
/// Case: reuse Build Number in different release trains.
/// (1.0.0, 1) -> (1.0.0, 2) -> ... -> (1.0.0, 11) -> (1.0.1, 1) -> (1.0.1, 2)
/// 
/// </summary>

namespace fwp.buildor.editor
{
    using UnityEditor;

    /// <summary>
    /// regroup all parameters used during build process
    /// encapsulate the build process
    /// </summary>
    public class BuildExecutor
    {
        public const string pref_prefix = "buildor_";

        static public readonly string pref_uid = pref_prefix + "." + Application.productName + "." + Application.productName;

        static public DataBuildSettingProfile Profile => BuildorVars.Profile;
        static public string Platform => Profile?.getPlatformUid();

        BuildPreprocess build_prepro;
        BuildPostprocess build_postpro;

        double time = 0f;

        public BuildExecutor()
        {
            build_prepro = new();
            build_postpro = new();
        }

        public void launch()
        {
            time = EditorApplication.timeSinceStartup;

            build_prepro.onBuildStart += () =>
            {
                log("build..");
                UnityEditor.EditorUtility.DisplayProgressBar("build " + Profile.FullPath, "build...", 0.25f);
            };

            build_prepro.onBuildEnd += (summary) =>
            {
                UnityEditor.EditorUtility.DisplayProgressBar("build " + Profile.FullPath, "postprocess...", 0.75f);
                log("build.post..");
                build_postpro.doLaunch(Profile, summary);

                build_postpro.onPostEnded += () =>
                {
                    log("build.end");
                    EditorUtility.ClearProgressBar();
                };
            };

            UnityEditor.EditorUtility.DisplayProgressBar("build " + Profile.FullPath, "preprocess...", 0f);
            log("build.pre..");
            build_prepro.doLaunch(Profile);
        }

        void log(string msg)
        {
            var dt = EditorApplication.timeSinceStartup - time;
            Debug.Log("[dt:" + (int)dt + "] <<< <color=#FF00FF><b>" + msg + "</b></color> >>>");
        }


    }

}

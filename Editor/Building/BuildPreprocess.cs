using UnityEngine;

using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;

namespace fwp.buildor.editor
{

    public class BuildPreprocess : BuildProcess
    {
        public BuildPlayerOptions buildPlayerOptions;
        public bool incVersion;

        public bool autorun;

        public BuildSummary? build_summary;

        public Action<BuildSummary> onBuilt;

        public BuildProcess doLaunch(DataBuildSettingProfile profil)
        {
            if (profil == null)
            {
                Debug.LogError("no profil ? can't launch");
                return this;
            }

            return launch(profil);
        }

        IEnumerator _pre;
        IEnumerator _wait;
        IEnumerator _build;

        protected override IEnumerator exec()
        {
            _pre = execPre();
            while (_pre.MoveNext()) yield return null;

            _wait = execWait();
            while (_wait.MoveNext()) yield return null;

            _build = execBuild();
            while (_build.MoveNext()) yield return null;
        }

        IEnumerator execPre()
        {
            buildPlayerOptions = new BuildPlayerOptions();

            if (autorun)
            {
                log("+autorun");
                buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;
            }

            //this will apply
            if (incVersion)
            {
                log("+inc.version");
                profil.versionInternal?.incrementFix();
                profil.versionPublish?.incrementFix();
            }

            profil.versionInternal?.event_build();
            profil.versionPublish?.event_build();

            // scenes
            profil.build.merger?.apply();

            // logs
            profil.GetLogsLevels(BuildorVars.TargetDebug)?.applyLogs();

            // apply PlayerSettings vars
            profil.injectProfilToEditor();

            yield return null;

            //buildPlayerOptions.scenes = new[] { "Assets/Scene1.unity", "Assets/Scene2.unity" };
            buildPlayerOptions.scenes = getBuildSettingsScenePaths();

            yield return null;

            // === CREATE SOLVED BUILD PATH

            if (!Directory.Exists(profil.BuildPath))
            {
                Directory.CreateDirectory(profil.BuildPath);
                log("folder: " + profil.BuildPath);
            }

            yield return null;

            // === INJECTING SOLVED PATH TO BUILD SETTINGS

            //[project]_[version].[ext]
            buildPlayerOptions.locationPathName = profil.FullPath;
            log("build.location: " + buildPlayerOptions.locationPathName);

            yield return null;

            //will setup android or ios based on unity build settings target platform
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;

            yield return null;

            if (profil.debug != null)
            {
                if (profil.debug.developement_build) buildPlayerOptions.options |= BuildOptions.Development;
                if (profil.debug.debugScripting) buildPlayerOptions.options |= BuildOptions.AllowDebugging;
            }

            //BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        IEnumerator execWait()
        {
            //wait N secondes
            float startupTime = Time.realtimeSinceStartup;
            float curTime = Time.realtimeSinceStartup;

            float waitTime = 2f;
            log("BuildHelper, waiting " + waitTime + " secondes ...");
            while (curTime - startupTime < waitTime)
            {
                curTime = Time.realtimeSinceStartup;

                //Debug.Log(curTime);
                yield return null;
            }

        }

        IEnumerator execBuild()
        {
            log("buildProcess");

            //curTime = Time.realtimeSinceStartup;
            //Debug.Log(curTime);

            log("BuildHelper, building @ " + buildPlayerOptions.locationPathName);

            yield return null;

            build_app();
        }

        void build_app()
        {
            // https://docs.unity3d.com/ScriptReference/Build.Reporting.BuildSummary.html

            if (buildPlayerOptions.options.HasFlag(BuildOptions.AutoRunPlayer))
            {
                log("+AUTORUN");
            }

            log("options: " + buildPlayerOptions.options);
            log("locationPathName: " + buildPlayerOptions.locationPathName);

            //      BUILD

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            build_summary = report.summary;

            if (onBuilt == null) log("-no reaction-");
            else
            {
                onBuilt?.Invoke(build_summary.Value);
            }
        }

        /// <summary>
        /// return scene.path[] contained in build settings
        /// </summary>
        static protected string[] getBuildSettingsScenePaths()
        {
            List<string> sceneNames = new();

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                sceneNames.Add(scenes[i].path);
                //Debug.Log("  --> " + i + " , adding " + scenes[i].path);
            }

            return sceneNames.ToArray();
        }

        static public void applyIcons(DataBuildSettingProfile profil)
        {

            Texture2D[] icons = new Texture2D[1];
            icons[0] = profil.build.icon;

            PlayerSettings.SetIcons(NamedBuildTarget.Unknown, icons, IconKind.Any);

            Debug.Log(" L icons updated");
        }

        static public void applyCompagny(DataBuildSettingProfile profil, bool verbose = false)
        {

            //profil.applyVersionToEditor(); // apply version

            PlayerSettings.companyName = profil.compagny_name;

            if (verbose) Debug.Log("companyName : " + PlayerSettings.companyName);

            //α,β,Ω
            string productName = profil.getProductName();

            PlayerSettings.productName = productName;
            if (verbose) Debug.Log("productName : " + PlayerSettings.productName);

        }

        static public void applyProfilToUnity(DataBuildSettingProfile profil)
        {

            PlayerSettings.SplashScreen.show = false;
            Debug.Log(" L splash show (auto false under licence) : " + PlayerSettings.SplashScreen.show);

            //dev build
            if (profil.debug != null)
            {
                EditorUserBuildSettings.development = profil.debug.developement_build;
                Debug.Log(" L dev build : " + EditorUserBuildSettings.development);
            }
        }

    }

}
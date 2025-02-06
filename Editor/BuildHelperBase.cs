using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using fwp.buildor.version;

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

    using UnityEditor.Build.Reporting;
    using UnityEditor;

    public class BuildHelperBase
    {
        BuildPlayerOptions buildPlayerOptions;

        IEnumerator preProc = null;
        IEnumerator buildProc = null;

        //float time_at_process = 0f;

        DataBuildSettingsBridge data = null;
        
        BuildParameters _parameters;

        public class BuildParameters
        {
            public BuildPathFlags pathFlags = new BuildPathFlags();
            public BuildHelperFlags buildFlags = new BuildHelperFlags();
            public DataBuildSettingProfile activeProfil;
            public DataBuildorScenesMerger mergerOverride;
        }

        public BuildHelperBase(BuildParameters param) => launch(param);
        //public BuildHelperBase(BuildHelperFlags build, BuildPathFlags path) => launch(build, path);

        void launch(BuildParameters param)
        {
            _parameters = param;

            data = getScriptableDataBuildSettings();

            //if (data != null) applySettings(data.activeProfile);

            Debug.Log(" <<< <b>starting build process</b> >>>");

            preProc = preBuildProcess();

            EditorApplication.update += update_check_process;
        }

        /// <summary>
        /// update in editor
        /// </summary>
        void update_check_process()
        {
            //Debug.Log("it " + Time.realtimeSinceStartup);

            if (preProc != null)
            {
                if (!preProc.MoveNext())
                {
                    preProc = null;

                    Debug.Log("pre proc done");

                    buildProc = buildProcess();
                }
                return;
            }

            if (buildProc != null)
            {
                if (!buildProc.MoveNext())
                {
                    Debug.Log("build proc done");

                    buildProc = null;

                    processEnded();
                }
                return;
            }

        }

        void processEnded()
        {
            EditorApplication.update -= update_check_process;
        }

        /// <summary>
        /// whatever is needed to do before building
        /// </summary>
        /// <returns></returns>
        virtual protected IEnumerator preBuildProcess()
        {
            yield return null;
            //...
        }

        protected IEnumerator buildProcess()
        {
            Debug.Log("BuildHelper, prep building ...");
            build_prep();

            //wait 5 secondes
            float startupTime = Time.realtimeSinceStartup;
            float curTime = Time.realtimeSinceStartup;
            int psec = Mathf.FloorToInt(startupTime);

            float waitTime = 2f;
            Debug.Log("BuildHelper, waiting " + waitTime + " secondes ...");

            while (curTime - startupTime < waitTime)
            {
                curTime = Time.realtimeSinceStartup;

                //Debug.Log(curTime);
                yield return null;
            }

            //curTime = Time.realtimeSinceStartup;
            //Debug.Log(curTime);

            Debug.Log("BuildHelper, building @ " + buildPlayerOptions.locationPathName);

            yield return null;

            build_app();

        }

        protected void build_prep()
        {

            if (BuildPipeline.isBuildingPlayer) return;

            Debug.Log("now building app ; inc version ? " + _parameters.buildFlags.incVersion);

            buildPlayerOptions = new BuildPlayerOptions();

            DataBuildSettingProfile profile = data.getPlatformProfil();

            if (profile == null)
            {
                Debug.LogError("no profile for current platform ?");
                return;
            }

            //DataBuildSettingProfileAndroid pAndroid = profile as DataBuildSettingProfileAndroid;
            //DataBuildSettingProfileIos pIos = profile as DataBuildSettingProfileIos;
            //DataBuildSettingProfileWindows pWindows = profile as DataBuildSettingProfileWindows;
            //DataBuildSettingProfileSwitch pSwitch = profile as DataBuildSettingProfileSwitch;

            //this will apply
            if (_parameters.buildFlags.incVersion)
            {
                if (_parameters.buildFlags.isPublishingBuild)
                    VersionIncrementor.incPublishFix();
                else
                    VersionIncrementor.incInternalFix();
            }

            //apply everything (after inc)
            profile.applyProfilEditor(_parameters.buildFlags.isPublishingBuild);

            //buildPlayerOptions.scenes = new[] { "Assets/Scene1.unity", "Assets/Scene2.unity" };
            buildPlayerOptions.scenes = getScenePaths();

            // === CREATE SOLVED BUILD PATH

            string absPath = profile.getAbsoluteBuildFolderPath(_parameters.pathFlags);

            bool pathExists = Directory.Exists(absPath);

            if (!pathExists)
            {
                Directory.CreateDirectory(absPath);
                Debug.Log("  ... created : " + absPath);
            }

            // === INJECTING SOLVED PATH TO BUILD SETTINGS

            //[project]_[version].[ext]
            absPath = Path.Combine(absPath, profile.getAppName());

            Debug.Log("BuildHelper, saving build at : " + absPath);
            buildPlayerOptions.locationPathName = absPath;

            //will setup android or ios based on unity build settings target platform
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;

            if (profile.developement_build) buildPlayerOptions.options |= BuildOptions.Development;
            if (profile.debugScripting)
            {
                buildPlayerOptions.options |= BuildOptions.AllowDebugging;
            }

            if (_parameters.buildFlags.autorun)
            {
                buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;
            }

            //BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        protected void build_app()
        {
            // https://docs.unity3d.com/ScriptReference/Build.Reporting.BuildSummary.html

            DataBuildSettingProfile profile = data.getPlatformProfil();

            if (buildPlayerOptions.options.HasFlag(BuildOptions.AutoRunPlayer))
            {
                Debug.Log("FLAGS = autorun");
            }

            Debug.Log("build_app() options : " + buildPlayerOptions.options);
            Debug.Log("build_app() @ " + buildPlayerOptions.locationPathName);

            // BUILD
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            // path/to/build/build.exe
            string buildFolderPath = summary.outputPath;

            // remove file.exe at the end
            //buildFolderPath = buildFolderPath.Replace('\\', '/');
            //buildFolderPath = buildFolderPath.Substring(0, buildFolderPath.LastIndexOf('/'));

            switch (summary.result)
            {
                case BuildResult.Succeeded:

                    onSuccess(summary);

                    if (_parameters.buildFlags.openFolderOnSuccess)
                    {
                        Debug.Log($"OPEN FOLDER @{summary.outputPath}");
                        openBuildFolder(summary.outputPath);
                    }

                    if (_parameters.buildFlags.zipOnSuccess)
                    {
                        string zipName = profile.getZipName(_parameters.pathFlags);
                        Debug.Log($"ZIP {summary.outputPath}@{zipName}");
                        zipBuildFolder(summary.outputPath, zipName);
                    }

                    if (_parameters.buildFlags.autorun)
                    {
                        Debug.Log("AUTORUN");
                        execAtPath(summary.outputPath);
                    }
                    break;

                case BuildResult.Failed:
                case BuildResult.Cancelled:
                case BuildResult.Unknown:
                default:
                    Debug.LogError($"BuildResult : Helper Build : {summary.result}");
                    Debug.Log("options : " + summary.options);
                    Debug.Log("output path : " + summary.outputPath);
                    break;
            }
        }

        protected void onSuccess(BuildSummary summary)
        {

            bool success = summary.totalErrors <= 0;

            Debug.Log("Build finished");
            Debug.Log("  L version : <b>" + VersionManager.getFormatedVersion() + "</b>");
            Debug.Log("  L result : summary says " + summary.result + " ( success ? " + success + " ) | warnings : " + summary.totalWarnings + " | errors " + summary.totalErrors);
            Debug.Log("  L symbols : " + ScriptableSymbolHelper.getGroupSymbols(summary.platformGroup));
            Debug.Log("  L platform : <b>" + summary.platform + "</b>");
            Debug.Log("  L build time : " + summary.totalTime);

            switch (summary.result)
            {
                case BuildResult.Succeeded:

                    ulong bytes = summary.totalSize;
                    ulong byteToMo = 1048576;

                    int size = (int)(bytes / byteToMo);

                    Debug.Log("  L size : " + summary.totalSize + " bytes ; " + size + " Mo");

                    Debug.Log("  L path : " + summary.outputPath);

                    break;
                default:

                    Debug.LogError("Build failed: " + summary.result);

                    break;
            }

        }

        static public void execAtPath(string path)
        {
            WinEdBuildor.os_openFolder(path);
        }

        /// <summary>
        /// open FOLDER of current build
        /// </summary>
        static public void openBuildFolder(string path)
        {
            // remove file name from path
            var profil = getActiveProfile();
            if (path.Contains(profil.getExtension()))
            {
                path = path.Substring(0, path.LastIndexOf("/"));
            }

            WinEdBuildor.os_openFolder(path);
        }

        /// <summary>
        /// https://superuser.com/questions/201371/create-zip-folder-from-the-command-line-windows
        /// https://techcommunity.microsoft.com/t5/containers/tar-and-curl-come-to-windows/ba-p/382409
        /// https://ss64.com/nt/tar.html
        /// </summary>
        void zipBuildFolder(string outputPath, string zipName)
        {

            // path/to/builds/project/zipName.exe
            if (outputPath.Contains("exe"))
            {
                outputPath = outputPath.Substring(0, outputPath.LastIndexOf('/'));
            }

            Debug.Log("zip output path @ " + outputPath);

            // remove '/' just before exe file name
            if (outputPath.EndsWith("/")) outputPath = outputPath.Substring(0, outputPath.Length - 1);

            // parent folder to project/ (builds/)
            string buildsRoot = outputPath.Substring(0, outputPath.LastIndexOf('/'));
            Debug.Log("rool : " + buildsRoot + " (from : " + outputPath + ")");

            // get project/
            string projectFolder = outputPath.Substring(outputPath.LastIndexOf('/') + 1);

            //string args = $"-cf {outputZip} {buildFolderPath}";

            // cd /D D:/fwProtoss/fw/builds/ && tar.exe -avcf fwp.zip fwp__win__0-0-11

            // https://stackoverflow.com/questions/60904/how-can-i-open-a-cmd-window-in-a-specific-location

            // /K cd /D {absPathToBuilds} && tar.exe -avcf {zipName} {relativeBuildFolderName}
            // /K cd /D D:/path/to/builds/ && tar.exe -avcf output.zip game_win_011
            string command = $"/K cd /D {buildsRoot} && tar.exe -avcf {zipName} {projectFolder}";

            Debug.Log("zip : " + command);

            //var info = new System.Diagnostics.ProcessStartInfo();
            //info.
            WinEdBuildor.startCmd(command);
            //System.Diagnostics.Process.Start("cmd", command);
        }

        protected string getBuildName()
        {
            DataBuildSettingsBridge data = getScriptableDataBuildSettings();
            return data.getPlatformProfil().build_prefix;
        }

        static protected string[] getScenePaths()
        {

            List<string> sceneNames = new List<string>();
            int count = SceneManager.sceneCountInBuildSettings;

            Debug.Log("BuildHelper, adding " + count + " scenes to list");

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            for (int i = 0; i < scenes.Length; i++)
            {
                sceneNames.Add(scenes[i].path);
                //Debug.Log("  --> " + i + " , adding " + scenes[i].path);
            }

            return sceneNames.ToArray();
        }

        static public DataBuildSettingsBridge getScriptableDataBuildSettings()
        {
            string[] all = AssetDatabase.FindAssets("t:DataBuildSettingsBridge");

            if (all.Length > 0)
            {
                for (int i = 0; i < all.Length; i++)
                {
                    Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(DataBuildSettingsBridge));
                    DataBuildSettingsBridge data = obj as DataBuildSettingsBridge;
                    if (data != null) return data;
                }
            }
            //Debug.LogWarning("no objects returned by AssetDatabase for type : DataBuildSettingsBridge");

            //Debug.LogError("could not find object of type : DataBuildSettingsBridge");
            return null;
        }

        static public DataBuildSettingProfile getActiveProfile()
        {
            var settings = getScriptableDataBuildSettings();

            if (settings == null) return null;

            return settings.getPlatformProfil();
        }

        static public void applyUnity(DataBuildSettingProfile profil)
        {

            PlayerSettings.SplashScreen.show = false;
            Debug.Log(" L splash show (auto false under licence) : " + PlayerSettings.SplashScreen.show);

            //dev build
            EditorUserBuildSettings.development = profil.developement_build;
            Debug.Log(" L dev build : " + EditorUserBuildSettings.development);

        }

        static public void applyIcons(DataBuildSettingProfile profil)
        {

            Texture2D[] icons = new Texture2D[1];
            icons[0] = profil.icon;

            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);

            Debug.Log(" L icons updated");
        }

        static public void applyCompagny(DataBuildSettingProfile profil, bool verbose = false)
        {

            //profil.applyVersionToEditor(); // apply version

            PlayerSettings.companyName = profil.compagny_name;

            if (verbose) Debug.Log("companyName : " + PlayerSettings.companyName);

            //α,β,Ω
            string productName = profil.getProductName();
            if (profil.phase != BuildPhase.none && profil.phase != BuildPhase.Ω)
            {
                productName += "(" + profil.phase + ")";
            }

            PlayerSettings.productName = productName;
            if (verbose) Debug.Log("productName : " + PlayerSettings.productName);

        }
    }

}

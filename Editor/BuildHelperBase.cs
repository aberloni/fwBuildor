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
    using fwp.version.editor;
    using fwp.version;

    /// <summary>
    /// regroup all parameters used during build process
    /// encapsulate the build process
    /// </summary>
    public class BuildHelperBase
    {
        BuildPlayerOptions buildPlayerOptions;

        IEnumerator preProc = null;
        IEnumerator buildProc = null;

        DataBuildSettingsBridge data = null;

        BuildParameters _parameters;
        BuildSummary? summary = null;

        public class BuildParameters
        {
            public const string pref_prefix = "buildor_";

            static public readonly string pref_uid = pref_prefix + "." + Application.productName + "." + Application.productName;

            static public readonly string pref_include_prefix = pref_uid + "prefix";
            static public readonly string pref_include_platform = pref_uid + "platform";
            static public readonly string pref_include_date = pref_uid + "date";
            static public readonly string pref_include_version = pref_uid + "version";
            static public readonly string pref_suffix = pref_uid + "suffix";
            static public readonly string pref_specific_folder = pref_uid + "specific_folder";
            static public readonly string pref_specific_folder_steam = pref_uid + "folder_steam";
            static public readonly string pref_specific_folder_switch = pref_uid + "folder_switch";

            public static bool IsFolderOverride => EditorPrefs.GetString(BuildHelperBase.BuildParameters.pref_specific_folder).Length > 0;

            /// <summary>
            /// all external additionnal process to exec
            /// </summary>
            public BuildHelperFlags buildFlags = new BuildHelperFlags();

            /// <summary>
            /// profil to use
            /// </summary>
            public DataBuildSettingProfile activeProfil;
        }

        public void launch(BuildParameters param)
        {
            _parameters = param;

            data = getScriptableDataBuildSettings();
            log(" <<< <b>starting build process</b> >>>");

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

                    log("build.preproc.done @ " + Time.realtimeSinceStartup);

                    buildProc = buildProcess();
                }
                return;
            }

            if (buildProc != null)
            {
                if (!buildProc.MoveNext())
                {
                    log("build.done @ " + Time.realtimeSinceStartup);

                    buildProc = null;

                }
                return;
            }

            processEnded();
        }

        void processEnded()
        {
            EditorApplication.update -= update_check_process;
        }

        /// <summary>
        /// whatever is needed to do before building
        /// </summary>
        virtual protected IEnumerator preBuildProcess()
        {
            yield return null;
            //...
        }

        protected IEnumerator buildProcess()
        {
            log("buildProcess");

            //if (!BuildPipeline.isBuildingPlayer)

            build_prep();

            //wait 5 secondes
            float startupTime = Time.realtimeSinceStartup;
            float curTime = Time.realtimeSinceStartup;
            int psec = Mathf.FloorToInt(startupTime);

            float waitTime = 2f;
            log("BuildHelper, waiting " + waitTime + " secondes ...");

            while (curTime - startupTime < waitTime)
            {
                curTime = Time.realtimeSinceStartup;

                //Debug.Log(curTime);
                yield return null;
            }

            //curTime = Time.realtimeSinceStartup;
            //Debug.Log(curTime);

            log("BuildHelper, building @ " + buildPlayerOptions.locationPathName);

            yield return null;

            build_app();
        }

        virtual protected void build_prep()
        {
            log("now building app ; inc version ? " + _parameters.buildFlags.incVersion);

            buildPlayerOptions = new BuildPlayerOptions();

            //this will apply
            if (_parameters.buildFlags.incVersion)
            {
                if (_parameters.buildFlags.isPublishingBuild)
                    VersionIncrementor.incPublishFix();
                else
                    VersionIncrementor.incInternalFix();
            }

            DataBuildSettingProfile profile = data.getPlatformProfil();

            //apply everything (after inc)
            profile.applyProfilEditor(_parameters.buildFlags.isPublishingBuild);

            //buildPlayerOptions.scenes = new[] { "Assets/Scene1.unity", "Assets/Scene2.unity" };
            buildPlayerOptions.scenes = getBuildSettingsScenePaths();

            // === CREATE SOLVED BUILD PATH

            string absPath = profile.getAbsoluteBuildFolderPath();

            bool pathExists = Directory.Exists(absPath);

            if (!pathExists)
            {
                Directory.CreateDirectory(absPath);
                log("folder.absPath: " + absPath);
            }

            // === INJECTING SOLVED PATH TO BUILD SETTINGS

            //[project]_[version].[ext]
            absPath = Path.Combine(absPath, profile.getAppName());

            log("BuildHelper, saving build at : " + absPath);
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
                log("+AUTORUN");
            }

            log("options: " + buildPlayerOptions.options);
            log("locationPathName: " + buildPlayerOptions.locationPathName);

            //      BUILD

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            summary = report.summary;

            switch (summary.Value.result)
            {
                case BuildResult.Succeeded:

                    onSuccess(summary.Value);

                    if (_parameters.buildFlags.openFolderOnSuccess)
                    {
                        log($"OPEN FOLDER of build : {summary.Value.outputPath}");
                        openBuildFolder(summary.Value.outputPath); // success.open
                    }

                    if (_parameters.buildFlags.zipOnSuccess)
                    {
                        string zipName = profile.getZipName();
                        log($"ZIP {summary.Value.outputPath}@{zipName}");
                        zipBuildFolder(summary.Value.outputPath, zipName);
                    }


                    // autorun will inject a flag, no need to do it by hand
                    /*
                    if (_parameters.buildFlags.autorun)
                    {
                        Debug.Log("<color=orange>AUTORUN</color>");
                        WinEdBuildor.winExecute(summary.outputPath); // autorun flag
                    }
                    */

                    break;

                case BuildResult.Failed:
                case BuildResult.Cancelled:
                case BuildResult.Unknown:
                default:
                    Debug.LogError($"BuildResult : Helper Build : {summary.Value.result}");
                    Debug.LogError("options : " + summary.Value.options);
                    Debug.LogError("output path : " + summary.Value.outputPath);
                    break;
            }
        }

        protected void onSuccess(BuildSummary summary)
        {
            log("<b>Build finished</b>");
            log("  L version : <b>" + VersionManager.getFormatedVersion() + "</b>");
            log("  L result : warnings : " + summary.totalWarnings + " | errors " + summary.totalErrors);
            log("  L symbols : " + ScriptableSymbolHelper.getGroupSymbols(summary.platformGroup));
            log("  L platform : <b>" + summary.platform + "</b>");
            log("  L build time : " + summary.totalTime);

            switch (summary.result)
            {
                case BuildResult.Succeeded:

                    log("<color=green>build.Success</color>");

                    ulong bytes = summary.totalSize;
                    ulong byteToMo = 1048576;

                    int size = (int)(bytes / byteToMo);

                    log("  L size : " + summary.totalSize + " bytes ; " + size + " Mo");
                    log("  L path : " + summary.outputPath);

                    break;
                default:
                    log("<color=red>build.Failure</color>");
                    Debug.LogError("Build failed: " + summary.result);

                    break;
            }

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
            if (outputPath.Contains("exe") || outputPath.Contains("app"))
            {
                outputPath = outputPath.Substring(0, outputPath.LastIndexOf('/'));
            }

            log("zip output path @ " + outputPath);

            // remove '/' just before exe file name
            if (outputPath.EndsWith("/")) outputPath = outputPath.Substring(0, outputPath.Length - 1);

            // parent folder to project/ (builds/)
            string buildsRoot = outputPath.Substring(0, outputPath.LastIndexOf('/'));
            log("output : " + outputPath);
            log("root : " + buildsRoot);

            // get project/
            string projectFolder = outputPath.Substring(outputPath.LastIndexOf('/') + 1);

            //string args = $"-cf {outputZip} {buildFolderPath}";

            // cd /D D:/fwProtoss/fw/builds/ && tar.exe -avcf fwp.zip fwp__win__0-0-11

            string folderToZip = $"{buildsRoot}/{projectFolder}";
            string pathZip = $"{buildsRoot}/{zipName}";

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // https://stackoverflow.com/questions/60904/how-can-i-open-a-cmd-window-in-a-specific-location
                string command = $"/K cd /D {buildsRoot} && tar.exe -avcf {zipName} {projectFolder}";
                log("win.zip : " + command);
                WinEdBuildor.winExecute(command);
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string command = $"zip -r {pathZip} {folderToZip}";
                log("osx.zip : " + command);
                WinEdBuildor.osxExecute(command);
            }
        }

        protected string getBuildName()
        {
            DataBuildSettingsBridge data = getScriptableDataBuildSettings();
            return data.getPlatformProfil().build_prefix;
        }

        protected void log(string msg)
        {
            string ret = "[BUILD]";
            if (summary != null) ret += " (" + summary.Value.result + ")";
            ret += " : " + msg;
            Debug.Log(ret);
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

        /// <summary>
        /// 
        /// </summary>
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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace fwp.buildor.editor
{
    using fwp.buildor.version;

    public class WinEdBuildor : EditorWindow
    {
        [MenuItem(BuildorVerbosity._buildor_menuitem_path + "(window) open buildor", false, 0)]
        static void init()
        {
            var win = EditorWindow.GetWindow(typeof(WinEdBuildor));
            win.titleContent = new GUIContent("Buildor");
        }

        Vector2 scroll;

        WinSubVersion subVersion;
        WinSubScriptableSymbols subSymbols;
        WinSubMerger subMergers;

        public BuildHelperBase.BuildParameters parameters;
        public DataBuildSettingProfile activeProfil => parameters.activeProfil;

        private void OnEnable()
        {
            if (parameters == null) parameters = new BuildHelperBase.BuildParameters();
            parameters.activeProfil = BuildHelperBase.getActiveProfile();
        }

        void OnGUI()
        {
            if (subVersion == null) subVersion = new WinSubVersion();
            if (subSymbols == null) subSymbols = new WinSubScriptableSymbols(this);
            if (subMergers == null) subMergers = new WinSubMerger(this);

            GUILayout.Label("Buildor", BuildorHelperGuiStyle.getWinTitle());

            scroll = GUILayout.BeginScrollView(scroll);

            drawProfil();

            if (parameters.activeProfil != null)
            {
                subVersion?.draw(this);
                subMergers?.draw();
                subSymbols?.draw();

                drawBuildFlags();
                drawPathModifiers();
                drawSuccess();
                drawBuildButton();
            }

            GUILayout.EndScrollView();
        }

        void drawProfil()
        {
            if (parameters.activeProfil == null)
            {
                GUILayout.Label("this view needs some active profil setup");
                return;
            }

            GUILayout.BeginHorizontal();
            // just display
            GUILayout.Label("platform", BuildorHelperGuiStyle.getCategoryBold());
            GUI.enabled = false;
            EditorGUILayout.ObjectField(parameters.activeProfil, typeof(DataBuildSettingProfile), true);
            GUI.enabled = true;

            if (GUILayout.Button(">>"))
            {
                UnityEditor.Selection.activeObject = parameters.activeProfil;
            }

            GUILayout.EndHorizontal();

        }

        void drawBuildFlags()
        {

            GUILayout.Label("build flags", BuildorHelperGuiStyle.getCategoryBold());

            parameters.buildFlags.incVersion = WinEdFieldsHelper.drawToggle("incVersion", "incVersion");

            activeProfil.developement_build = GUILayout.Toggle(activeProfil.developement_build, "dev build");
            if (activeProfil.developement_build != EditorUserBuildSettings.development)
            {
                EditorUserBuildSettings.development = activeProfil.developement_build;
                Debug.LogWarning("changed dev build : " + EditorUserBuildSettings.development);

                UnityEditor.EditorUtility.SetDirty(activeProfil);
            }

            activeProfil.debugScripting = GUILayout.Toggle(activeProfil.debugScripting, "debug scripting");
            if (activeProfil.debugScripting != EditorUserBuildSettings.allowDebugging)
            {
                EditorUserBuildSettings.allowDebugging = activeProfil.debugScripting;
                UnityEditor.EditorUtility.SetDirty(activeProfil);
            }

            var level = (DataBuildSettingProfile.ProfilingLevel)EditorGUILayout.EnumPopup("profiling", activeProfil.debugProfiling);

            if (level != activeProfil.debugProfiling)
            {
                switch (level)
                {
                    case DataBuildSettingProfile.ProfilingLevel.deep:
                        EditorUserBuildSettings.connectProfiler = true;
                        EditorUserBuildSettings.buildWithDeepProfilingSupport = true;
                        break;
                    case DataBuildSettingProfile.ProfilingLevel.profiling:
                        EditorUserBuildSettings.connectProfiler = true;
                        EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
                        break;
                    default:
                        EditorUserBuildSettings.connectProfiler = false;
                        EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
                        break;

                }
                activeProfil.debugProfiling = level;
                UnityEditor.EditorUtility.SetDirty(activeProfil);
            }

        }

        void drawPathModifiers()
        {

            GUILayout.Label("path modifiers", BuildorHelperGuiStyle.getCategoryBold());

            GUILayout.BeginHorizontal();
            parameters.pathFlags.pathIncludePrefix = WinEdFieldsHelper.drawToggle("prefix", "pathIncludePrefix");
            parameters.pathFlags.pathIncludePlatform = WinEdFieldsHelper.drawToggle("platform", "pathIncludePlatform");
            parameters.pathFlags.pathIncludeDate = WinEdFieldsHelper.drawToggle("date", "pathIncludeDate");
            parameters.pathFlags.pathIncludeVersion = WinEdFieldsHelper.drawToggle("version", "pathIncludeVersion");
            GUILayout.EndHorizontal();

        }

        void drawSuccess()
        {
            GUILayout.Label("on success", BuildorHelperGuiStyle.getCategoryBold());

            parameters.buildFlags.openFolderOnSuccess = WinEdFieldsHelper.drawToggle("open folder after build", "openAfterBuild");
            parameters.buildFlags.zipOnSuccess = WinEdFieldsHelper.drawToggle("zip", "zip");
            parameters.buildFlags.autorun = WinEdFieldsHelper.drawToggle("autorun", "autorun");

            GUILayout.Space(20f);
            GUILayout.Label("outputs", BuildorHelperGuiStyle.getCategoryBold());

            string outputFolder = activeProfil.getAbsoluteBuildFolderPath(parameters.pathFlags);

            WinEdFieldsHelper.drawCopyPastablePath("base path : ", outputFolder);
            WinEdFieldsHelper.drawCopyPastablePath("app name :", activeProfil.getAppName());
            WinEdFieldsHelper.drawCopyPastablePath("zip name :", activeProfil.getZipName(parameters.pathFlags));

            string fullPath = Path.Combine(outputFolder, activeProfil.getAppName());
            WinEdFieldsHelper.drawCopyPastablePath("full path :", fullPath);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("(folder) player logs"))
            {
                openPlayerLogsFolder();
            }

            if (GUILayout.Button("(folder) editor logs"))
            {
                openEditorLogsFolder();
            }

            if (GUILayout.Button("(folder) build output "))
            {
                BuildHelperBase.openBuildFolder(outputFolder);
            }

            if (GUILayout.Button("exe last build"))
            {
                BuildHelperBase.execAtPath(fullPath);
            }

            GUILayout.EndHorizontal();

        }

        /// <summary>
        /// return override
        /// or active profil
        /// </summary>
        public DataBuildorScenesMerger getActiveMerger()
        {
            if (subMergers.value != null)
            {
                return subMergers.value;
            }
            else if (activeProfil.merger != null)
            {
                return activeProfil.merger;
            }
            return null;
        }

        void drawBuildButton()
        {
            DataBuildSettingVersion version = subVersion.getActiveVersion(this);
            if(version != null)
            {
                GUILayout.Label("+ version : " + version.version);
            }

            DataBuildorScenesMerger merger = getActiveMerger();
            if(merger != null)
            {
                GUILayout.Label("+ merger : " + merger.strOneLine());
            }

            GUILayout.Space(20f);
            if (GUILayout.Button("BUILD", BuildorHelperGuiStyle.getButtonBig(50f)))
            {
                solveBuild();
            }

        }

        void solveBuild()
        {
            getActiveMerger()?.apply();

            new BuildHelperBase(parameters);
        }


        /// <summary>
        /// open explorer at path
        /// </summary>
        /// <param name="folderPath"></param>
        static public void os_openFolder(string folderPath, bool selectFolder = false)
        {
            folderPath = folderPath.Replace(@"/", @"\");   // explorer doesn't like front slashes

            string argument = string.Empty;

            if (selectFolder)
            {
                //https://stackoverflow.com/questions/334630/opening-a-folder-in-explorer-and-selecting-a-file
                argument = "/select, ";
            }

            argument += "\"" + folderPath + "\"";

            UnityEngine.Debug.Log("explorer:opening : " + argument);

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        static public void startCmd(string args)
        {
            Debug.Log("cmd :: " + args);
            startExecute("cmd", args);
        }

        /// <summary>
        /// meant to call cmd on windows
        /// </summary>
        static public void startExecute(string processPath, string args = "")
        {
            // string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";

            var startInfo = new System.Diagnostics.ProcessStartInfo(processPath);
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            if (args.Length > 0) startInfo.Arguments = args;

            //UnityEngine.Debug.Log(processPath);

            //Debug.Log(Environment.CurrentDirectory);

            //https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.start?view=netframework-4.7.2#System_Diagnostics_Process_Start_System_String_System_String_
            //System.Diagnostics.Process.Start(startInfo);
            System.Diagnostics.Process.Start(processPath, args);
        }


        [MenuItem("Window/Buildor/Logs/(folder) player logs")]
        static void openPlayerLogsFolder()
        {
            //https://stackoverflow.com/questions/4494290/detect-the-location-of-appdata-locallow
            // Environment.SpecialFolder.LocalApplicationData)

            //startCmd("C:/Users/lego/AppData/LocalLow/com.redcorner.king/King");
            //startCmd("cmd", "start %APPDATA%"); // roaming
            //startCmd("cmd", "/K \"cd /D %LOCALAPPDATA%\""); // local
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "Low"; // c:/[USERAPPDATA]/LocalLow
            path = System.IO.Path.Combine(path, Application.companyName, Application.productName);
            path = System.IO.Path.Combine(path, "Player.log");

            startExecute(path); // local
        }

        [MenuItem("Window/Buildor/Logs/(folder) editor logs")]
        static void openEditorLogsFolder()
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData); // c:/[USERAPPDATA]/Local
            path = System.IO.Path.Combine(path, "Unity/Editor");

            //startCmd("C:/Users/lego/AppData/LocalLow/com.redcorner.king/King");
            startExecute("C:/Users/lego/AppData/Local/Unity/Editor");
        }

    }

}

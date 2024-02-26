using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using fwp.buildor.version;

namespace fwp.buildor.editor
{
    public class WinEdBuildor : EditorWindow
    {

        [MenuItem("Window/Buildor/(window) open buildor", false, 0)]
        static void init() => EditorWindow.GetWindow(typeof(WinEdBuildor));

        Vector2 scroll;

        BuildHelperFlags flagsBuild;
        BuildPathFlags flagsPath;

        const string _merger_selection = "merger_selection";

        DataBuildorScenesMerger _merger;
        DataBuildorScenesMerger merger
        {
            get
            {
                string val = EditorPrefs.GetString(_merger_selection, string.Empty);
                
                if (_merger != null && !_merger.name.Contains(val))
                {
                    _merger = null;
                    EditorPrefs.SetString(_merger_selection, string.Empty);
                }

                if (_merger == null && !string.IsNullOrEmpty(val))
                {
                    _merger = BuildorHelpers.getScriptableObjectInEditor<DataBuildorScenesMerger>(val);

                    // reset
                    if(_merger == null)
                    {
                        EditorPrefs.SetString(_merger_selection, string.Empty);
                    }
                }

                return _merger;
            }
            set
            {
                if(value != _merger)
                {
                    _merger = value;
                    EditorPrefs.SetString(_merger_selection, _merger.name);
                }
            }
        }

        DataBuildSettingProfile prof;
        bool foldMerger;

        private void Update()
        { }

        void OnGUI()
        {
            GUILayout.Label("Buildor", BuildorHelperGuiStyle.getWinTitle());
            
            prof = BuildHelperBase.getActiveProfile();
            if (prof == null)
            {
                GUILayout.Label("this view needs some active profil setup");
                return;
            }

            scroll = GUILayout.BeginScrollView(scroll);

            GUILayout.BeginHorizontal();
            // just display
            GUILayout.Label("platform", BuildorHelperGuiStyle.getCategoryBold());
            GUI.enabled = false;
            EditorGUILayout.ObjectField(prof, typeof(DataBuildSettingProfile), true);
            GUI.enabled = true;

            if(GUILayout.Button(">>"))
            {
                UnityEditor.Selection.activeObject = prof;
            }

            GUILayout.EndHorizontal();

            drawVersion();

            GUILayout.Label("merger", BuildorHelperGuiStyle.getCategoryBold());

            GUILayout.BeginHorizontal();
            merger = (DataBuildorScenesMerger)EditorGUILayout.ObjectField(merger, typeof(DataBuildorScenesMerger), true);
            if(merger != null)
            {
                if (GUILayout.Button("apply", GUILayout.Width(50f)))
                {
                    merger.apply();
                }
            }
            GUILayout.EndHorizontal();

            foldMerger = EditorGUILayout.Foldout(foldMerger, "scenes in build settings x" + EditorBuildSettings.scenes.Length, true);
            if (foldMerger)
            {
                foreach(var sc in EditorBuildSettings.scenes)
                {
                    GUILayout.Label(sc.path);
                }
            }

            GUILayout.Label("build flags", BuildorHelperGuiStyle.getCategoryBold());

            string symbols = ScriptableSymbolHelper.getGroupSymbols(prof.getPlatformTargetGroup());
            if(string.IsNullOrEmpty(symbols))
            {
                GUILayout.Label("empty scriptable symbols");
            }
            else
            {
                var w = GUILayout.Width(60f);
                //GUILayout.Label("#if "+symbols);
                string[] split = symbols.Split(';');
                GUILayout.BeginHorizontal();
                GUILayout.Label("#if", w);
                for (int i = 0; i < split.Length; i++)
                {
                    if (i > 0 && i % 8 == 0)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }
                    GUILayout.Label(split[i], w);
                }
                GUILayout.EndHorizontal();
            }
            

            flagsBuild.incVersion = WinEdFieldsHelper.drawToggle("incVersion", "incVersion");
            

            prof.developement_build = GUILayout.Toggle(prof.developement_build, "dev build");
            if (prof.developement_build != EditorUserBuildSettings.development)
            {
                EditorUserBuildSettings.development = prof.developement_build;
                Debug.LogWarning("changed dev build : " + EditorUserBuildSettings.development);

                UnityEditor.EditorUtility.SetDirty(prof);
            }

            prof.debugScripting = GUILayout.Toggle(prof.debugScripting, "debug scripting");
            if (prof.debugScripting != EditorUserBuildSettings.allowDebugging)
            {
                EditorUserBuildSettings.allowDebugging = prof.debugScripting;
                UnityEditor.EditorUtility.SetDirty(prof);
            }

            var level = (DataBuildSettingProfile.ProfilingLevel)EditorGUILayout.EnumPopup("profiling", prof.debugProfiling);

            if(level != prof.debugProfiling)
            {
                switch(level)
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
                prof.debugProfiling = level;
                UnityEditor.EditorUtility.SetDirty(prof);
            }

            GUILayout.Label("path modifiers", BuildorHelperGuiStyle.getCategoryBold());

            GUILayout.BeginHorizontal();
            flagsPath.pathIncludePrefix = WinEdFieldsHelper.drawToggle("prefix", "pathIncludePrefix");
            flagsPath.pathIncludePlatform = WinEdFieldsHelper.drawToggle("platform", "pathIncludePlatform");
            flagsPath.pathIncludeDate = WinEdFieldsHelper.drawToggle("date", "pathIncludeDate");
            flagsPath.pathIncludeVersion = WinEdFieldsHelper.drawToggle("version", "pathIncludeVersion");
            
            GUILayout.EndHorizontal();

            GUILayout.Label("on success", BuildorHelperGuiStyle.getCategoryBold());

            flagsBuild.openFolderOnSuccess = WinEdFieldsHelper.drawToggle("open folder after build", "openAfterBuild");
            flagsBuild.zipOnSuccess = WinEdFieldsHelper.drawToggle("zip", "zip");
            flagsBuild.autorun = WinEdFieldsHelper.drawToggle("autorun", "autorun");

            GUILayout.Space(20f);
            GUILayout.Label("outputs", BuildorHelperGuiStyle.getCategoryBold());

            string outputFolder = prof.getAbsoluteBuildFolderPath(flagsPath);
            
            WinEdFieldsHelper.drawCopyPastablePath("base path : ", outputFolder);
            WinEdFieldsHelper.drawCopyPastablePath("app name :", prof.getAppName());
            WinEdFieldsHelper.drawCopyPastablePath("zip name :", prof.getZipName());
            
            string fullPath = Path.Combine(outputFolder, prof.getAppName());
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

            GUILayout.Space(20f);
            if (GUILayout.Button("BUILD", BuildorHelperGuiStyle.getButtonBig(50f)))
            {
                merger?.apply();
                new BuildHelperBase(flagsBuild, flagsPath);
            }

            GUILayout.EndScrollView();
        }

        void drawVersion()
        {

            GUILayout.Space(20f);
            GUILayout.Label("version", BuildorHelperGuiStyle.getCategoryBold());

            BuildSettingVersionType vType = BuildorWinEdHelper.drawEnum<BuildSettingVersionType>("publish type", "publish", 0);
            flagsBuild.isPublishingBuild = vType == BuildSettingVersionType.vPublish;

            GUILayout.BeginHorizontal();

            GUI.enabled = false;
            var version = vType == BuildSettingVersionType.vPublish ? prof.publishVersion : prof.internalVersion;
            EditorGUILayout.ObjectField(version, typeof(DataBuildSettingVersion), true);
            GUI.enabled = true;

            if (version != null)
            {
                GUILayout.Label(version.getFormated());

                if (GUILayout.Button("MAJOR"))
                {
                    version.incrementMajor();
                }
                if (GUILayout.Button("MINOR"))
                {
                    version.incrementMinor();
                }
                if (GUILayout.Button("FIX"))
                {
                    version.incrementFix();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("version manager output : " + VersionManager.getDisplayVersion());

            GUILayout.Space(10f);

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

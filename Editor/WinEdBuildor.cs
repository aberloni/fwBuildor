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
        [MenuItem(BuildorVerbosity._buildor_menuitem_path + "(window) open buildor", false, 0)]
        static void init()
        {
            var win = EditorWindow.GetWindow(typeof(WinEdBuildor));
            win.titleContent = new GUIContent("Buildor");
        }

        private void OnEnable()
        {
            _activeProfil = BuildHelperBase.getActiveProfile();
            if (_activeProfil != null && _activeProfil.merger != null && merger == null)
            {
                merger = _activeProfil.merger;
            }
        }

        DataBuildSettingProfile _activeProfil;

        BuildPathFlags pathFlags;
        BuildHelperFlags buildFlags;

        Vector2 scroll;

        const string _merger_selection = "merger_selection";

        DataBuildorScenesMerger _merger;
        DataBuildorScenesMerger merger
        {
            get
            {
                string val = EditorPrefs.GetString(_merger_selection, string.Empty);

                // save new name if changed
                if (_merger != null && !_merger.name.Contains(val))
                {
                    _merger = null;
                }

                // if none yet, try to get ppref one
                if (_merger == null && !string.IsNullOrEmpty(val))
                {
                    _merger = BuildorHelpers.getScriptableObjectInEditor<DataBuildorScenesMerger>(val);
                }

                return _merger;
            }

            set
            {
                if (value != _merger)
                {
                    _merger = value;
                    EditorPrefs.SetString(_merger_selection, _merger != null ? _merger.name : string.Empty);
                    if (BuildorVerbosity.verbose) Debug.Log("merger:" + merger, _merger);
                }
            }
        }

        bool foldMerger;

        private void Update()
        { }

        void OnGUI()
        {
            GUILayout.Label("Buildor", BuildorHelperGuiStyle.getWinTitle());

            scroll = GUILayout.BeginScrollView(scroll);

            drawProfil();

            if (_activeProfil != null)
            {
                drawVersion();
                drawMerger();
                drawSymbols();
                drawBuildFlags();
                drawPathModifiers();
                drawSuccess();
                drawBuildButton();
            }

            GUILayout.EndScrollView();
        }

        void drawProfil()
        {
            if (_activeProfil == null)
            {
                GUILayout.Label("this view needs some active profil setup");
                return;
            }

            GUILayout.BeginHorizontal();
            // just display
            GUILayout.Label("platform", BuildorHelperGuiStyle.getCategoryBold());
            GUI.enabled = false;
            EditorGUILayout.ObjectField(_activeProfil, typeof(DataBuildSettingProfile), true);
            GUI.enabled = true;

            if (GUILayout.Button(">>"))
            {
                UnityEditor.Selection.activeObject = _activeProfil;
            }

            GUILayout.EndHorizontal();

        }


        void drawMerger()
        {
            GUILayout.Label("merger", BuildorHelperGuiStyle.getCategoryBold());

            GUILayout.BeginHorizontal();
            merger = (DataBuildorScenesMerger)EditorGUILayout.ObjectField(merger, typeof(DataBuildorScenesMerger), true);
            if (merger != null)
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
                foreach (var sc in EditorBuildSettings.scenes)
                {
                    GUILayout.Label(sc.path);
                }
            }

        }

        void drawSymbols()
        {
            GUILayout.Label("symbols (#if)", BuildorHelperGuiStyle.getCategoryBold());

            string symbols = ScriptableSymbolHelper.getGroupSymbols(_activeProfil.getPlatformTargetGroup());
            if (string.IsNullOrEmpty(symbols))
            {
                GUILayout.Label("empty scriptable symbols");
            }
            else
            {
                var w = GUILayout.Width(60f);
                //GUILayout.Label("#if "+symbols);
                string[] split = symbols.Split(';');
                GUILayout.BeginHorizontal();
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

        }

        void drawBuildFlags()
        {

            GUILayout.Label("build flags", BuildorHelperGuiStyle.getCategoryBold());

            buildFlags.incVersion = WinEdFieldsHelper.drawToggle("incVersion", "incVersion");

            _activeProfil.developement_build = GUILayout.Toggle(_activeProfil.developement_build, "dev build");
            if (_activeProfil.developement_build != EditorUserBuildSettings.development)
            {
                EditorUserBuildSettings.development = _activeProfil.developement_build;
                Debug.LogWarning("changed dev build : " + EditorUserBuildSettings.development);

                UnityEditor.EditorUtility.SetDirty(_activeProfil);
            }

            _activeProfil.debugScripting = GUILayout.Toggle(_activeProfil.debugScripting, "debug scripting");
            if (_activeProfil.debugScripting != EditorUserBuildSettings.allowDebugging)
            {
                EditorUserBuildSettings.allowDebugging = _activeProfil.debugScripting;
                UnityEditor.EditorUtility.SetDirty(_activeProfil);
            }

            var level = (DataBuildSettingProfile.ProfilingLevel)EditorGUILayout.EnumPopup("profiling", _activeProfil.debugProfiling);

            if (level != _activeProfil.debugProfiling)
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
                _activeProfil.debugProfiling = level;
                UnityEditor.EditorUtility.SetDirty(_activeProfil);
            }

        }

        void drawPathModifiers()
        {

            GUILayout.Label("path modifiers", BuildorHelperGuiStyle.getCategoryBold());

            GUILayout.BeginHorizontal();
            pathFlags.pathIncludePrefix = WinEdFieldsHelper.drawToggle("prefix", "pathIncludePrefix");
            pathFlags.pathIncludePlatform = WinEdFieldsHelper.drawToggle("platform", "pathIncludePlatform");
            pathFlags.pathIncludeDate = WinEdFieldsHelper.drawToggle("date", "pathIncludeDate");
            pathFlags.pathIncludeVersion = WinEdFieldsHelper.drawToggle("version", "pathIncludeVersion");
            GUILayout.EndHorizontal();

        }

        void drawSuccess()
        {
            GUILayout.Label("on success", BuildorHelperGuiStyle.getCategoryBold());

            buildFlags.openFolderOnSuccess = WinEdFieldsHelper.drawToggle("open folder after build", "openAfterBuild");
            buildFlags.zipOnSuccess = WinEdFieldsHelper.drawToggle("zip", "zip");
            buildFlags.autorun = WinEdFieldsHelper.drawToggle("autorun", "autorun");

            GUILayout.Space(20f);
            GUILayout.Label("outputs", BuildorHelperGuiStyle.getCategoryBold());

            string outputFolder = _activeProfil.getAbsoluteBuildFolderPath(pathFlags);

            WinEdFieldsHelper.drawCopyPastablePath("base path : ", outputFolder);
            WinEdFieldsHelper.drawCopyPastablePath("app name :", _activeProfil.getAppName());
            WinEdFieldsHelper.drawCopyPastablePath("zip name :", _activeProfil.getZipName(pathFlags));

            string fullPath = Path.Combine(outputFolder, _activeProfil.getAppName());
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

        void drawBuildButton()
        {
            GUILayout.Space(20f);
            if (GUILayout.Button("BUILD", BuildorHelperGuiStyle.getButtonBig(50f)))
            {
                merger?.apply();
                new BuildHelperBase(buildFlags, pathFlags);
            }

        }

        /// <summary>
        /// result is stored in flagsBuild
        /// </summary>
        void drawVersion()
        {

            GUILayout.Space(20f);
            GUILayout.Label("version", BuildorHelperGuiStyle.getCategoryBold());

            BuildSettingVersionType vType = BuildorWinEdHelper.drawEnum<BuildSettingVersionType>("publish type", "publish", 0);
            buildFlags.isPublishingBuild = vType == BuildSettingVersionType.vPublish;

            GUILayout.BeginHorizontal();

            GUI.enabled = false;
            var version = vType == BuildSettingVersionType.vPublish ? _activeProfil.publishVersion : _activeProfil.internalVersion;
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

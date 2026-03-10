using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using fwp.version;
using fwp.logs.editor;
using fwp.logs;

namespace fwp.buildor.editor
{
	public class WinEdBuildor : EditorWindow
	{
		[MenuItem(BuildorVerbosity._buildor_menuitem_path + "buildor (win)", false, 0)]
		static void init()
		{
			var win = EditorWindow.GetWindow(typeof(WinEdBuildor));
			win.titleContent = new GUIContent("Buildor");
		}

		Vector2 scroll;

		WinSubVersionBuildor subVersion;
		WinSubScriptableSymbols subSymbols;
		WinSubMerger subMergers;
		WinSubLogs subLogs;

		public BuildHelperBase.BuildParameters parameters;

		private void OnEnable()
		{
			if (parameters == null) parameters = new BuildHelperBase.BuildParameters();

			if (subVersion == null) subVersion = new();
			if (subSymbols == null) subSymbols = new WinSubScriptableSymbols(this);
			if (subMergers == null) subMergers = new WinSubMerger(this);
			if (subLogs == null) subLogs = new WinSubLogs(this);

			var profil = BuildHelperBase.getActiveProfile();
			if (profil == null) Debug.LogWarning("profil.missing");
			else Debug.Log("profil:" + profil.name, profil);
		}

		private void OnFocus()
		{
			subLogs?.focus();
			subMergers?.focus();
			subSymbols?.focus();
		}

		readonly GUIContent _title = new("Buildor");
		void OnGUI()
		{
			GUILayout.Label(_title, BuildorHelperGuiStyle.gWinTitle);

			scroll = GUILayout.BeginScrollView(scroll);

			drawProfil();

			var profil = BuildHelperBase.getActiveProfile();
			if (profil != null)
			{
				subVersion?.draw(this);
				subMergers?.draw();
				subSymbols?.draw();
				subLogs?.draw();

				drawBuildFlags();
				drawPathModifiers();
				drawSuccess();
				drawBuildButton();
			}

			GUILayout.EndScrollView();
		}

		void drawProfil()
		{
			var profil = BuildHelperBase.getActiveProfile();
			if (profil == null)
			{
				GUILayout.Label("this view needs some active profil setup");
				if (GUILayout.Button("refresh"))
				{
					BuildHelperBase.getActiveProfile(true);
				}
				return;
			}

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("?"))
			{
				UnityEditor.Selection.activeObject = profil;
			}

			// just display
			GUILayout.Label("platform", BuildorHelperGuiStyle.gCategoryBold);
			GUI.enabled = false;
			EditorGUILayout.ObjectField(profil, typeof(DataBuildSettingProfile), true);
			GUI.enabled = true;

			GUILayout.EndHorizontal();

		}

		void drawBuildFlags()
		{
			if (parameters == null) return;

			GUILayout.Label("build flags", BuildorHelperGuiStyle.gCategoryBold);

			parameters.buildFlags.incVersion = WinEdFieldsHelper.drawToggle("incVersion", "incVersion");

			var profil = BuildHelperBase.getActiveProfile();
			if (profil == null) return;

			profil.developement_build = GUILayout.Toggle(profil.developement_build, "dev build");
			if (profil.developement_build != EditorUserBuildSettings.development)
			{
				EditorUserBuildSettings.development = profil.developement_build;
				Debug.LogWarning("changed dev build : " + EditorUserBuildSettings.development);

				UnityEditor.EditorUtility.SetDirty(profil);
			}

			profil.debugScripting = GUILayout.Toggle(profil.debugScripting, "debug scripting");
			if (profil.debugScripting != EditorUserBuildSettings.allowDebugging)
			{
				EditorUserBuildSettings.allowDebugging = profil.debugScripting;
				UnityEditor.EditorUtility.SetDirty(profil);
			}

			var level = (DataBuildSettingProfile.ProfilingLevel)EditorGUILayout.EnumPopup("profiling", profil.debugProfiling);

			if (level != profil.debugProfiling)
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
				profil.debugProfiling = level;
				UnityEditor.EditorUtility.SetDirty(profil);
			}

		}

		readonly GUIContent btn_browse = new GUIContent("browse");

		void drawSpecificFolder()
		{
			string ppref = BuildHelperBase.BuildParameters.pref_specific_folder + "_" + Application.platform;

			// force to a specific folder
			// GUILayout.Label("specific build/", BuildorHelperGuiStyle.gBold);

			string path = EditorPrefs.GetString(ppref);
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(btn_browse, GUILayout.Width(100f)))
			{
				string _path = EditorUtility.OpenFolderPanel("Select export folder", path, string.Empty);
				if (path != _path)
				{
					EditorPrefs.SetString(ppref, _path);
				}
			}
			else
			{
				WinEdFieldsHelper.editText(Application.platform.ToString(), ppref);

				if (path.Length > 0 && GUILayout.Button("clear", GUILayout.Width(100f)))
					EditorPrefs.SetString(ppref, string.Empty);

			}
			GUILayout.EndHorizontal();
		}

		void drawPathModifiers()
		{
			GUILayout.Label("path modifiers", BuildorHelperGuiStyle.gCategoryBold);


			GUILayout.Label("build/ modifiers", BuildorHelperGuiStyle.gBold);
			// do not provide current value to drawer, must regen value on opening from ppref
			GUILayout.BeginHorizontal();
			WinEdFieldsHelper.drawToggle("prefix", BuildHelperBase.BuildParameters.pref_include_prefix);
			WinEdFieldsHelper.drawToggle("platform", BuildHelperBase.BuildParameters.pref_include_platform);
			if (!BuildHelperBase.BuildParameters.IsFolderOverride)
			{
				WinEdFieldsHelper.drawToggle("date", BuildHelperBase.BuildParameters.pref_include_date);
				WinEdFieldsHelper.drawToggle("version", BuildHelperBase.BuildParameters.pref_include_version);
			}
			GUILayout.EndHorizontal();

			WinEdFieldsHelper.editText("suffix", BuildHelperBase.BuildParameters.pref_suffix);

			drawSpecificFolder();
		}

		void drawSuccess()
		{
			GUILayout.Label("on success", BuildorHelperGuiStyle.gCategoryBold);

			parameters.buildFlags.openFolderOnSuccess = WinEdFieldsHelper.drawToggle("open folder after build", "openAfterBuild");
			parameters.buildFlags.zipOnSuccess = WinEdFieldsHelper.drawToggle("zip", "zip");
			parameters.buildFlags.autorun = WinEdFieldsHelper.drawToggle("autorun", "autorun");

			GUILayout.Space(20f);
			GUILayout.Label("outputs", BuildorHelperGuiStyle.gCategoryBold);

			var profil = BuildHelperBase.getActiveProfile();
			string outputFolder = profil.getAbsoluteBuildFolderPath();

			WinEdFieldsHelper.drawCopyPastablePath("base path : ", outputFolder);
			WinEdFieldsHelper.drawCopyPastablePath("app name :", profil.getAppName());
			WinEdFieldsHelper.drawCopyPastablePath("zip name :", profil.getZipName());

			string fullPath = Path.Combine(outputFolder, profil.getAppName());
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
				BuildHelperBase.openBuildFolder(outputFolder); // win.button
			}

			if (GUILayout.Button("exe last build"))
			{
				winExecute(fullPath); // win.button
			}

			GUILayout.EndHorizontal();

		}

		string getBuildLabel() => parameters.buildFlags.autorun ? "SHIPIT" : "BUILD";

		void drawBuildButton()
		{
			DataBuildSettingVersion version = getActiveVersion();
			if (version != null)
			{
				GUILayout.Label("+ version : " + version.version);
			}

			DataBuildorScenesMerger merger = subMergers.Value;
			if (merger != null) GUILayout.Label("+ merger : " + merger.strOneLine());

			ProfilLogLevels logs = subLogs.Value;
			if (logs != null) GUILayout.Label("+ logs : " + logs.ToString());

			GUILayout.Space(20f);

			if (GUILayout.Button(getBuildLabel(), BuildorHelperGuiStyle.gButtonBig))
			{
				Debug.Log("[BUILD] apply.merger:" + merger.strOneLine());
				merger?.apply();

				Debug.Log("[BUILD] apply.logs:" + logs);
				logs?.apply();

				var helper = createBuildHelper();
				if (helper == null)
				{
					Debug.LogError("must provide helper");
					return;
				}

				helper.launch(parameters);
			}

		}

		virtual protected BuildHelperBase createBuildHelper()
		{
			return new BuildHelperBase();
		}

		public DataBuildSettingVersion getActiveVersion()
		{
			DataBuildSettingVersion version = null;

			var profil = BuildHelperBase.getActiveProfile();
			if (profil != null)
			{
				version = parameters.buildFlags.isPublishingBuild ? profil.publishVersion : profil.internalVersion;
			}

			return version;
		}

		/// <summary>
		/// open explorer at path
		/// </summary>
		/// <param name="folderPath"></param>
		static public void os_openFolder(string folderPath, bool selectFolder = false)
		{
			folderPath = folderPath.Replace(@"\", @"/"); // uniform
			if (!folderPath.EndsWith("/")) folderPath += "/"; // last /

			if (!System.IO.Directory.Exists(folderPath))
			{
				Debug.LogWarning("no folder " + folderPath);
				return;
			}

			string argument = string.Empty;
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				folderPath = folderPath.Replace(@"/", @"\");   // explorer doesn't like front slashes

				if (selectFolder)
				{
					//https://stackoverflow.com/questions/334630/opening-a-folder-in-explorer-and-selecting-a-file
					argument = "/select, ";
				}

				argument += "\"" + folderPath + "\"";

				UnityEngine.Debug.Log("explorer:opening : " + argument);

				System.Diagnostics.Process.Start("explorer.exe", argument);
			}
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{


				UnityEngine.Debug.Log("finder:opening : " + folderPath);
				EditorUtility.RevealInFinder(folderPath);
			}
			else
			{
				throw new System.NotImplementedException("platform not implem");
			}


		}

		static public void osxExecute(string args)
		{
			Debug.Log("osx.bash : " + args);
			var pStart = new System.Diagnostics.ProcessStartInfo("/bin/bash");
			//pStart.FileName = "/System/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal";
			pStart.WorkingDirectory = "/";

			pStart.UseShellExecute = false;
			pStart.RedirectStandardInput = true;
			pStart.RedirectStandardOutput = true;

			var proc = new System.Diagnostics.Process();
			proc.StartInfo = pStart;
			proc.Start();

			proc.StandardInput.WriteLine(args);
			proc.StandardInput.Flush();
			//proc.WaitForExit();
		}

		/// <summary>
		/// call of generic windows process
		/// </summary>
		static public void winExecute(string processPath, string args = "")
		{
			var process = new System.Diagnostics.Process();
			process.StartInfo.FileName = processPath;
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			if (args.Length > 0) process.StartInfo.Arguments = args;

			Debug.Log("win.exe : " + processPath);

			//https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.start?view=netframework-4.7.2#System_Diagnostics_Process_Start_System_String_System_String_
			process.Start();
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

			winExecute(path); // local
		}

		[MenuItem("Window/Buildor/Logs/(folder) editor logs")]
		static void openEditorLogsFolder()
		{
			string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData); // c:/[USERAPPDATA]/Local
			path = System.IO.Path.Combine(path, "Unity/Editor");

			//startCmd("C:/Users/lego/AppData/LocalLow/com.redcorner.king/King");
			os_openFolder("C:/Users/lego/AppData/Local/Unity/Editor");
		}

	}

}

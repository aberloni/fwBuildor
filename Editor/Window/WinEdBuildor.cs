using UnityEngine;
using UnityEditor;

using fwp.logs;
using fwp.symbols;
using fwp.symbols.editor;

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

		DataBuildSettingProfile Profile => BuildorHelpers.Profile;

		Vector2 _scroll;

		DataBuildSettingProfile aProfil;

		BuildHelperBase helper;

		WinSubVersionBuildor subVersion;
		WinSubMerger subMergers;

		/// <summary>
		/// this helper manage process during building
		/// </summary>
		virtual protected BuildHelperBase createBuildHelper()
		{
			return new BuildHelperBase();
		}

		private void OnEnable()
		{

			if (subVersion == null) subVersion = new();
			if (subMergers == null) subMergers = new(this);

			swap(BuildorHelpers.Profile);

			if (helper == null) helper = createBuildHelper();
		}

		private void OnFocus()
		{
			if (helper == null) helper = createBuildHelper();

			subMergers?.focus();
		}

		readonly GUIContent _title = new("Buildor");
		void OnGUI()
		{
			GUILayout.Label(_title, HelperGui.gWinTitle);

			_scroll = GUILayout.BeginScrollView(_scroll);

			drawProfilSelector();

			if (aProfil != null)
			{
				subVersion?.draw(this);
				subMergers?.draw();

				drawSymbols();
				drawLogs();

				if (helper != null)
				{
					GUILayout.Label("build flags", HelperGui.gCategoryBold);
					helper.build_prepro.incVersion = WinEdFieldsHelper.drawToggle("incVersion", "incVersion");
				}

				if (aProfil.debug == null) GUILayout.Label("-no debug-");
				else aProfil.debug.drawEd();

				drawPathModifiers();
				drawPostProcess();
				drawBuildButton();
			}

			GUILayout.EndScrollView();
		}

		void swap(DataBuildSettingProfile np)
		{
			aProfil = np;
			aProfil?.applyProfilEditor();

			subMergers?.focus();

			Debug.Log("*new* " + aProfil, aProfil);
		}

		void drawProfilSelector()
		{
			if (BuildorHelpers.PublishLevel != BuildorWinEdHelper.drawEnum<PublishLevel>("publish", BuildorHelpers.ppref_publish))
			{
				swap(Profile);
			}

			if (BuildorHelpers.DebugLevel != BuildorWinEdHelper.drawEnum<DebugLevel>("debug", BuildorHelpers.ppref_debug))
			{
				swap(Profile);
			}

			GUILayout.BeginHorizontal();

			if (aProfil == null)
			{
				GUILayout.Label("this view needs some active profil setup");
				if (GUILayout.Button("refresh"))
				{
					aProfil = Profile;
				}
			}
			else
			{
				// just display
				GUILayout.Label("platform", HelperGui.gCategoryBold);
				GUI.enabled = false;
				EditorGUILayout.ObjectField(aProfil, typeof(DataBuildSettingProfile), true);
				GUI.enabled = true;

				if (GUILayout.Button(">>", GUILayout.Width(40f)))
				{
					UnityEditor.Selection.activeObject = aProfil;
				}

			}

			GUILayout.EndHorizontal();


		}

		void drawSymbols()
		{
			GUILayout.Label("Symbols", HelperGui.gCategoryBold);

			var logs = Profile.GetLogsLevels(BuildorHelpers.DebugLevel);
			var _logs = WinEdFieldsHelper.drawObject("logs", logs);
			if (_logs != logs)
			{
				if (BuildorHelpers.IsDebug) Profile.debug.logLevels = _logs;
				else Profile.build.logLevels = _logs;
			}

			Profile.build.watermark = WinEdFieldsHelper.drawToggle(Profile.build.watermark, "watermark");
			string sCurrent = ScriptSymbolsView.getPlayerSetSymbols(Profile.getBuildTargetGroup());
			GUILayout.Label("unity: " + sCurrent);

			GUILayout.BeginHorizontal();
			string symbs = Profile.Symbols;
			if (string.IsNullOrEmpty(symbs)) GUILayout.Label("empty symbols");
			else GUILayout.Label("profil: " + symbs, HelperGui.gWrapped);
			if (sCurrent != symbs && GUILayout.Button("apply", HelperGui.bS))
			{
				Debug.LogWarning("apply.symbols: " + symbs, Profile);
				ScriptSymbolsView.applyEditor(Profile.getBuildTargetGroup(), symbs);
			}
			GUILayout.EndHorizontal();
		}

		void drawLogs()
		{
			GUILayout.Label("Logs", HelperGui.gCategoryBold);

			if (!BuildorHelpers.IsDebug && Profile.build != null && Profile.build.logLevels != null)
			{
				GUILayout.Label("build.logs", HelperGui.gBold);
				foreach (var lvl in Profile.build.logLevels.levels)
				{
					GUILayout.Label(lvl.stringify());
				}
			}
			else if (BuildorHelpers.IsDebug && Profile.build != null && Profile.build.logLevels != null)
			{
				GUILayout.Label("debug.logs", HelperGui.gBold);
				foreach (var lvl in Profile.debug.logLevels.levels)
				{
					GUILayout.Label(lvl.stringify());
				}
			}

		}

		readonly GUIContent gui_btn_browse = new GUIContent("browse");

		/// <summary>
		/// change destination folder
		/// </summary>
		void drawPathModifiers()
		{
			GUILayout.Label("path modifiers", HelperGui.gCategoryBold);

			GUILayout.Label("build/ modifiers", HelperGui.gBold);


			//EditorGUI.BeginDisabledGroup(true);
			GUI.enabled = !aProfil.build.HasSpecificFolder;
			// do not provide current value to drawer, must regen value on opening from ppref
			GUILayout.BeginHorizontal();
			WinEdFieldsHelper.drawToggle("prefix", BuildHelperBase.pref_include_prefix);
			WinEdFieldsHelper.drawToggle("platform", BuildHelperBase.pref_include_platform);
			WinEdFieldsHelper.drawToggle("date", BuildHelperBase.pref_include_date);
			WinEdFieldsHelper.drawToggle("version", BuildHelperBase.pref_include_version);
			GUILayout.EndHorizontal();

			WinEdFieldsHelper.editText("suffix", BuildHelperBase.pref_suffix);
			GUI.enabled = true;

			// force to a specific folder
			GUILayout.Label("build/ specific", HelperGui.gBold);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button(gui_btn_browse, GUILayout.Width(100f)))
			{
				string _path = EditorUtility.OpenFolderPanel(
					"Select export folder", aProfil.build.build_folder_specific, string.Empty);

				if (aProfil.build.build_folder_specific != _path)
				{
					aProfil.build.build_folder_specific = _path;
				}
			}
			else
			{
				GUILayout.Label(aProfil.build.build_folder_specific);

				if (aProfil.build.build_folder_specific.Length > 0 && GUILayout.Button("clear", GUILayout.Width(100f)))
				{
					aProfil.build.build_folder_specific = string.Empty;
				}
			}
			GUILayout.EndHorizontal();
		}

		void drawPostProcess()
		{
			if (helper == null) return;

			GUILayout.Label("on success", HelperGui.gCategoryBold);

			helper.build_postpro.openFolderOnSuccess = WinEdFieldsHelper.drawToggle("open folder after build", "openAfterBuild");
			helper.build_postpro.zipOnSuccess = WinEdFieldsHelper.drawToggle("zip", "zip");
			helper.build_prepro.autorun = WinEdFieldsHelper.drawToggle("autorun", "autorun");

			GUILayout.Space(20f);
			GUILayout.Label("outputs", HelperGui.gCategoryBold);

			GUILayout.BeginHorizontal();
			WinEdFieldsHelper.drawCopyPastablePath("app subpath : ", aProfil.getRelativeBuildFolderPath(), false);
			WinEdFieldsHelper.drawCopyPastablePath("app name :", aProfil.getAppName(), false);
			GUILayout.EndHorizontal();

			WinEdFieldsHelper.drawCopyPastablePath("export path :", aProfil.BuildPath);

			WinEdFieldsHelper.drawCopyPastablePath("zip name :", aProfil.getZipName());

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
				BuildPostprocess.openBuildFolder(aProfil.FullPath); // win.button
			}

			if (GUILayout.Button("exe last build"))
			{
				winExecute(aProfil.FullPath); // win.button
			}

			GUILayout.EndHorizontal();

		}

		string getBuildLabel() => helper.build_prepro.autorun ? "SHIPIT" : "BUILD";

		void drawBuildButton()
		{
			if (helper == null) return;

			if (helper.build_prepro.incVersion)
			{
				if (aProfil.versionInternal != null) GUILayout.Label("+ v.internal : " + aProfil.versionInternal.version);
				if (aProfil.versionPublish != null) GUILayout.Label("+ v.publish : " + aProfil.versionPublish.version);
			}

			GUILayout.Label("+ path : " + aProfil.FullPath);

			DataBuildorScenesMerger merger = null;
			if (subMergers != null) merger = subMergers.Value;
			if (merger != null) GUILayout.Label("+ merger : " + merger.strOneLine());

			ProfilLogLevels logs = null;
			if (logs != null) GUILayout.Label("+ logs : " + logs.ToString());

			GUILayout.Space(20f);

			/// BUILD
			if (GUILayout.Button(getBuildLabel(), HelperGui.gButtonBig))
			{
				Debug.Log("[BUILD] apply.merger:" + merger.strOneLine());
				merger?.apply();

				Debug.Log("[BUILD] apply.logs:" + logs);
				logs?.applyLogs();

				if (helper == null)
				{
					Debug.LogError("must provide helper");
					return;
				}

				aProfil.versionInternal?.event_build();
				aProfil.versionPublish?.event_build();

				helper.launch();
			}

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

			Debug.Log("winExecute @ " + processPath);

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

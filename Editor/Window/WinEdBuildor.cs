using UnityEngine;
using UnityEditor;

using fwp.logs;
using fwp.symbols;
using System.IO;

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

		Vector2 _scroll;

		DataBuildSettingProfile aProfil;

		WinSubVersionBuildor subVersion; // draw version tools

		private void OnEnable()
		{

			if (subVersion == null) subVersion = new();

			onProfilRefresh();

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

				drawSymbols();
				drawLogs();

				if (aProfil != null && BuildorVars.IsDebug)
				{
					aProfil.debug.drawEd();
				}

				drawPathModifiers();
				drawPreprocessVars();
				drawBuildButton();
			}

			GUILayout.EndScrollView();
		}

		void onProfilRefresh()
		{
			aProfil = BuildorVars.Profile;

			Debug.Log("*new* " + aProfil, aProfil);

			aProfil?.injectProfilToEditor();
		}

		void drawProfilSelector()
		{


			GUILayout.BeginHorizontal();

			HelperGuiFields.drawPrefEnum<PublishLevel>(BuildorVars.ppref_publish, "publish", (value) =>
			{
				onProfilRefresh();
			});

			HelperGuiFields.drawPrefEnum<Sdks>(BuildorVars.ppref_sdk, "sdk", (value) =>
			{
				onProfilRefresh();
			});

			HelperGuiFields.drawPrefEnum<DebugLevel>(BuildorVars.ppref_debug, "debug", (value) =>
			{
				onProfilRefresh();
			});

			GUILayout.EndHorizontal();

			if (aProfil == null)
			{
				GUILayout.Label("no active profil");
				return;
			}

			GUILayout.BeginHorizontal();

			GUILayout.Label(Application.platform.ToString());
			
			GUI.enabled = false;
			EditorGUILayout.ObjectField(aProfil, typeof(DataBuildSettingProfile), true);
			GUI.enabled = true;
			if (GUILayout.Button("refresh")) onProfilRefresh();
			if (GUILayout.Button(">>", GUILayout.Width(40f))) Selection.activeObject = aProfil;
			GUILayout.EndHorizontal();

		}

		void drawSymbols()
		{
			GUILayout.Label("Symbols", HelperGui.gCategoryBold);

			var p = BuildorVars.Profile;

			HelperGuiFields.drawLabel(BuildorHelpers.formatSymbols(p.build.symbols));

			var logs = p.GetLogsLevels(BuildorVars.TargetDebug);
			if (logs != null)
			{
				HelperGuiFields.drawObjectDisabled(logs);
				HelperGuiFields.drawLabel(BuildorHelpers.formatSymbols(logs.symbolsVerbose));
			}
			p.build.watermark = HelperGuiFields.drawToggle(p.build.watermark, "watermark");

			string sCurrent = ScriptSymbolsView.getPlayerSetSymbols(p.getBuildTargetGroup());
			GUILayout.Label("unity: " + sCurrent);

			GUILayout.BeginHorizontal();
			string symbs = p.Symbols;
			if (string.IsNullOrEmpty(symbs)) GUILayout.Label("empty symbols");
			else GUILayout.Label("profil: " + symbs, HelperGui.gWrapped);
			if (sCurrent != symbs && GUILayout.Button("apply", HelperGui.bS))
			{
				Debug.LogWarning("apply.symbols: " + symbs, p);
				ScriptSymbolsView.applyEditor(p.getBuildTargetGroup(), symbs);
			}
			GUILayout.EndHorizontal();
		}

		void drawLogs()
		{
			var p = BuildorVars.Profile;



			var logs = p.GetLogsLevels(BuildorVars.TargetDebug);
			if (logs != null)
			{
				GUILayout.Label("Logs", HelperGui.gCategoryBold);
				// GUILayout.BeginVertical();
				HelperGuiFields.drawObjectDisabled(logs);
				GUILayout.BeginHorizontal();
				foreach (var lvl in logs.levels) GUILayout.Label(lvl.stringify());
				GUILayout.EndHorizontal();
				// GUILayout.EndVertical();
			}
			/*
			GUILayout.BeginHorizontal();
			if (p.build.logLevels != null)
			{
				GUILayout.BeginVertical();
				GUILayout.Label("build.logs", HelperGui.gBold);
				HelperGuiFields.drawObjectDisabled(p.build.logLevels);
				foreach (var lvl in p.build.logLevels.levels)
				{
					GUILayout.Label(lvl.stringify());
				}
				GUILayout.EndVertical();
			}

			if (p.debug.logLevels != null)
			{
				GUILayout.BeginVertical();
				GUILayout.Label("debug.logs", HelperGui.gBold);
				HelperGuiFields.drawObjectDisabled(p.debug.logLevels);
				foreach (var lvl in p.debug.logLevels.levels)
				{
					GUILayout.Label(lvl.stringify());
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
			*/
		}

		readonly GUIContent gui_btn_browse = new GUIContent("browse");

		/// <summary>
		/// change destination folder
		/// </summary>
		void drawPathModifiers()
		{
			GUILayout.Label("Path modifiers", HelperGui.gCategoryBold);

			GUILayout.Label("build/ modifiers", HelperGui.gBold);


			//EditorGUI.BeginDisabledGroup(true);
			GUI.enabled = !aProfil.build.HasSpecificFolder;
			// do not provide current value to drawer, must regen value on opening from ppref
			GUILayout.BeginHorizontal();
			HelperGuiFields.drawPrefToggle(BuildorVars.pref_include_prefix, "prefix");
			HelperGuiFields.drawPrefToggle(BuildorVars.pref_include_platform, "platform");
			HelperGuiFields.drawPrefToggle(BuildorVars.pref_include_date, "date");
			HelperGuiFields.drawPrefToggle(BuildorVars.pref_include_version, "version");
			GUILayout.EndHorizontal();

			HelperGuiFields.editText("suffix", BuildorVars.pref_suffix);
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
			else if (!string.IsNullOrEmpty(aProfil.build.build_folder_specific))
			{
				GUILayout.Label(aProfil.build.build_folder_specific);

				if (aProfil.build.build_folder_specific.Length > 0 && GUILayout.Button("clear", GUILayout.Width(100f)))
				{
					aProfil.build.build_folder_specific = string.Empty;
				}
			}
			GUILayout.EndHorizontal();
		}

		void drawPreprocessVars()
		{
			GUILayout.Space(20f);
			GUILayout.Label("outputs", HelperGui.gCategoryBold);

			GUILayout.BeginHorizontal();
			HelperGuiFields.drawCopyPastablePath("app subpath : ", aProfil.getRelativeBuildFolderPath(), false);
			HelperGuiFields.drawCopyPastablePath("app name :", aProfil.getAppName(), false);
			GUILayout.EndHorizontal();

			HelperGuiFields.drawCopyPastablePath("export path :", aProfil.BuildPath);

			HelperGuiFields.drawCopyPastablePath("zip name :", aProfil.getZipName());

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("(folder) player logs"))
			{
				BuildPostprocess.openPlayerLogsFolder();
			}

			if (GUILayout.Button("(folder) editor logs"))
			{
				BuildPostprocess.openEditorLogsFolder();
			}

			if (GUILayout.Button("(folder) build/"))
			{
				BuildPostprocess.openBuildFolder(aProfil.BuildPath, true); // win.button
			}

			if (GUILayout.Button("exe last build"))
			{
				BuildPostprocess.shellOpenFile(aProfil.FullPath); // win.button
			}

			GUILayout.EndHorizontal();

		}

		string getBuildLabel() => BuildorVars.PostAutorun ? "SHIPIT" : "BUILD";

		void drawBuildButton()
		{
			drawPrevisualization();
			GUILayout.Space(20f);

			/// BUILD
			if (GUILayout.Button(getBuildLabel(), HelperGui.gButtonBig))
			{
				new BuildExecutor().launch();
			}

		}

		void drawPrevisualization()
		{
			if (BuildorVars.PreIncVersion)
			{
				if (aProfil.versionInternal != null) GUILayout.Label("+ v.internal : " + aProfil.versionInternal.version);
				if (aProfil.versionPublish != null) GUILayout.Label("+ v.publish : " + aProfil.versionPublish.version);
			}

			GUILayout.Label("+ path : " + aProfil.FullPath);

			DataBuildorScenesMerger merger = aProfil.build.merger;
			if (merger != null) GUILayout.Label("+ merger : " + merger.strOneLine());

			ProfilLogLevels logs = aProfil.GetLogsLevels(BuildorVars.TargetDebug);
			if (logs != null) GUILayout.Label("+ logs : " + logs.ToString());

		}
	}

}

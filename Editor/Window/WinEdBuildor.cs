using UnityEngine;
using UnityEditor;

using fwp.logs;
using fwp.symbols;

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

				drawMergers();

				drawModules();

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
			if (aProfil != null)
			{
				Debug.Log("*new* " + aProfil, aProfil);
				aProfil.injectProfilToEditor();
			}
		}

		void drawProfilSelector()
		{
			GUILayout.BeginHorizontal();

			HelperGuiFields.drawPrefEnum<TargetPublish>(BuildorVars.ppref_publish, "publish", (value) =>
			{
				onProfilRefresh();
			});

			HelperGuiFields.drawPrefEnum<TargetSdks>(BuildorVars.ppref_sdk, "sdk", (value) =>
			{
				onProfilRefresh();
			});

			HelperGuiFields.drawPrefEnum<TargetDebug>(BuildorVars.ppref_debug, "debug", (value) =>
			{
				onProfilRefresh();
			});

			GUILayout.EndHorizontal();

			if (aProfil == null)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("no active profil");
				if (GUILayout.Button("open bridge"))
				{
					Selection.activeObject = BuildorHelpers.getScriptableDataBuildSettings();
				}
				GUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.BeginHorizontal();

				GUILayout.Label(Application.platform.ToString());

				GUI.enabled = false;
				EditorGUILayout.ObjectField(aProfil, typeof(DataBuildSettingProfile), true);
				GUI.enabled = true;
				if (GUILayout.Button("refresh")) onProfilRefresh();
				// if(GUILayout.Button("editor")) aProfil?.editor.apply();
				if (GUILayout.Button(">>", GUILayout.Width(40f))) Selection.activeObject = aProfil;
				GUILayout.EndHorizontal();
			}
		}

		void drawSymbols()
		{
			GUILayout.Label("Symbols", HelperGui.gCategoryBold);

			var p = BuildorVars.Profile;

			// profil.build symbols
			HelperGuiFields.drawField("symbols.build", BuildorHelpers.formatSymbols(p.build.symbols));

			// profil.logs.symbols
			var logs = p.GetLogsLevels(BuildorVars.TargetDebug);
			if (logs != null)
			{
				// HelperGuiFields.drawObjectDisabled(logs);
				HelperGuiFields.drawField("symbols.logs", BuildorHelpers.formatSymbols(logs.symbolsVerbose));
			}

			// profil.build.features

			HelperGuiFields.drawField("symbols.features", p.build.SymbolsFeatures);

			foreach (TargetFeatures f in System.Enum.GetValues(typeof(TargetFeatures)))
			{
				if (f == TargetFeatures.none) continue;
				bool toggled = HelperGuiFields.drawToggle(p.build.features.HasFlag(f), f.ToString());
				if (toggled) p.build.features |= f;
				else p.build.features &= ~f;
			}

			GUILayout.Space(10f);

			// current unity context value
			string sCurrent = ScriptSymbolsView.getPlayerSetSymbols(p.getBuildTargetGroup());
			HelperGuiFields.drawField("unity.settings", sCurrent);

			// wrap test
			// HelperGuiFields.drawLabel("IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;");

			GUILayout.BeginHorizontal();
			string symbs = p.Symbols;

			if (string.IsNullOrEmpty(symbs)) GUILayout.Label("no symbols to inject");
			else HelperGuiFields.drawField("inject", symbs);

			if (GUILayout.Button("apply", HelperGui.bS))
			{
				Debug.LogWarning("apply.symbols: " + symbs, p);
				ScriptSymbolsView.applyEditor(p.getBuildTargetGroup(), symbs);
			}

			GUILayout.EndHorizontal();
		}

		void drawLogs()
		{
			GUILayout.Label("Logs", HelperGui.gCategoryBold);
			var p = BuildorVars.Profile;

			drawLog(p.GetLogsLevels(BuildorVars.TargetDebug), "inject");
			drawLog(p.editor.logLevels, "editor");
		}

		void drawLog(ProfilLogLevels log, string context)
		{
			if (log == null) return;

			GUILayout.BeginHorizontal();
			GUILayout.Label(context);
			// GUILayout.BeginVertical();
			HelperGuiFields.drawObjectDisabled(log);
			if (GUILayout.Button("apply", HelperGui.bM)) log.applyLogs();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			foreach (var lvl in log.levels) GUILayout.Label(lvl.stringify());
			GUILayout.EndHorizontal();
			// GUILayout.EndVertical();
		}



		void drawModules()
		{
			if (aProfil.build == null) return;

			GUILayout.Label("Modules", HelperGui.gCategoryBold);

			drawModules(aProfil.build.profilModules);
			drawModules(aProfil.build.buildModules);

		}

		void drawModules(BuildModule[] mods)
		{
			GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
			{
				normal = { textColor = new Color(0.5f, 0.5f, 0.5f) },
				padding = new RectOffset(4, 0, 0, 2),
			};

			if (mods == null || mods.Length <= 0)
			{
				GUILayout.Label("-empty-");
				return;
			}
			foreach (var m in mods)
			{
				GUILayout.BeginHorizontal();
				// GUILayout.Label(m.name, GUILayout.Width(200f));
				GUI.enabled = false;
				var ret = (BuildModule)EditorGUILayout.ObjectField(m, typeof(BuildModule), false);
				GUI.enabled = true;
				if (ret != null)
				{
					if (GUILayout.Button("apply", HelperGui.bXS)) m.Apply();
				}
				if (GUILayout.Button(">>", HelperGui.bXS))
				{
					UnityEditor.Selection.activeObject = ret;
				}
				GUILayout.EndHorizontal();

				var rect = EditorGUILayout.GetControlRect(false, EditorStyles.miniLabel.lineHeight + 2);
				EditorGUI.LabelField(rect, m.strOneLine(), style);
			}
		}

		bool foldMerger = false;
		void drawMergers()
		{
			GUILayout.Label("Mergers", HelperGui.gCategoryBold);
			if (aProfil.build.Merger == null) GUILayout.Label("no merger set");
			else
			{
				var merg = aProfil.build.Merger;

				GUILayout.BeginHorizontal();
				GUILayout.Label("merger.build");
				HelperGuiFields.drawObjectDisabled(merg);
				if (GUILayout.Button("apply", HelperGui.bM)) merg.Apply();
				GUILayout.EndHorizontal();
				foldMerger = EditorGUILayout.Foldout(foldMerger, merg.strOneLine(), true);
				if (foldMerger) GUILayout.Label(merg.stringify());
			}
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

			HelperGuiFields.editText(BuildorVars.pref_suffix, "suffix");
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

			GUILayout.BeginHorizontal(GUILayout.Height(20f));
			HelperGuiFields.drawPrefToggle(BuildorVars.ppref_pre_incVersion, "version.incr");
			HelperGuiFields.drawPrefToggle(BuildorVars.ppref_post_openFolder, "open folder");
			HelperGuiFields.drawPrefToggle(BuildorVars.ppref_post_zip, "zip folder");
			HelperGuiFields.drawPrefToggle(BuildorVars.ppref_post_autorun, "autorun");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			HelperGuiFields.drawCopyPastablePath("app subpath : ", aProfil.getRelativeBuildFolderPath(), false);
			HelperGuiFields.drawCopyPastablePath("app name :", aProfil.getAppName(), false);
			GUILayout.EndHorizontal();

			HelperGuiFields.drawCopyPastablePath("path.build :", aProfil.BuildPath);
			HelperGuiFields.drawCopyPastablePath("path: ", aProfil.FullPath);

			HelperGuiFields.drawCopyPastablePath("zip: ", aProfil.ZipFullPath);

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

			if (GUILayout.Button("zip"))
			{
				BuildPostprocess.zipBuildFolder(aProfil.BuildPath, aProfil.ZipFullPath); // window button
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
				if (aProfil.versionInternal != null) GUILayout.Label("+ v.internal");
				if (aProfil.versionPublish != null) GUILayout.Label("+ v.publish");
			}

			GUILayout.Label("+ path : " + aProfil.FullPath);

			foreach (var mod in aProfil.build.profilModules) GUILayout.Label("+ " + mod.strOneLine());
			foreach (var mod in aProfil.build.buildModules) GUILayout.Label("+ " + mod.strOneLine());


			ProfilLogLevels logs = aProfil.GetLogsLevels(BuildorVars.TargetDebug);
			if (logs != null) GUILayout.Label("+ logs : " + logs.ToString());

		}
	}

}

using UnityEngine;
using UnityEditor;

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

			drawProfilSelector();

			_scroll = GUILayout.BeginScrollView(_scroll);

			if (aProfil != null)
			{
				subVersion?.draw(this);

				drawMergers();

				drawProfilModules();

				drawSymbols();

				if (aProfil != null && BuildorVars.IsDebug)
				{
					aProfil.debug.drawEd();
				}

				drawPathModifiers();
				drawPreprocessVars();
			}

			GUILayout.EndScrollView();

			drawBuildButton();
		}

		void onProfilRefresh()
		{
			aProfil = BuildorVars.Profile;
			if (aProfil == null)
			{
				Debug.LogWarning("(refresh) no active profil");
				return;
			}

			Debug.Log("*new* " + aProfil, aProfil);
			// aProfil.injectProfilToEditor();
			fwp.configs.ConfigDemo.Instance.Usage = aProfil.publish == TargetPublish.demo;
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
					Selection.activeObject = BuildorHelpers.GetBridge();
					if (Selection.activeObject == null) Debug.LogError("no bridge ?");
				}
				GUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.BeginHorizontal();
				GUI.enabled = false;
				EditorGUILayout.ObjectField(aProfil, typeof(DataBuildSettingProfile), true);
				GUI.enabled = true;
				if (GUILayout.Button("refresh", HelperGui.bS)) onProfilRefresh();
				if (aProfil != null && GUILayout.Button("apply", HelperGui.bS)) aProfil.applyProfilToEditor();
				if (GUILayout.Button(">>", HelperGui.bS)) Selection.activeObject = aProfil;
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("unity.platform", GUILayout.Width(100f));
			GUILayout.Label(Application.platform.ToString());
			GUILayout.Label("unity.build.target", GUILayout.Width(100f));
			GUILayout.Label(EditorUserBuildSettings.activeBuildTarget.ToString());

			GUILayout.EndHorizontal();
		}

		void drawSymbols()
		{
			GUILayout.Label("Symbols", HelperGui.gCategoryBold);

			var p = BuildorVars.Profile;

			// profil.build symbols
			HelperGuiFields.drawField("symbols.build", p.build.Symbols?.Symbols);
			HelperGuiFields.drawField("symbols.debug", p.debug.Symbols?.Symbols);

			// profil.logs.symbols
			/*
			if (p.Logs != null)
			{
				// HelperGuiFields.drawObjectDisabled(logs);
				HelperGuiFields.drawField("symbols.logs", BuildorHelpers.formatSymbols(p.Logs.symbolsVerbose));
			}
			*/

			// profil.build.features
			/*
			HelperGuiFields.drawField("symbols.features", p.build.SymbolsFeatures);

			foreach (TargetFeatures f in System.Enum.GetValues(typeof(TargetFeatures)))
			{
				if (f == TargetFeatures.none) continue;
				bool toggled = HelperGuiFields.drawToggle(p.build.features.HasFlag(f), f.ToString());
				if (toggled) p.build.features |= f;
				else p.build.features &= ~f;
			}

			*/

			GUILayout.Space(10f);

			GUILayout.BeginHorizontal();
			// current unity context value
			string sCurrent = ScriptSymbolsView.getPlayerSetSymbols(p.getBuildTargetGroup());
			HelperGuiFields.drawField("unity.settings", sCurrent);
			if (GUILayout.Button("playersetts", HelperGui.bS))
			{
				SettingsService.OpenProjectSettings("Project/Player");
			}
			GUILayout.EndHorizontal();

			// wrap test
			// HelperGuiFields.drawLabel("IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;IOAHFOADOIA;");

			GUILayout.BeginHorizontal();
			string symbs = p.Symbols;

			if (string.IsNullOrEmpty(symbs)) GUILayout.Label("no symbols to inject");
			else HelperGuiFields.drawField("inject", symbs);

			if (GUILayout.Button("inject", HelperGui.bS))
			{
				Debug.LogWarning("apply.symbols: " + symbs, p);
				ScriptSymbolsView.setPlayerSymbols(p.getBuildTargetGroup(), symbs);
			}

			GUILayout.EndHorizontal();
		}


		void drawProfilModules()
		{
			if (aProfil.build == null) return;

			GUILayout.Label("Modules", HelperGui.gCategoryBold);

			GUILayout.Label("modules.build");
			drawModules(aProfil.build.modules);

			if (BuildorVars.IsDebug)
			{
				GUILayout.Label("modules.debug");
				drawModules(aProfil.debug.modules);
			}
		}

		void drawModules(BuildModule[] mods)
		{
			if (mods == null || mods.Length <= 0)
			{
				GUILayout.Label("-none-");
				return;
			}

			GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
			{
				normal = { textColor = new Color(0.5f, 0.5f, 0.5f) },
				padding = new RectOffset(4, 0, 0, 2),
			};

			foreach (var m in mods)
			{
				if (m == null) continue;

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
			var merg = aProfil.build.merger;

			GUILayout.Label("Mergers", HelperGui.gCategoryBold);
			if (merg == null)
			{
				GUILayout.Label("-none-");
				return;
			}

			// draw object line
			GUILayout.BeginHorizontal();
			GUILayout.Label("merger.build");
			HelperGuiFields.drawObjectDisabled(merg);
			if (GUILayout.Button("apply", HelperGui.bM)) merg.Apply();
			GUILayout.EndHorizontal();

			// see more details
			foldMerger = EditorGUILayout.Foldout(foldMerger, merg.strOneLine(), true);
			if (foldMerger) GUILayout.Label(merg.stringify());
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

			aProfil.build.drawFolderSelector();
			aProfil.debug.drawFolderSelector();
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


		/// <summary>
		/// sum up
		/// </summary>
		void drawPrevisualization()
		{
			GUILayout.Label("Summary", HelperGui.gCategoryBold);

			if (aProfil == null)
			{
				GUILayout.Label("no active profil");
				return;
			}

			if (BuildorVars.PreIncVersion) GUILayout.Label(" + inc.version");

			// ProfilLogLevels logs = aProfil.Logs;
			// if (logs != null) GUILayout.Label("+ logs : " + logs.ToString());

			GUILayout.Label(aProfil.stringifySummary());
		}
	}

}


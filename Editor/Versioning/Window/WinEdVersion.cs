using UnityEngine;
using UnityEditor;

namespace fwp.version.editor
{
	using fwp.buildor;
	using fwp.buildor.editor;
	using fwp.version;

	/// <summary>
	/// versions of active profile (bridge, active build target, publish & sdk)
	/// no bridge or no active profile : all version scriptables, focused one has increment controls
	/// </summary>
	public class WinEdVersion : UnityEditor.EditorWindow
	{

		[MenuItem(BuildorVerbosity._buildor_menuitem_path + "version (win)", false, 10)]
		static void init()
		{
			var win = EditorWindow.GetWindow(typeof(WinEdVersion));
			win.titleContent = new GUIContent("Version");
		}

		WinSubVersion subVersion;

		DataBuildSettingProfile profile;

		/// <summary>
		/// why all versions are listed
		/// </summary>
		string fallbackReason;

		DataBuildSettingVersion[] versions;
		[SerializeField] DataBuildSettingVersion focused;

		Vector2 scroll;

		private void OnEnable()
		{
			if (subVersion == null) subVersion = new WinSubVersion();
			refresh();
		}

		// active build target, publish or sdk might have changed
		private void OnFocus() => refresh();

		void refresh()
		{
			profile = null;
			versions = null;

			var bridge = BuildorHelpers.GetBridge();
			if (bridge == null)
			{
				fallbackReason = "no bridge in project";
			}
			else
			{
				// not verbose : called on each focus
				profile = bridge.getPlatformProfil(BuildorVars.TargetPublish, BuildorVars.TargetSdk, false);
				fallbackReason = "no active profile : " + EditorUserBuildSettings.activeBuildTarget + " " + BuildorVars.TargetPublish + " " + BuildorVars.TargetSdk;
			}

			if (profile != null) return;

			versions = DataBuildSettingVersion.getScriptables();
			if (versions == null || versions.Length <= 0) return;

			if (focused == null || System.Array.IndexOf(versions, focused) < 0) focused = versions[0];
		}

		private void OnGUI()
		{
			if (GUILayout.Button("refresh")) refresh();

			scroll = GUILayout.BeginScrollView(scroll);

			if (profile != null) drawProfile();
			else drawAll();

			GUILayout.EndScrollView();
		}

		void drawProfile()
		{
			GUI.enabled = false;
			EditorGUILayout.ObjectField("active profile", profile, typeof(DataBuildSettingProfile), false);
			GUI.enabled = true;

			if (profile.versionInternal == null && profile.versionPublish == null)
			{
				EditorGUILayout.HelpBox("active profile has no version", MessageType.Warning);
				return;
			}

			if (profile.versionInternal != null) subVersion.drawVersion(profile.versionInternal);
			if (profile.versionPublish != null) subVersion.drawVersion(profile.versionPublish);
		}

		void drawAll()
		{
			EditorGUILayout.HelpBox(fallbackReason + "\nlisting all versions", MessageType.Info);

			if (versions == null || versions.Length <= 0)
			{
				GUILayout.Label("no version scriptable in project");
				return;
			}

			// selector
			foreach (var v in versions)
			{
				if (v == null) continue; // deleted since refresh

				bool selected = v == focused;
				if (GUILayout.Toggle(selected, v.name + "   " + v.getFormated(), EditorStyles.radioButton) && !selected)
				{
					focused = v;
				}
			}

			GUILayout.Space(10f);

			if (focused != null) subVersion.drawVersion(focused);
		}

	}

}

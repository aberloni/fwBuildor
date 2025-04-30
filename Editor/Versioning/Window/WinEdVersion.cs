using UnityEngine;
using UnityEditor;

namespace fwp.version.editor
{
	using fwp.buildor;
	using fwp.version;
	using System.Linq;

	public class WinEdVersion : UnityEditor.EditorWindow
	{

		[MenuItem(BuildorVerbosity._buildor_menuitem_path + "(win) version", false, 0)]
		static void init()
		{
			var win = EditorWindow.GetWindow(typeof(WinEdVersion));
			win.titleContent = new GUIContent("Version");
		}

		WinSubVersion subVersion;

		DataBuildSettingVersion[] versions;
		DataBuildSettingVersion version => versions.FirstOrDefault();

		private void OnEnable()
		{
			if (subVersion == null) subVersion = new WinSubVersion();
			if (versions == null) versions = DataBuildSettingVersion.getScriptables();
		}

		private void OnGUI()
		{
			if (versions == null || GUILayout.Button("fetch versions"))
			{
				versions = DataBuildSettingVersion.getScriptables();
			}

			if (subVersion != null)
			{
				subVersion.draw(version);
			}
		}

		public DataBuildSettingVersion getActiveVersion()
		{
			return DataBuildSettingVersion.getScriptable();
		}

	}

}
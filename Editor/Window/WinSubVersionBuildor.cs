using UnityEditor;
using UnityEngine;

namespace fwp.buildor.editor
{

	public class WinSubVersionBuildor : fwp.version.editor.WinSubVersion
	{
		public void draw(WinEdBuildor win)
		{
			GUILayout.Label("Version", HelperGui.gCategoryBold);

			var p = BuildorVars.Profile;
			drawVersion(p.versionInternal);
			drawVersion(p.versionPublish);

			GUILayout.Label("unity.player.settings: " + fwp.version.VersionManager.getPlayerSettingsVersion());
		}
	}

}
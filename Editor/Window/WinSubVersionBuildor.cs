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

			if (p.versionInternal != null) drawVersion(p.versionInternal);
			if (p.versionPublish != null) drawVersion(p.versionPublish);

			GUILayout.Label("unity.player.settings: " + fwp.version.VersionManager.getPlayerSettingsVersion());
		}
	}

}
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

			// FIX only : MAJOR / MINOR in version window
			if (p.versionInternal != null) drawVersion(p.versionInternal, fixOnly: true);
			if (p.versionPublish != null) drawVersion(p.versionPublish, fixOnly: true);

			GUILayout.Label("unity.player.settings: " + fwp.version.VersionManager.getPlayerSettingsVersion());
		}
	}

}
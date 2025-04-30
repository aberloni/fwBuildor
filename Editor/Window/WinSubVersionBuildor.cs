using UnityEngine;

using fwp.version.editor;
using fwp.version;
using fwp.buildor.editor;

public class WinSubVersionBuildor : fwp.version.editor.WinSubVersion
{
	public void draw(WinEdBuildor win)
	{
		BuildSettingVersionType vType = BuildorWinEdHelper.drawEnum<BuildSettingVersionType>("publish type", "publish", 0);
		win.parameters.buildFlags.isPublishingBuild = vType == BuildSettingVersionType.vPublish;

		draw(win.getActiveVersion());
	}
}

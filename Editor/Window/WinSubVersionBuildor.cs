using fwp.version;
using fwp.buildor.editor;

public class WinSubVersionBuildor : fwp.version.editor.WinSubVersion
{
	public void draw(WinEdBuildor win)
	{
		draw(win.getActiveVersion());
	}
}

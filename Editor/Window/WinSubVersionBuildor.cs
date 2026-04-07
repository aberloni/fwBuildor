namespace fwp.buildor.editor
{

	public class WinSubVersionBuildor : fwp.version.editor.WinSubVersion
	{
		public void draw(WinEdBuildor win)
		{
			draw(BuildorHelpers.Profile.Version);
		}
	}
	
}
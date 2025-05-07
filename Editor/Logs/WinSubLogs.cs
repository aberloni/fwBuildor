using UnityEngine;

namespace fwp.logs.editor
{
	using fwp.buildor.editor;
	//using fwp.buildor;

	public class WinSubLogs : WinSubFieldApply<ProfilLogLevels>
	{
		public WinSubLogs(WinEdBuildor win) : base(win)
		{
		}

		protected override string getSectionTitle() => "Logs";

		protected override ProfilLogLevels fetchProfilInstance()
		{
			return win.activeProfil.logLevels;
		}

		protected override void applyEditor(ProfilLogLevels value)
		{
			value?.apply();
		}

		protected override void drawDetails(ProfilLogLevels value)
		{
			foreach (var lvl in value.levels)
			{
				GUILayout.Label(lvl.stringify());
			}

		}
	}
}
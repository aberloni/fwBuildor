using UnityEngine;

namespace fwp.logs.editor
{
    using fwp.buildor;
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
			var profil = BuildorHelpers.Profile;
			if (profil == null) return null;
			if (profil.build == null) return null;
			return profil.build.logLevels;
		}

		protected override void applyEditor(ProfilLogLevels value)
		{
			value?.applyLogs();
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
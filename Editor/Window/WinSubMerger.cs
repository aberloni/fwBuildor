using UnityEngine;

namespace fwp.buildor.editor
{
    public class WinSubMerger : WinSubFieldApply<DataBuildorScenesMerger>
    {

        public WinSubMerger(WinEdBuildor win) : base(win)
        {
        }

        protected override string getSectionTitle() => "Merger";

		protected override DataBuildorScenesMerger fetchProfilInstance()
		{
            return win.activeProfil.merger;
		}

		/// <summary>
		/// click apply in editor
		/// </summary>
		protected override void applyEditor(DataBuildorScenesMerger value)
        {
            value.apply();
        }

		protected override void drawHeader(DataBuildorScenesMerger value)
		{
            GUILayout.Label(value.strOneLine());
		}

		protected override void drawDetails(DataBuildorScenesMerger value)
        {
			GUILayout.Label(value.stringify());

            /*
            GUILayout.Label("editor Build Settings scenes");
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var sc = EditorBuildSettings.scenes[i];
                GUILayout.Label($"#{i}  {sc.path}");
            }
            */
        }

    }

}

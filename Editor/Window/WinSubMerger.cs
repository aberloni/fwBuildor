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
            var profil = WinEdBuildor.Profile;
            if (profil != null) return profil.build.merger;
            return null;
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
        }

    }

}

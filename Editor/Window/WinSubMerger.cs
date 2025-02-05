using UnityEngine;

using UnityEditor;

namespace fwp.buildor.editor
{
    public class WinSubMerger : WinSubFieldApply<DataBuildorScenesMerger>
    {

        public WinSubMerger(WinEdBuildor win) : base(win)
        {
        }

        protected override string getSectionTitle() => "override merger";
        protected override string pprefUid() => "buildor.merger";

        protected override void apply()
        {
            base.apply();

            value?.apply();
        }

        protected override void drawContent()
        {
            base.drawContent();
            if (win.activeProfil.merger != null)
            {
                GUILayout.Label("active profil has merger : " + win.activeProfil.merger.name);
            }
        }

        protected override void drawDetails()
        {
            base.drawDetails();

            GUILayout.Label("merger : " + win.activeProfil.merger.strOneLine());

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var sc = EditorBuildSettings.scenes[i];
                GUILayout.Label($"#{i}  {sc.path}");
            }
        }

    }

}

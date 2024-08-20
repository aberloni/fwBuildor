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
            if(win.activeProfil.merger != null)
            {
                GUILayout.Label("active profil has merger : " + win.activeProfil.merger.strOneLine());
            }
        }

        protected override void drawDetails()
        {
            base.drawDetails();

            foreach (var sc in EditorBuildSettings.scenes)
            {
                GUILayout.Label(sc.path);
            }
        }

    }

}

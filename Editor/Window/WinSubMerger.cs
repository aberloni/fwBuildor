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

        /// <summary>
        /// click apply in editor
        /// </summary>
        protected override void apply()
        {
            base.apply();

            value?.apply();
        }

        protected override void drawContent()
        {
            base.drawContent();

            if (win.activeProfil.merger == null)
            {
                GUILayout.Label("profil.merger.empty");
            }
            else
            {
                GUILayout.Label("profil.merger is " + win.activeProfil.merger.name);
            }
        }

        protected override void drawDetails()
        {
            base.drawDetails();

            var activeMerger = win.getActiveMerger();
            if (activeMerger != null)
            {
                GUILayout.Label("build.merger will use " + activeMerger.strOneLine());
                GUILayout.Label(activeMerger.stringify());
            }
            else
            {
                GUILayout.Label("build.merger.empty");
            }

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

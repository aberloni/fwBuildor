using UnityEngine;

using UnityEditor;

namespace fwp.buildor.editor
{
    public class WinSubMerger : WinSubFieldApply<DataBuildorScenesMerger>
    {
        
        public WinSubMerger(WinEdBuildor win) : base(win)
        {
        }

        protected override string getSectionTitle() => "merger";
        protected override string pprefUid() => "buildor.merger";

        override public void fetchInstance()
        {
            base.fetchInstance();
            
            if (_value == null && win != null)
            {
                _value = win.activeProfil.merger;
            }
        }

        protected override void apply()
        {
            base.apply();

            value?.apply();
        }

        protected override void drawContent()
        {
            base.drawContent();
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

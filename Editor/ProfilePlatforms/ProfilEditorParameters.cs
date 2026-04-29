using UnityEngine;

namespace fwp.buildor.editor
{
    [System.Serializable]
    public class ProfilEditorParameters : ProfilParameter
    {
        public DataBuildorScenesMerger merger;
        public fwp.logs.ProfilLogLevels logLevels;

        override public void applyProfil()
        {
            base.applyProfil();
            merger?.Apply();
            logLevels?.apply();
        }
    }

}

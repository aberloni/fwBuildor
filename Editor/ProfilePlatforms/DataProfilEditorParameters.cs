using UnityEngine;

namespace fwp.buildor.editor
{
    [System.Serializable]
    public class DataProfilEditorParameters
    {
        public DataBuildorScenesMerger merger;
        public fwp.logs.ProfilLogLevels logLevels;

        public void apply()
        {
            merger?.Apply();
            logLevels?.apply();
        }
    }

}

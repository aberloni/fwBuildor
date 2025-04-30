using System.Collections.Generic;
using UnityEngine;


namespace fwp.version
{
    [CreateAssetMenu(menuName = "buildor/version/new OSX", order = 100)]
    public class DataVersionOsx : DataBuildSettingVersion
    {
        public override void applyVersionToEditor()
        {
#if UNITY_EDITOR
            //short version
            UnityEditor.PlayerSettings.bundleVersion = version;
#endif
        }
    }

}

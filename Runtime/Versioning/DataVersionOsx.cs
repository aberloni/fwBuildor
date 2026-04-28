using System.Collections.Generic;
using UnityEngine;


namespace fwp.version
{
    [CreateAssetMenu(menuName = "buildor/version/new OSX", order = 100)]
    public class DataVersionOsx : DataBuildSettingVersion
    {
        
#if UNITY_EDITOR
        public override void applyVersionToEditor()
        {
            //short version
            UnityEditor.PlayerSettings.bundleVersion = Version;
        }
#endif

    }

}

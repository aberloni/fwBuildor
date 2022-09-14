using System.Collections.Generic;
using UnityEngine;


namespace fwp.buildor.version
{
    [CreateAssetMenu(menuName = "buildor/version/new switch", order = 100)]
    public class DataVersionSwitch : DataBuildSettingVersion
    {
        public override void applyVersionToEditor()
        {
#if UNITY_EDITOR

            //num inc rom release inc number
            UnityEditor.PlayerSettings.Switch.releaseVersion = buildNumber.ToString();

            // ??
            UnityEditor.PlayerSettings.Switch.displayVersion = version;

            // global version
            UnityEditor.PlayerSettings.bundleVersion = version;
        
#endif
        }

    }

}

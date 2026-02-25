using UnityEngine;

namespace fwp.version
{
    [CreateAssetMenu(menuName = "buildor/version/+switch", order = 100)]
    public class DataVersionSwitch : DataBuildSettingVersion
    {
        
#if UNITY_EDITOR
        /// <summary>
        /// how to apply version number on switch
        /// </summary>
        public override void applyVersionToEditor()
        {
            // global version
            UnityEditor.PlayerSettings.bundleVersion = version;

#if UNITY_SWITCH && !UNITY_6000_0_OR_NEWER
            // before u6000
            // switch section only exist with installed package

            // num inc rom release inc number
            // must be numeric string
            UnityEditor.PlayerSettings.Switch.releaseVersion = buildNumber.ToString();

            // user visible version
            UnityEditor.PlayerSettings.Switch.displayVersion = version;
#endif
        }
#endif

    }

}

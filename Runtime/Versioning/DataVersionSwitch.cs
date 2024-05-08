using System.Collections.Generic;
using UnityEngine;


namespace fwp.buildor.version
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [CreateAssetMenu(menuName = "buildor/version/new switch", order = 100)]
    public class DataVersionSwitch : DataBuildSettingVersion
    {

        public override void applyVersionToEditor()
        {

#if UNITY_EDITOR && !UNITY_6000_0_OR_NEWER
            //num inc rom release inc number
            PlayerSettings.Switch.releaseVersion = buildNumber.ToString();

            // ??
            UnityEditor.PlayerSettings.Switch.displayVersion = version;
#endif

#if UNITY_EDITOR
            // global version
            UnityEditor.PlayerSettings.bundleVersion = version;
#endif

        }

        }

    }

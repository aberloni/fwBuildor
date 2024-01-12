using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>


namespace fwp.buildor
{
    [CreateAssetMenu(menuName = "buildor/new profil:ios", order = 100)]
    public class DataBuildSettingProfileIos : DataBuildSettingProfileMobile
    {

#if UNITY_EDITOR
        [Header("SDK")]
        public iOSTargetDevice target_device;
        public string iOSVersion = "9.0";
#endif

        public override string getExtension() => "";

        public override BuildTarget getPlatformTarget() => BuildTarget.iOS;
        public override BuildTargetGroup getPlatformTargetGroup() => BuildTargetGroup.iOS;

        static public string getPlayerSettingsBuildNumber() => PlayerSettings.iOS.buildNumber.ToString();
        static public string getPlayerSettingsVersion() => PlayerSettings.bundleVersion;
    }
}
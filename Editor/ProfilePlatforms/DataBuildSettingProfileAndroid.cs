using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// 
/// lvl 21 - android 5.0
/// lvl 23 - 6.0
/// lvl 24 - 7.0
/// lvl 26 - 8.0
/// </summary>


namespace fwp.buildor.editor
{
    [CreateAssetMenu(menuName = "buildor/new profil:android", order = 100)]
    public class DataBuildSettingProfileAndroid : DataBuildSettingProfileMobile
    {
        [Header("SDK")]
        public AndroidSdkVersions minSdk = AndroidSdkVersions.AndroidApiLevel30;

        public override string getExtension() => "apk";

        public override void applyProfilEditor(bool usePublishVersion = false)
        {
            base.applyProfilEditor(usePublishVersion);

            PlayerSettings.Android.minSdkVersion = minSdk;
            Debug.Log(" L android min sdk : " + minSdk);
        }

        public override BuildTarget getPlatformTarget() => BuildTarget.Android;
        public override BuildTargetGroup getPlatformTargetGroup() => BuildTargetGroup.Android;

        static public string getPlayerSettingsBuildNumber() => PlayerSettings.Android.bundleVersionCode.ToString();
        static public string getPlayerSettingsVersion() => PlayerSettings.bundleVersion;
    }
}
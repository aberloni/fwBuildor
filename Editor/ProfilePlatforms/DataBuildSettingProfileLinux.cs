using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>


namespace fwp.buildor
{
    /// <summary>
    /// not compatible before u2021
    /// </summary>
    [CreateAssetMenu(menuName = "buildor/new profil:linux", order = 100)]
    public class DataBuildSettingProfileLinux : DataBuildSettingProfile
    {
        public override string getExtension() => "x86_64";
        public override BuildTarget getPlatformTarget() => BuildTarget.StandaloneLinux64;
        public override BuildTargetGroup getPlatformTargetGroup()
        {
#if UNITY_2019 || UNITY_2020
            return BuildTargetGroup.Unknown;
#else
            return BuildTargetGroup.EmbeddedLinux;
#endif
        }

        static public string getPlayerSettingsBuildNumber() => "-";
        static public string getPlayerSettingsVersion() => PlayerSettings.bundleVersion;

    }
}
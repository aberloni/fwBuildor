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
    [CreateAssetMenu(menuName = "buildor/new profil:windows", order = 100)]
    public class DataBuildSettingProfileWindows : DataBuildSettingProfile
    {
        public override string getExtension() => "exe";
        public override BuildTarget getPlatformTarget() => BuildTarget.StandaloneWindows;

        protected override void OnValidate()
        {
            base.OnValidate();

        }

        public override string getBasePath()
        {
            string basePath = base.getBasePath();

            basePath = Path.Combine(basePath, getBuildNameVersion());

            return basePath;
        }

        static public string getPlayerSettingsBuildNumber() => "-";
        static public string getPlayerSettingsVersion() => PlayerSettings.bundleVersion;

    }
}
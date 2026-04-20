using UnityEngine;
using UnityEditor;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>


namespace fwp.buildor.editor
{
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "profils/+windows", order = 100)]
    public class DataBuildSettingProfileWindows : DataBuildSettingProfile
    {
        public override string getExtension() => "exe";
        public override BuildTarget getPlatformTarget() => BuildTarget.StandaloneWindows;
        public override BuildTargetGroup getBuildTargetGroup() => BuildTargetGroup.Standalone;

        static public string getPlayerSettingsBuildNumber() => "-";
        static public string getPlayerSettingsVersion() => PlayerSettings.bundleVersion;
    }
}
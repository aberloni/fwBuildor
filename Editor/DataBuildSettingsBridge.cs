using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

using fwp.buildor.version;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace fwp.buildor.editor
{


    [CreateAssetMenu(menuName = "buildor/new brdge settings", order = 100)]
    public class DataBuildSettingsBridge : ScriptableObject
    {
        [Header("desktop")]
        public DataBuildSettingProfile[] windows;
        public DataBuildSettingProfile[] linux;
        public DataBuildSettingProfile[] osx;

        [Header("mobile")]
        public DataBuildSettingProfile[] android;
        public DataBuildSettingProfile[] ios;

        [Header("console")]
        public DataBuildSettingProfile[] ninSwitch;

        public DataBuildSettingProfile getPlatformProfil(PublishLevel tarState = PublishLevel.intern)
        {
            var target = EditorUserBuildSettings.activeBuildTarget;

            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                    return windows.FirstOrDefault(x => x.releaseLevel == tarState);
                case BuildTarget.StandaloneOSX:
                    return osx.FirstOrDefault(x => x.releaseLevel == tarState);
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.EmbeddedLinux:
                    return linux.FirstOrDefault(x => x.releaseLevel == tarState);

                case BuildTarget.iOS:
                    return ios.FirstOrDefault(x => x.releaseLevel == tarState);
                case BuildTarget.Android:
                    return android.FirstOrDefault(x => x.releaseLevel == tarState);

                case BuildTarget.Switch:
                    return ninSwitch.FirstOrDefault(x => x.releaseLevel == tarState);

                case BuildTarget.NoTarget:
                default:
                    Debug.LogWarning(" ? active build target is : NoTarget");
                    break;
            }

            Debug.LogWarning("no profil possible for " + tarState);
            return null;
        }

    }
}
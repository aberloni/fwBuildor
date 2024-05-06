using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        public enum BuildorBuildState
        {
            RELEASE,DEBUG,FEST
        }

        [Header("desktop")]
        public DataBuildSettingsBridgeCouple windows;
        public DataBuildSettingsBridgeCouple osx;
        public DataBuildSettingsBridgeCouple linux;

        [Header("mobile")]
        public DataBuildSettingsBridgeCouple android;
        public DataBuildSettingsBridgeCouple ios;
        
        [Header("console")]
        public DataBuildSettingsBridgeCouple switcheu;
        
        public DataBuildSettingProfile getPlatformProfil(BuildorBuildState? tarState = null)
        {
            BuildTarget target = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

            DataBuildSettingsBridgeCouple couple = new DataBuildSettingsBridgeCouple();

            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                    couple = windows;
                    break;
                case BuildTarget.iOS:
                    couple = ios;
                    break;
                case BuildTarget.Android:
                    couple = android;
                    break;
                case BuildTarget.StandaloneLinux64:
                    couple = linux;
                    break;
                case BuildTarget.Switch:
                    couple = switcheu;
                    break;
                case BuildTarget.StandaloneOSX:
                    couple = osx;
                    break;
                case BuildTarget.NoTarget:
                default:
                    Debug.LogError("not implem for " + target);
                    break;
            }

            return couple.getActive(tarState);
        }

    }

    /// <summary>
    /// default will be release
    /// </summary>
    [System.Serializable]
    public struct DataBuildSettingsBridgeCouple
    {
        public DataBuildSettingProfile release;
        public DataBuildSettingProfile festival;
        public DataBuildSettingProfile debug;

        public DataBuildSettingProfile getActive(DataBuildSettingsBridge.BuildorBuildState? tarState)
        {
            if(tarState != null)
            {
                switch (tarState)
                {
                    case DataBuildSettingsBridge.BuildorBuildState.DEBUG:       return debug;
                    case DataBuildSettingsBridge.BuildorBuildState.RELEASE:     return release;
                    case DataBuildSettingsBridge.BuildorBuildState.FEST:        return festival;
                    default: Debug.LogError("nop"); break;
                }
            }

#if debug
            return debug;
#elif fest
            return festival;
#else
            return release;
#endif

        }

    }
}
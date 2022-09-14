using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace buildor
{


    [CreateAssetMenu(menuName = "buildor/new brdge settings", order = 100)]
    public class DataBuildSettingsBridge : ScriptableObject
    {
        public enum BuildorBuildState
        {
            RELEASE,DEBUG,FEST
        }

        public DataBuildSettingsBridgeCouple windows;
        public DataBuildSettingsBridgeCouple android;
        public DataBuildSettingsBridgeCouple ios;
        public DataBuildSettingsBridgeCouple osx;
        public DataBuildSettingsBridgeCouple switcheu;

        public DataBuildSettingProfile getPlatformProfil(BuildorBuildState? tarState = null)
        {
            BuildTarget target = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

            DataBuildSettingsBridgeCouple couple;

            if (target == BuildTarget.iOS)
            {
                couple = ios;
            } 
            else if (target == BuildTarget.Android)
            {
                couple = android;
            }
            else if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                couple = windows;
            }
            else if (target == BuildTarget.Switch)
            {
                couple = switcheu;
            }
            else if (target == BuildTarget.StandaloneOSX)
            {
                couple = osx;
            }
            else
            {
                Debug.LogError("not implem for " + target);
                return null;
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
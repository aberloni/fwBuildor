using UnityEngine;
using fwp.logs;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace fwp.buildor.editor
{
    [CreateAssetMenu(menuName = "buildor/profil/+debug.params", order = 100)]
    public class DataProfilDebugParameters : ScriptableObject
    {
        public enum ProfilingLevel
        {
            none,
            profiling,
            deep,
        }
        
        [Tooltip("unity dev build ticked")]
        public bool developement_build = false; // match : EditorUserBuildSettings.development

        [Tooltip("activate all profiler stuff")]
        public bool debugScripting = false;

        public ProfilingLevel debugProfiling = ProfilingLevel.none;
        
        public ProfilLogLevels logLevels;   
    }

}

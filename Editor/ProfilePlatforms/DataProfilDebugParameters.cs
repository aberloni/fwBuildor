using UnityEngine;
using UnityEditor;
using fwp.logs;
using System.Linq;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace fwp.buildor.editor
{
    // [CreateAssetMenu(menuName = "buildor/profil/+debug.params", order = 100)]
    [System.Serializable]
    public class DataProfilDebugParameters
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
        
        public BuildModule[] modules;
        public ProfilLogLevels Logs => modules.OfType<ProfilLogLevels>().FirstOrDefault();

        void clear()
        {
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.allowDebugging = false;
            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
        }

        public void apply()
        {
            if(!BuildorVars.IsDebug)
            {
                clear();
                return;
            }

            EditorUserBuildSettings.development = developement_build;
            EditorUserBuildSettings.allowDebugging = debugScripting;

            switch (debugProfiling)
            {
                case DataProfilDebugParameters.ProfilingLevel.deep:
                    EditorUserBuildSettings.connectProfiler = true;
                    EditorUserBuildSettings.buildWithDeepProfilingSupport = true;
                    break;
                case DataProfilDebugParameters.ProfilingLevel.profiling:
                    EditorUserBuildSettings.connectProfiler = true;
                    EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
                    break;
                default:
                    EditorUserBuildSettings.connectProfiler = false;
                    EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
                    break;
            }
        }

        public void log()
        {
            Debug.Log(" L dev build? " + EditorUserBuildSettings.development);
            Debug.Log(" L debugging? " + EditorUserBuildSettings.allowDebugging);
            Debug.Log(" L profiler? " + EditorUserBuildSettings.connectProfiler);
            Debug.Log(" L profiler.deep? " + EditorUserBuildSettings.buildWithDeepProfilingSupport);
        }

        public void drawEd()
        {
            GUILayout.Label("Debug", HelperGui.gCategoryBold);

            developement_build = GUILayout.Toggle(developement_build, "dev build");
            if (developement_build != UnityEditor.EditorUserBuildSettings.development)
            {
                EditorUserBuildSettings.development = developement_build;
                Debug.LogWarning("changed dev build : " + EditorUserBuildSettings.development);

                // UnityEditor.EditorUtility.SetDirty(this);
            }

            debugScripting = GUILayout.Toggle(debugScripting, "debug scripting");
            if (debugScripting != EditorUserBuildSettings.allowDebugging)
            {
                EditorUserBuildSettings.allowDebugging = debugScripting;
                // UnityEditor.EditorUtility.SetDirty(this);
            }

            var level = (DataProfilDebugParameters.ProfilingLevel)EditorGUILayout.EnumPopup("profiling", debugProfiling);

            if (level != debugProfiling)
            {
                switch (level)
                {
                    case DataProfilDebugParameters.ProfilingLevel.deep:
                        EditorUserBuildSettings.connectProfiler = true;
                        EditorUserBuildSettings.buildWithDeepProfilingSupport = true;
                        break;
                    case DataProfilDebugParameters.ProfilingLevel.profiling:
                        EditorUserBuildSettings.connectProfiler = true;
                        EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
                        break;
                    default:
                        EditorUserBuildSettings.connectProfiler = false;
                        EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
                        break;

                }

                debugProfiling = level;
                // UnityEditor.EditorUtility.SetDirty(this);
            }
        }
    }

}

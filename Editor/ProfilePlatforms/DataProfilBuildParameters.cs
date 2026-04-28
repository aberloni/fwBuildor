using System.Linq;
using fwp.logs;
using UnityEditor;
using UnityEngine;

namespace fwp.buildor.editor
{
    [System.Serializable]
    public class DataProfilBuildParameters
    {
        public DataBuildorScenesMerger Merger => modules?.OfType<DataBuildorScenesMerger>().FirstOrDefault();
        public ProfilLogLevels Logs => modules?.OfType<ProfilLogLevels>().FirstOrDefault();

        /// <summary>
        /// sdks, lang_en, ...
        /// </summary>
        public string[] symbols = new string[0];

        public TargetFeatures features;

        public string SymbolsFeatures
        {
            get
            {
                string ret = string.Empty;
                foreach (TargetFeatures f in System.Enum.GetValues(typeof(TargetFeatures)))
                {
                    if (f == TargetFeatures.none) continue;
                    if (features.HasFlag(f)) ret += f.ToString() + ";";
                }
                return ret;
            }
        }

        public DataBuildorScenesMerger merger;
        public ProfilLogLevels logs;

        public BuildModule[] modules = new BuildModule[0];

        [Header("post process")]
        [Tooltip("remove any folder from buidl matching given pattern(s)")]
        public string[] clearFolders = new string[0];

        [Tooltip("project name used to generate output file")]
        public string build_prefix;

        [Tooltip("meant for override, folder where build is located each time, build name is then static")]
        public string build_folder_specific;
        public bool HasSpecificFolder => !string.IsNullOrEmpty(build_folder_specific);

        [Header("bundle")]
        public Sprite splashscreen;
        public Texture2D icon;

        public void ApplyModules()
        {
            if (modules == null) return;

            if (merger != null) merger.Apply();
            if (logs != null) logs.Apply();

            foreach (var m in modules)
            {
                if (m == null) continue;

                if (EditorUtility.DisplayDialog("module", "apply module: " + m.GetType() + "." + m.name + " ?", "yes", "no"))
                {
                    m.Apply();
                }
            }
        }
    }

}

using System.Linq;
using UnityEngine;

namespace fwp.buildor.editor
{
    [System.Serializable]
    public class DataProfilBuildParameters
    {
        public DataBuildorScenesMerger Merger => profilModules.OfType<DataBuildorScenesMerger>().FirstOrDefault();

        public fwp.logs.ProfilLogLevels logLevels;

        /// <summary>
        /// sdks, lang_en, ...
        /// </summary>
        public string[] symbols;

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

        /// <summary>
        /// universal module, applied with profil
        /// </summary>
        public BuildModule[] profilModules = new BuildModule[0];

        /// <summary>
        /// apply only when actually building
        /// </summary>
        public BuildModule[] buildModules = new BuildModule[0];

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

        public void Apply()
        {
            foreach (var m in profilModules) m?.Apply();
            logLevels?.applyLogs();
        }
    }

}

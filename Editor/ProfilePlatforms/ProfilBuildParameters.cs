using System.Linq;
using UnityEditor;
using UnityEngine;

namespace fwp.buildor.editor
{
    [System.Serializable]
    public class ProfilBuildParameters : ProfilParameter
    {
        /// <summary>
        /// enum flag
        /// </summary>
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

        public BoduleSymbols Symbols => modules.OfType<BoduleSymbols>().FirstOrDefault();

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

        public override void applyProfil()
        {
            base.applyProfil();

            merger?.Apply();
        }

    }

}

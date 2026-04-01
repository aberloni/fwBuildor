using UnityEngine;

namespace fwp.buildor.editor
{
    [CreateAssetMenu(menuName = "buildor/profil/+build.params", order = 100)]
    public class DataProfilBuildParameters : ScriptableObject
    {
        [Tooltip("project name used to generate output file")]
        public string build_prefix = "";

        [Tooltip("meant for override, folder where build is located each time, build name is then static")]
        public string build_folder_specific = "";

        public Sprite splashscreen;
        public Texture2D icon;

        public DataBuildorScenesMerger merger;
        public fwp.symbols.ScriptableSymbolProfil scriptSymbols;

    }

}

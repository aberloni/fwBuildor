using UnityEngine;

namespace fwp.buildor.editor
{
    [CreateAssetMenu(menuName = "buildor/profil/+build.params", order = 100)]
    public class DataProfilBuildParameters : ScriptableObject
    {
        [Tooltip("remove any folder from buidl matching given pattern(s)")]
        public string[] clearFolders = new string[0];

        [Tooltip("project name used to generate output file")]
        public string build_prefix = "";

        [Tooltip("meant for override, folder where build is located each time, build name is then static")]
        public string build_folder_specific = "";

        public Sprite splashscreen;
        public Texture2D icon;

        public DataBuildorScenesMerger merger;
        public fwp.symbols.editor.ScriptableSymbolProfil scriptSymbols;

    }

}

using UnityEngine;
using UnityEditor;
using System.IO;
using fwp.buildor.version;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace fwp.buildor.editor
{
    /// <summary>
    ///  ALL DATA contains into those files won't be usable in build
    ///  it's meant to be used as a build flow tool params
    ///  
    /// .../builds/.../prefix_X-Y-Z.app
    /// .../builds/.../Y-m-d/...
    /// </summary>
    abstract public class DataBuildSettingProfile : ScriptableObject
    {
        const string build_path = "builds/"; // next to Assets/
        const string path_separator = "__";

        [Header("version")]
        public DataBuildSettingVersion publishVersion; // shown on marketplace and used by build
        public DataVersionInternal internalVersion; // local iterations
        public BuildPhase phase = BuildPhase.none;

        [Header("identification")]
        public string compagny_name = "*";
        public string product_name = "*";

        [Tooltip("project name used to generate output file")]
        public string build_prefix = "";

        [Header("misc")]
        public bool developement_build = false; // match : EditorUserBuildSettings.development
        public bool addVersionInBuildPath = true;

        [Tooltip("activate all profiler stuff")]
        public bool debugScripting = false;

        public enum ProfilingLevel
        {
            none,
            profiling,
            deep,
        }
        public ProfilingLevel debugProfiling = ProfilingLevel.none;

        public Sprite splashscreen;
        public Texture2D icon;

        [Header("buildor features")]
        public DataBuildorScenesMerger merger;
        public fwp.symbols.ScriptableSymbolProfil scriptSymbols;

        virtual protected void OnValidate()
        {
            if (isSelectedPlatform()) applyProfilEditor(); // on validate
        }

        public bool isSelectedPlatform() => getPlatformTarget() == EditorUserBuildSettings.activeBuildTarget;

#if UNITY_EDITOR
        abstract public BuildTarget getPlatformTarget();
        abstract public BuildTargetGroup getPlatformTargetGroup();
#endif

        /// <summary>
        /// output builds/ folder (next to Assets/)
        /// </summary>
        virtual public string getAbsoluteBuildFolderPath(BuildPathFlags pathFlags)
        {
            // path/to/Assets/
            string baseProjectPath = Application.dataPath;
            baseProjectPath = baseProjectPath.Substring(0, baseProjectPath.LastIndexOf('/')); // remove Assets/

            // path/to/builds/
            baseProjectPath = Path.Combine(baseProjectPath, build_path);

            string sub = string.Empty;

            if (pathFlags.pathIncludePrefix) sub += build_prefix + path_separator;
            if (pathFlags.pathIncludeDate) sub += getFullDate() + path_separator;
            if (pathFlags.pathIncludePlatform) sub += getPlatformUid() + path_separator;
            if (pathFlags.pathIncludeVersion) sub += VersionManager.getFormatedVersion('-') + path_separator;

            if (sub.Length > 0)
            {
                sub = sub.Substring(0, sub.Length - path_separator.Length); // remove last "__"
            }

            sub += getFlagsString();

            // builds/(sub/)
            baseProjectPath = Path.Combine(baseProjectPath, sub);

            return baseProjectPath;
        }

        /// <summary>
        /// prefix(_date)(_version)(.extension)
        /// </summary>
        public string getAppName(bool includeExtension = true)
        {
            string output = build_prefix;

            //output += getFlagsString();

            if (includeExtension) output += "." + getExtension();

            return output;
        }

        // windows, osx, ios, ...
        public string getPlatformUid()
        {
            switch (getPlatformTarget())
            {
                case BuildTarget.StandaloneOSX:
                    return "osx";
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                    return "win";
                case BuildTarget.iOS:
                    return "ios";
                case BuildTarget.Android:
                    return "android";
                //case BuildTarget.StandaloneLinux: 
                case BuildTarget.StandaloneLinux64:
                    return "linux";
                case BuildTarget.WebGL:
                    return "web";
                case BuildTarget.Switch:
                    return "switch";
                case BuildTarget.NoTarget:
                    return string.Empty;
                default:
                    Debug.LogError(getPlatformUid() + " : UID unknown ? or un-supported");
                    return string.Empty;
            }
        }

        public string getFlagsString()
        {
            string flags = string.Empty;

#if debug
    flags += "_d";
#endif

#if novideo
    flags += "_nv";
#endif

#if loca_en
    flags += "_en";
#elif loca_fr
    flags += "_fr";
#elif loca_cn
    flags += "_cn";
#endif

            if (developement_build)
            {
                flags += "_dbuild";
            }

            return flags;
        }

        public string getZipName(BuildPathFlags flags)
        {
            string zipName = getAbsoluteBuildFolderPath(flags);

            zipName = zipName.Substring(zipName.LastIndexOf("/") + 1);

            //string zipName = getAppName(false);
            //zipName += getFlagsString();

            zipName += ".zip";

            return zipName;
        }

        abstract public string getExtension();

        virtual public string getProductName() => product_name;

        [MenuItem("Window/Buildor/Apply platform settings (!publish)")]
        static public void applySettings()
        {
            DataBuildSettingProfile data = BuildHelperBase.getActiveProfile();
            data.applyProfilEditor();
        }

        [ContextMenu("apply to player settings")]
        protected void cmApply()
        {
            applyProfilEditor(); // contextmenu on profile
        }

        virtual public void applyProfilEditor(bool usePublishVersion = false)
        {

            //BuildTarget bt = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

            //if (bt == null) Debug.LogError("no build target ?");

            Debug.Log("applying profile : <b>" + name + "</b>");
            Debug.Log("current platform ? " + GetType());

            //Debug.Log("applying " + name);
            //fwp.build.BuildHelperBase.applySettings(this);
            BuildHelperBase.applyCompagny(this);
            BuildHelperBase.applyIcons(this);
            BuildHelperBase.applyUnity(this);
            
            if (usePublishVersion) publishVersion.applyVersionToEditor();
            else internalVersion.applyVersionToEditor();

            if (merger != null) merger.apply();

            EditorUtility.SetDirty(this);
        }

        protected void applyVersionToEditorDefault(bool publish = false)
        {
            if (publish)
            {
                PlayerSettings.bundleVersion = publishVersion.version;
            }
            else
            {
                PlayerSettings.bundleVersion = internalVersion.version;
            }
        }


        /// <summary>
        /// yyyy-mm-dd_hh:mm
        /// </summary>
        static public string getFullDate()
        {
            System.DateTime dt = System.DateTime.Now;
            return dt.Year + "-" + dt.Month + "-" + dt.Day + "_" + dt.Hour + "-" + dt.Minute;
        }

    }

}

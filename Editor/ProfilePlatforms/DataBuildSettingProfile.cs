using UnityEngine;
using UnityEditor;
using System.IO;
using fwp.buildor.version;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace fwp.buildor
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
        public bool developement_build = false;
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

        [Header("splashscreen")]
        public Sprite splashscreen;

        [Header("icons")]
        public Texture2D icon;

        virtual protected void OnValidate()
        {
            if (isSelectedPlatform()) apply(); // on validate
        }

        public bool isSelectedPlatform() => getPlatformTarget() == EditorUserBuildSettings.activeBuildTarget;

#if UNITY_EDITOR
        virtual public BuildTarget getPlatformTarget() => BuildTarget.NoTarget;
#endif

        /// <summary>
        /// output builds/ folder (next to Assets/)
        /// </summary>
        virtual public string getAbsoluteBuildFolderPath(BuildPathFlags flags)
        {
            // path/to/Assets/
            string baseProjectPath = Application.dataPath;
            baseProjectPath = baseProjectPath.Substring(0, baseProjectPath.LastIndexOf('/')); // remove Assets/

            // path/to/builds/
            baseProjectPath = Path.Combine(baseProjectPath, build_path);

            string sub = string.Empty;

            if (flags.pathIncludePrefix) sub += build_prefix + path_separator;
            if (flags.pathIncludeDate) sub += getFullDate() + path_separator;
            if (flags.pathIncludePlatform) sub += getPlatformUid() + path_separator;
            if (flags.pathIncludeVersion) sub += VersionManager.getFormatedVersion('-') + path_separator;

            if (sub.Length > 0)
            {
                sub = sub.Substring(0, sub.Length - path_separator.Length); // remove last "__"
            }

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

        public string getZipName()
        {
            string zipName = getAppName(false);

#if debug
    zipName += "_d";
#endif

#if novideo
    zipName += "_nv";
#endif

#if loca_en
    zipName += "_en";
#elif loca_fr
    zipName += "_fr";
#elif loca_cn
    zipName += "_cn";
#endif

            if (EditorUserBuildSettings.development)
            {
                zipName += "_dbuild";
            }

            zipName += ".zip";

            return zipName;
        }

        abstract public string getExtension();

        virtual public string getProductName() => product_name;

        [ContextMenu("apply to player settings")]
        protected void cmApply()
        {
            apply(); // contextmenu on profile
        }

        virtual public void apply(bool publish = false)
        {
            //Debug.Log("applying " + name);
            //fwp.build.BuildHelperBase.applySettings(this);

            PlayerSettings.companyName = compagny_name;
            PlayerSettings.productName = getProductName();

            Texture2D[] icons = new Texture2D[1];
            icons[0] = icon;

            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);

            PlayerSettings.SplashScreen.show = false;

            EditorUserBuildSettings.development = developement_build;
            //EditorUserBuildSettings.allowDebugging = scriptDebugging;

            if (publish) publishVersion.applyVersionToEditor();
            else internalVersion.applyVersionToEditor();

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

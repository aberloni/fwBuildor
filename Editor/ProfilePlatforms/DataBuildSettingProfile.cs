using UnityEngine;
using UnityEditor;
using System.IO;

using fwp.version;
using fwp.logs;

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
    abstract public class DataBuildSettingProfile : BuildorScriptable
    {
        const string build_path = "builds/"; // next to Assets/
        const string path_separator = "__";

        [Header("version")]

        public DataVersionInternal versionInternal;     // local iterations
        public DataBuildSettingVersion versionPublish;  // can be null, alternate wat to count versions

        public DataBuildSettingVersion Version
        {
            get
            {
                if (versionPublish != null) return versionPublish;
                return versionInternal;
            }
        }

        public TargetPublish publish = TargetPublish.release;
        public TargetSdks sdk = TargetSdks.none;

        public bool Is(TargetPublish lvl, TargetSdks sdk) => this.publish == lvl && this.sdk == sdk;

        [Header("identification")]
        public string compagny_name = "*";
        public string product_name = "*";

        // exe name is located in BUILD scriptable

        public DataProfilBuildParameters build;
        public DataProfilDebugParameters debug;

        public string Symbols
        {
            get
            {
                string ret = string.Empty;

                // demo & festival
                if (publish != TargetPublish.release) ret += publish.ToString() + ";";
                if (sdk != TargetSdks.none) ret += sdk.ToString() + ";";

                if (build != null)
                {
                    ret += build.SymbolsFeatures;
                    if (build.symbols != null) ret += BuildorHelpers.formatSymbols(build.symbols);
                }

                // logs by debug level
                if (Logs != null) ret += BuildorHelpers.formatSymbols(Logs.symbolsVerbose);

                if (BuildorVars.IsDebug)
                {
                    ret += "debug;";
                }

                return ret;
            }
        }

        public ProfilLogLevels Logs => BuildorVars.IsDebug ? debug.Logs : build.Logs;

        /// <summary>
        /// root/path/build.ext
        /// </summary>
        public string FullPath => Path.Combine(BuildPath, getAppName()).Replace("\\", "/");

        /// <summary>
        /// folder parent of export destination
        /// </summary>
        public string PathParent => System.IO.Path.GetDirectoryName(FullPath);

        /// <summary>
        /// path where executable is built
        /// </summary>
        public string BuildPath
        {
            get
            {

                if (!string.IsNullOrEmpty(build.build_folder_specific))
                    return build.build_folder_specific;

                // drive:to/root/Assets + relative/folder/
                return Path.Combine(
                    Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")),
                    getRelativeBuildFolderPath());
            }
        }

        virtual protected void OnValidate()
        {
            if (isSelectedPlatform()) injectProfilToEditor(); // on validate
        }

        public bool isSelectedPlatform() => getPlatformTarget() == EditorUserBuildSettings.activeBuildTarget;

#if UNITY_EDITOR
        abstract public BuildTarget getPlatformTarget();
        abstract public BuildTargetGroup getBuildTargetGroup();
#endif

        /// <summary>
        /// export folder + path within export folder
        /// relative to project root (parent of Assets/)
        /// this use window modifiers
        /// </summary>
        virtual public string getRelativeBuildFolderPath()
        {
            string sub = string.Empty;

            if (EditorPrefs.GetBool(BuildorVars.pref_include_prefix)) sub += build.build_prefix + path_separator;
            if (EditorPrefs.GetBool(BuildorVars.pref_include_platform)) sub += getPlatformUid() + path_separator;
            if (EditorPrefs.GetBool(BuildorVars.pref_include_date)) sub += getFullDate() + path_separator;
            if (EditorPrefs.GetBool(BuildorVars.pref_include_version)) sub += Version + path_separator;

            sub += EditorPrefs.GetString(BuildorVars.pref_suffix);

            // remove last "__"
            if (sub.EndsWith(path_separator))
            {
                sub = sub.Substring(0, sub.Length - path_separator.Length);
            }

            sub += solveFlagsString();

            // builds/(sub/)
            return Path.Combine(build_path, sub);
        }

        /// <summary>
        /// prefix(_date)(_version)(.extension)
        /// </summary>
        public string getAppName(bool includeExtension = true)
        {
            string output = build.build_prefix;

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

        public string solveFlagsString()
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

            if (BuildorVars.IsDebug && debug != null)
            {
                if (debug.developement_build)
                {
                    flags += "_dbuild";
                }
            }


            return flags;
        }

        public string ZipFullPath
        {
            get
            {
                string parent = Path.GetDirectoryName(Path.GetDirectoryName(FullPath));
                return Path.Combine(parent, getZipName());
            }
        }

        string getZipName()
        {
            return System.IO.Path.GetDirectoryName(FullPath) + ".zip";
        }

        abstract public string getExtension();

        virtual public string getProductName() => product_name;

        [ContextMenu("apply to player settings")]
        protected void cmApply()
        {
            injectProfilToEditor(); // contextmenu on profile
        }

        virtual public void injectProfilToEditor()
        {
            Debug.Log("applying scriptable profile : <b>" + name + "</b>", this);
            Debug.Log("current platform ? " + GetType());
            Debug.Log("Debug ? " + BuildorVars.IsDebug);

            //Debug.Log("applying " + name);
            //fwp.build.BuildHelperBase.applySettings(this);
            BuildPreprocess.applyCompagny(this);
            BuildPreprocess.applyIcons(this);

            if (debug != null)
            {
                // apply: will clear instead if not at debug level
                debug.apply();
                debug.log();
            }

            if (versionPublish != null) versionPublish.applyVersionToEditor();
            else versionInternal.applyVersionToEditor();

            // merger might be override
            // merger is applied before build exec
            // if (merger != null) merger.apply();
            if (build != null)
            {
                build.Apply();
            }

            EditorUtility.SetDirty(this);
        }

        protected void applyVersionToEditorDefault(bool publish = false)
        {
            if (publish)
            {
                PlayerSettings.bundleVersion = versionPublish.Version;
            }
            else
            {
                PlayerSettings.bundleVersion = versionInternal.Version;
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

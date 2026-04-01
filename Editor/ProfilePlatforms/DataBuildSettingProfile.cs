using UnityEngine;
using UnityEditor;
using System.IO;

using fwp.version;
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

        public PublishLevel releaseLevel = PublishLevel.intern;
        public BuildPhase phase = BuildPhase.none;

        [Header("identification")]
        public string compagny_name = "*";
        public string product_name = "*";

        public DataProfilBuildParameters build;
        public DataProfilDebugParameters debug;

        virtual protected void OnValidate()
        {
            if (isSelectedPlatform()) applyProfilEditor(); // on validate
        }

        public bool isSelectedPlatform() => getPlatformTarget() == EditorUserBuildSettings.activeBuildTarget;

#if UNITY_EDITOR
        abstract public BuildTarget getPlatformTarget();
        abstract public BuildTargetGroup getPlatformTargetGroup();
#endif

        string SubFolder
        {
            get
            {
                string ret = string.Empty;

                if (EditorPrefs.GetBool(BuildHelperBase.pref_specific_folder_steam)) ret = "steam";
                else if (EditorPrefs.GetBool(BuildHelperBase.pref_specific_folder_switch)) ret = "switch";
                else
                {
                    ret = EditorPrefs.GetString(BuildHelperBase.pref_specific_folder);
                }

                if (string.IsNullOrEmpty(ret))
                {
                    ret = build_path;
                }

                return ret;
            }
        }

        /// <summary>
        /// solve subfolder
        /// </summary>
        virtual public string getRelativeBuildFolderPath()
        {
            string sub = string.Empty;

            if (EditorPrefs.GetBool(BuildHelperBase.pref_include_prefix)) sub += build.build_prefix + path_separator;
            if (EditorPrefs.GetBool(BuildHelperBase.pref_include_platform)) sub += getPlatformUid() + path_separator;

            // anything dynamic
            if (!BuildHelperBase.IsFolderOverride)
            {
                if (EditorPrefs.GetBool(BuildHelperBase.pref_include_date)) sub += getFullDate() + path_separator;
                if (EditorPrefs.GetBool(BuildHelperBase.pref_include_version)) sub += VersionManager.getFormatedVersion('-') + path_separator;
            }

            sub += EditorPrefs.GetString(BuildHelperBase.pref_suffix);

            // remove last "__"
            if (sub.EndsWith(path_separator))
            {
                sub = sub.Substring(0, sub.Length - path_separator.Length);
            }

            sub += getFlagsString();

            // builds/(sub/)
            return Path.Combine(SubFolder, sub);
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

            if (debug.developement_build)
            {
                flags += "_dbuild";
            }

            return flags;
        }

        public string getZipName()
        {
            string zipName = getRelativeBuildFolderPath();

            zipName = zipName.Substring(zipName.LastIndexOf("/") + 1);

            //string zipName = getAppName(false);
            //zipName += getFlagsString();

            zipName += ".zip";

            return zipName;
        }

        abstract public string getExtension();

        virtual public string getProductName() => product_name;

        [ContextMenu("apply to player settings")]
        protected void cmApply()
        {
            applyProfilEditor(); // contextmenu on profile
        }

        virtual public void applyProfilEditor()
        {
            Debug.Log("applying profile : <b>" + name + "</b>");
            Debug.Log("current platform ? " + GetType());

            //Debug.Log("applying " + name);
            //fwp.build.BuildHelperBase.applySettings(this);
            BuildHelperBase.applyCompagny(this);
            BuildHelperBase.applyIcons(this);
            BuildHelperBase.applyUnity(this);

            if (versionPublish != null) versionPublish.applyVersionToEditor();
            else versionInternal.applyVersionToEditor();

            // merger might be override
            // merger is applied before build exec
            // if (merger != null) merger.apply();

            EditorUtility.SetDirty(this);
        }

        protected void applyVersionToEditorDefault(bool publish = false)
        {
            if (publish)
            {
                PlayerSettings.bundleVersion = versionPublish.version;
            }
            else
            {
                PlayerSettings.bundleVersion = versionInternal.version;
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

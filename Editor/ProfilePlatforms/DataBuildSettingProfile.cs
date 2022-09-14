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
    /// </summary>
    abstract public class DataBuildSettingProfile : ScriptableObject
    {
        [Header("version")]
        public DataBuildSettingVersion publishVersion; // shown on marketplace and used by build
        public DataVersionInternal internalVersion; // local iterations
        public BuildPhase phase = BuildPhase.none;

        [Header("identification")]
        public string compagny_name = "*";
        public string product_name = "*";

        [Header("file")]
        public string build_path = "builds/";

        [Tooltip("project name used to generate output file")]
        public string build_prefix = "";

        [Header("misc")]
        public bool developement_build = false;
        public bool addVersionInBuildPath = true;
        public bool scriptDebugging = false;

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
        /// path/to/build/
        /// no name
        /// </summary>
        protected string getAbsBuildFolderPath()
        {
            string baseProjectPath = Application.dataPath;
            baseProjectPath = baseProjectPath.Substring(0, baseProjectPath.LastIndexOf('/')); // remove Assets/

            //profile build suffix path 
            string buildPathFolder = build_path;
            if (!buildPathFolder.EndsWith("/")) buildPathFolder += "/";

            string absPath = Path.Combine(baseProjectPath, buildPathFolder);
            //Debug.Log("build folder => " + absPath);

            return absPath;
        }

        public string getBuildFolderName() => build_prefix + "_" + fwp.halpers.HalperTime.getFullDate();
        public string getBuildNameVersion()
        {
            string output = build_prefix;
            if (addVersionToBuildPath()) output += "_" + VersionManager.getFormatedVersion('-');
            return output;
        }

        virtual public bool addVersionToBuildPath()
        {
            bool addVersion = addVersionInBuildPath;

#if steam
    addVersion = false;
#endif

            return addVersion;
        }

        public string getFullPath() => Path.Combine(getBasePath(), getBuildFullName(true));

        public string getExportFolderPath() => getAbsBuildFolderPath();

        // all path by NOT build name.ext
        virtual public string getBasePath() => getAbsBuildFolderPath();

        virtual public string getBuildFullName(bool ext)
        {
            string buildName = getBuildNameVersion();

            if (ext) buildName += "." + getExtension();
            return buildName;
        }

        public string getZipName()
        {
            string zipName = getBuildNameVersion();

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

    }

}

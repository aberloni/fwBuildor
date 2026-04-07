using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;

/// <summary>
/// 
/// build by script
/// https://docs.unity3d.com/ScriptReference/BuildPipeline.BuildPlayer.html
/// 
/// menu item shortcuts
/// https://docs.unity3d.com/ScriptReference/MenuItem.html
/// 
/// clear console
/// https://answers.unity.com/questions/707636/clear-console-window.html
/// 
/// what is supposed to be version number on ios
/// https://stackoverflow.com/questions/21125159/which-ios-app-version-build-numbers-must-be-incremented-upon-app-store-release/38009895#38009895
/// 
/// buils size
/// 
/// https://stackoverflow.com/questions/28100362/how-to-reduce-the-size-of-an-apk-file-in-unity
/// https://docs.unity3d.com/Manual/iphone-playerSizeOptimization.html
/// 
/// The pair(Version, Build number) must be unique.
/// The sequence is valid: (1.0.1, 12) -> (1.0.1, 13) -> (1.0.2, 13) -> (1.0.2, 14) ...
/// Version(CFBundleShortVersionString) must be in ascending sequential order.
/// Build number(CFBundleVersion) must be in ascending sequential order.
/// 
/// Based on the checklist, the following (Version, Build Number) sequence is valid too.
/// Case: reuse Build Number in different release trains.
/// (1.0.0, 1) -> (1.0.0, 2) -> ... -> (1.0.0, 11) -> (1.0.1, 1) -> (1.0.1, 2)
/// 
/// </summary>

namespace fwp.buildor.editor
{
    using UnityEditor;

    /// <summary>
    /// regroup all parameters used during build process
    /// encapsulate the build process
    /// </summary>
    public class BuildHelperBase
    {
        public const string pref_prefix = "buildor_";

        static public readonly string pref_uid = pref_prefix + "." + Application.productName + "." + Application.productName;

        // modifiers
        static public readonly string pref_include_prefix = pref_uid + "prefix";
        static public readonly string pref_include_platform = pref_uid + "platform";
        static public readonly string pref_include_date = pref_uid + "date";
        static public readonly string pref_include_version = pref_uid + "version";
        static public readonly string pref_suffix = pref_uid + "suffix";
        
        static public DataBuildSettingProfile Profile => WinEdBuildor.Profile;
        static public string Platform => Profile?.getPlatformUid();

        public BuildPreprocess build_prepro;

        public BuildPostprocess build_postpro;

        public BuildHelperBase()
        {
            build_prepro = new();
            build_postpro = new();
        }

        public void launch()
        {
            // data = getScriptableDataBuildSettings();
            Debug.Log(" <<< <b>starting build process</b> >>>");

            build_prepro.doLaunch(Profile);
            build_prepro.onBuilt += (summary) =>
            {
                build_postpro.doLaunch(Profile, summary);
            };
        }

        protected string getBuildName()
        {
            DataBuildSettingsBridge data = getScriptableDataBuildSettings();
            return data.getPlatformProfil().build.build_prefix;
        }

        /// <summary>
        /// 
        /// </summary>
        static public DataBuildSettingsBridge getScriptableDataBuildSettings()
        {
            string[] all = AssetDatabase.FindAssets("t:DataBuildSettingsBridge");

            if (all.Length > 0)
            {
                for (int i = 0; i < all.Length; i++)
                {
                    Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(DataBuildSettingsBridge));
                    DataBuildSettingsBridge data = obj as DataBuildSettingsBridge;
                    if (data != null) return data;
                }
            }
            //Debug.LogWarning("no objects returned by AssetDatabase for type : DataBuildSettingsBridge");

            //Debug.LogError("could not find object of type : DataBuildSettingsBridge");
            return null;
        }

        /// <summary>
        /// buffed
        /// </summary>
        static DataBuildSettingProfile _active;
        static public DataBuildSettingProfile getActiveProfile(PublishLevel pl)
        {
            if (_active != null && _active.releaseLevel == pl) return _active;

            var settings = getScriptableDataBuildSettings();

            if (settings == null)
            {
                if (settings == null) Debug.LogWarning("could not locate any bridge: DataBuildSettingsBridge");
                return null;
            }

            _active = settings.getPlatformProfil(pl);
            return _active;
        }

        static public void applyUnity(DataBuildSettingProfile profil)
        {

            PlayerSettings.SplashScreen.show = false;
            Debug.Log(" L splash show (auto false under licence) : " + PlayerSettings.SplashScreen.show);

            //dev build
            if (profil.debug != null)
            {
                EditorUserBuildSettings.development = profil.debug.developement_build;
                Debug.Log(" L dev build : " + EditorUserBuildSettings.development);
            }
        }

        static public void applyIcons(DataBuildSettingProfile profil)
        {

            Texture2D[] icons = new Texture2D[1];
            icons[0] = profil.build.icon;

            PlayerSettings.SetIcons(NamedBuildTarget.Unknown, icons, IconKind.Any);

            Debug.Log(" L icons updated");
        }

        static public void applyCompagny(DataBuildSettingProfile profil, bool verbose = false)
        {

            //profil.applyVersionToEditor(); // apply version

            PlayerSettings.companyName = profil.compagny_name;

            if (verbose) Debug.Log("companyName : " + PlayerSettings.companyName);

            //α,β,Ω
            string productName = profil.getProductName();
            
            PlayerSettings.productName = productName;
            if (verbose) Debug.Log("productName : " + PlayerSettings.productName);

        }
    }

}

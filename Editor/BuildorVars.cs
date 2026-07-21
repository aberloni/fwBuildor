using UnityEngine;
using UnityEditor;
using fwp.buildor.editor;

namespace fwp.buildor
{

    static public class BuildorVars
    {
        /// <summary>
        /// filter by platform
        /// </summary>
        static public readonly string Ppref_prefix = 
            EditorUserBuildSettings.activeBuildTarget + "." + 
            Application.companyName + "." + 
            Application.productName;

        /// <summary>
        /// buffed, don't refetch all scriptables each time
        /// </summary>
        static DataBuildSettingProfile _profil;
        static public DataBuildSettingProfile Profile
        {
            get
            {
                if (_profil != null && _profil.Is(BuildorVars.TargetPublish, BuildorVars.TargetSdk))
                {
                    // Debug.Log("recycling : "+_active, _active);
                    return _profil;
                }

                var settings = BuildorHelpers.GetBridge();

                if (settings == null)
                {
                    if (settings == null) Debug.LogWarning("could not locate any bridge <DataBuildSettingsBridge>");
                    return null;
                }

                _profil = settings.getPlatformProfil(TargetPublish, TargetSdk);
                return _profil;
            }
        }

        static public readonly string ppref_sdk = Ppref_prefix + "sdk";
        static public TargetSdks TargetSdk
        {
            get => (TargetSdks)EditorPrefs.GetInt(ppref_sdk, 0);
            set => EditorPrefs.SetInt(ppref_sdk, (int)value);
        }

        static public readonly string ppref_publish = Ppref_prefix + "level_publish";
        static public TargetPublish TargetPublish
        {
            get => (TargetPublish)EditorPrefs.GetInt(ppref_publish, (int)TargetPublish.release);
            set => EditorPrefs.SetInt(ppref_publish, (int)value);
        }

        static public readonly string ppref_debug = Ppref_prefix + "level_debug";
        static public TargetDebug TargetDebug
        {
            get => (TargetDebug)EditorPrefs.GetInt(ppref_debug, (int)TargetDebug.release);
            set => EditorPrefs.SetInt(ppref_debug, (int)value);
        }

        static public bool IsDebug => TargetDebug == TargetDebug.debug;

        static public readonly string ppref_pre_incVersion = Ppref_prefix + "pre_inc_version";
        static public bool PreIncVersion
        {
            get => EditorPrefs.GetBool(ppref_pre_incVersion);
            set => EditorPrefs.SetBool(ppref_pre_incVersion, value);
        }

        static public readonly string ppref_post_openFolder = Ppref_prefix + "post_open_folder";
        static public bool PostOpenFolder
        {
            get => EditorPrefs.GetBool(ppref_post_openFolder);
            set => EditorPrefs.SetBool(ppref_post_openFolder, value);
        }

        static public readonly string ppref_post_zip = Ppref_prefix + "post_zip";
        static public bool PostZip
        {
            get => EditorPrefs.GetBool(ppref_post_zip);
            set => EditorPrefs.SetBool(ppref_post_zip, value);
        }

        static public readonly string ppref_post_autorun = Ppref_prefix + "ppref_post_autorun";
        static public bool PostAutorun
        {
            get => EditorPrefs.GetBool(ppref_post_autorun);
            set => EditorPrefs.SetBool(ppref_post_autorun, value);
        }

        static public readonly string ppref_post_dropVersion = Ppref_prefix + "ppref_post_dropVersion";
        static public bool PostDropVersion
        {
            get => EditorPrefs.GetBool(ppref_post_dropVersion);
            set => EditorPrefs.SetBool(ppref_post_dropVersion, value);
        }

        // modifiers
        static public readonly string pref_include_prefix = Ppref_prefix + "prefix";
        static public readonly string pref_include_platform = Ppref_prefix + "platform";
        static public readonly string pref_include_date = Ppref_prefix + "date";
        static public readonly string pref_include_version = Ppref_prefix + "version";
        static public readonly string pref_suffix = Ppref_prefix + "suffix";

    }

}
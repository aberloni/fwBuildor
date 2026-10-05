using UnityEngine;
using UnityEditor;

namespace fwp.version.editor
{
    using fwp.buildor;
    using fwp.buildor.editor;

    static public class VersionIncrementor
    {
        /// <summary>
        /// current DataVersion of project
        /// active profile (with version) : its DataVersion
        /// no active profile : the only DataVersion of project
        /// null : none, or multiple without active profile
        /// </summary>
        static public DataVersion getCurrentVersion() => solveCurrent(out _, out _);

        /// <param name="origin">where it comes from (or why none), for logs</param>
        static public DataVersion getCurrentVersion(out string origin) => solveCurrent(out _, out origin);

        /// <param name="platform">platform version of active profile, null when coming from project</param>
        static DataVersion solveCurrent(out DataBuildSettingVersion platform, out string origin)
        {
            platform = null;

            // active profile, not verbose : caller logs origin
            var bridge = BuildorHelpers.GetBridge();
            var profile = bridge != null ? bridge.getPlatformProfil(BuildorVars.TargetPublish, BuildorVars.TargetSdk, false) : null;
            if (profile != null && profile.Version != null && profile.Version.HasData)
            {
                platform = profile.Version;
                origin = "active profile " + profile.name;
                return platform.Data;
            }

            // only DataVersion of project
            string[] guids = AssetDatabase.FindAssets("t:" + nameof(DataVersion));
            if (guids.Length == 1)
            {
                origin = "only DataVersion in project";
                return AssetDatabase.LoadAssetAtPath<DataVersion>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }

            origin = "no active profile (with version) & x" + guids.Length + " DataVersion in project";
            return null;
        }

        /// <summary>
        /// patch++ & save current DataVersion (see getCurrentVersion)
        /// command line : -executeMethod fwp.version.editor.VersionIncrementor.incrementPatch
        /// null : nothing incremented
        /// </summary>
        static public DataVersion incrementPatch()
        {
            DataVersion v = getCurrentVersion(out string origin);
            if (v == null)
            {
                Debug.LogWarning("patch++ : " + origin + ", nothing incremented");
                return null;
            }

            v.incrementFix();
            AssetDatabase.SaveAssetIfDirty(v);

            Debug.Log("patch++ : " + v.name + " → <b>" + v.getFormated() + "</b> (" + origin + ")", v);
            return v;
        }

        const string mi_apply = BuildorVerbosity._buildor_menuitem_path + "apply version to player settings";

        [MenuItem(mi_apply, false, 51)]
        static void miApply() => applyVersion();

        /// <summary>
        /// apply current version (see getCurrentVersion) to player settings
        /// active profile : its platform version, platform specifics included
        /// no active profile : X.Y.Z only (bundleVersion)
        /// command line : -executeMethod fwp.version.editor.VersionIncrementor.applyVersion
        /// null : nothing applied
        /// </summary>
        static public DataVersion applyVersion()
        {
            DataVersion v = solveCurrent(out DataBuildSettingVersion platform, out string origin);
            if (v == null)
            {
                Debug.LogWarning("apply version : " + origin + ", nothing applied");
                return null;
            }

            if (platform != null)
            {
                platform.applyVersionToEditor();
                Debug.Log("apply version : " + platform.name + " <b>" + platform.getFormated() + "</b> → player settings (" + origin + ")", platform);
            }
            else
            {
                v.applyVersionToEditor();
                Debug.Log("apply version : " + v.name + " <b>" + v.Version + "</b> → player settings bundleVersion (" + origin + ")", v);
            }

            return v;
        }
    }
}

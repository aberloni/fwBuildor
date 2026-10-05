using UnityEngine;
using UnityEditor;

namespace fwp.version.editor
{
    using fwp.buildor;
    using fwp.buildor.editor;

    static public class VersionIncrementor
    {
        /// <summary>
        /// patch++ & save a DataVersion of project
        /// single DataVersion in project : that one
        /// multiple : the one of active profile, no active profile : warning, nothing incremented
        /// command line : -executeMethod fwp.version.editor.VersionIncrementor.incrementPatch
        /// null : nothing incremented
        /// </summary>
        static public DataVersion incrementPatch()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + nameof(DataVersion));
            if (guids.Length <= 0)
            {
                Debug.LogWarning("patch++ : no DataVersion in project");
                return null;
            }

            DataVersion v;
            string origin;

            if (guids.Length == 1)
            {
                v = AssetDatabase.LoadAssetAtPath<DataVersion>(AssetDatabase.GUIDToAssetPath(guids[0]));
                origin = "only one";
            }
            else
            {
                // not verbose : warning below is enough
                var bridge = BuildorHelpers.GetBridge();
                var profile = bridge != null ? bridge.getPlatformProfil(BuildorVars.TargetPublish, BuildorVars.TargetSdk, false) : null;

                if (profile == null || profile.Version == null || !profile.Version.HasData)
                {
                    Debug.LogWarning("patch++ : x" + guids.Length + " DataVersion in project & no active profile (with version) to pick one, nothing incremented");
                    return null;
                }

                v = profile.Version.Data;
                origin = "active profile " + profile.name;
            }

            v.incrementFix();
            AssetDatabase.SaveAssetIfDirty(v);

            Debug.Log("patch++ : " + v.name + " → <b>" + v.getFormated() + "</b> (x" + guids.Length + " in project, " + origin + ")", v);
            return v;
        }

        const string mi_apply = BuildorVerbosity._buildor_menuitem_path + "apply version to player settings";

        [MenuItem(mi_apply, false, 51)]
        static void miApply() => applyVersion();

        /// <summary>
        /// apply a platform version to player settings
        /// active profile version (if any), else the only platform version of project
        /// command line : -executeMethod fwp.version.editor.VersionIncrementor.applyVersion
        /// null : nothing applied
        /// </summary>
        static public DataBuildSettingVersion applyVersion()
        {
            DataBuildSettingVersion v = null;
            string origin = null;

            // active profile
            var bridge = BuildorHelpers.GetBridge();
            var profile = bridge != null ? bridge.getPlatformProfil(BuildorVars.TargetPublish, BuildorVars.TargetSdk, false) : null;
            if (profile != null && profile.Version != null)
            {
                v = profile.Version;
                origin = "active profile " + profile.name;
            }
            else
            {
                // only platform version of project
                string[] guids = AssetDatabase.FindAssets("t:" + nameof(DataBuildSettingVersion));
                if (guids.Length != 1)
                {
                    Debug.LogWarning("apply version : no active profile (with version) & x" + guids.Length + " platform versions in project, nothing applied");
                    return null;
                }

                v = AssetDatabase.LoadAssetAtPath<DataBuildSettingVersion>(AssetDatabase.GUIDToAssetPath(guids[0]));
                origin = "only one in project";
            }

            if (!v.HasData)
            {
                Debug.LogWarning("apply version : " + v.name + " has no DataVersion, nothing applied (" + origin + ")", v);
                return null;
            }

            v.applyVersionToEditor();

            Debug.Log("apply version : " + v.name + " <b>" + v.getFormated() + "</b> → player settings (" + origin + ")", v);
            return v;
        }
    }
}

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
        /// apply version to player settings
        /// active profile : its platform version (publish, else internal), platform specifics included
        /// no active profile : the only DataVersion of project, X.Y.Z only (bundleVersion)
        /// command line : -executeMethod fwp.version.editor.VersionIncrementor.applyVersion
        /// null : nothing applied
        /// </summary>
        static public DataVersion applyVersion()
        {
            // active profile : platform version
            var bridge = BuildorHelpers.GetBridge();
            var profile = bridge != null ? bridge.getPlatformProfil(BuildorVars.TargetPublish, BuildorVars.TargetSdk, false) : null;
            if (profile != null && profile.Version != null)
            {
                var pv = profile.Version;
                if (!pv.HasData)
                {
                    Debug.LogWarning("apply version : " + pv.name + " has no DataVersion, nothing applied (active profile " + profile.name + ")", pv);
                    return null;
                }

                pv.applyVersionToEditor();
                Debug.Log("apply version : " + pv.name + " <b>" + pv.getFormated() + "</b> → player settings (active profile " + profile.name + ")", pv);
                return pv.Data;
            }

            // no active profile : only DataVersion of project
            string[] guids = AssetDatabase.FindAssets("t:" + nameof(DataVersion));
            if (guids.Length != 1)
            {
                Debug.LogWarning("apply version : no active profile (with version) & x" + guids.Length + " DataVersion in project, nothing applied");
                return null;
            }

            var v = AssetDatabase.LoadAssetAtPath<DataVersion>(AssetDatabase.GUIDToAssetPath(guids[0]));
            v.applyVersionToEditor();
            Debug.Log("apply version : " + v.name + " <b>" + v.Version + "</b> → player settings bundleVersion (only DataVersion in project)", v);
            return v;
        }
    }
}

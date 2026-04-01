using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.version.editor
{
    using fwp.version;
    using fwp.buildor.editor;

    public class VersionIncrementor
    {
        
        [MenuItem("Version/Internal/MAJOR++")]
        static public void incInternalMajor()
        {
            var prof = WinEdBuildor.Profile;
            prof.versionInternal.incrementMajor();
            apply(false);
        }

        [MenuItem("Version/Internal/MINOR++")]
        static public void incInternalMinor()
        {
            var prof = WinEdBuildor.Profile;
            prof.versionInternal.incrementMinor();
            apply(false);
        }

        [MenuItem("Version/Internal/FIX++")]
        static public void incInternalFix()
        {
            var prof = WinEdBuildor.Profile;
            prof.versionInternal.incrementFix();
            apply(false);
        }


        [MenuItem("Version/Publish/MAJOR++")]
        static public void incPublishMajor()
        {
            var prof = WinEdBuildor.Profile;
            prof.versionPublish.incrementMajor();
            apply(true);
        }

        [MenuItem("Version/Publish/MINOR++")]
        static public void incPublishMinor()
        {
            var prof = WinEdBuildor.Profile;
            prof.versionPublish.incrementMinor();
            apply(true);
        }

        [MenuItem("Version/Publish/FIX++")]
        static public void incPublishFix()
        {
            var prof = WinEdBuildor.Profile;
            prof.versionPublish.incrementFix();
            apply(true);
        }

        static public void apply(bool publish)
        {
            var prof = WinEdBuildor.Profile;
            Debug.Assert(prof != null);
            Debug.Assert(prof.versionInternal != null);

            Debug.Log(prof.GetType());

            DataBuildSettingVersion v = publish ? prof.versionPublish : prof.versionInternal;
            Debug.Log(v.GetType() + " | " + v.getFormated());

            v.applyVersionToEditor();
        }


	}

}
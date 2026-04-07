using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.version.editor
{
    using fwp.version;
    using fwp.buildor.editor;
    using fwp.buildor;

    public class VersionIncrementor
    {
        
        [MenuItem("Version/Internal/MAJOR++")]
        static public void incInternalMajor()
        {
            var prof = BuildorHelpers.Profile;
            prof.versionInternal.incrementMajor();
            apply(false);
        }

        [MenuItem("Version/Internal/MINOR++")]
        static public void incInternalMinor()
        {
            var prof = BuildorHelpers.Profile;
            prof.versionInternal.incrementMinor();
            apply(false);
        }

        [MenuItem("Version/Internal/FIX++")]
        static public void incInternalFix()
        {
            var prof = BuildorHelpers.Profile;
            prof.versionInternal.incrementFix();
            apply(false);
        }


        [MenuItem("Version/Publish/MAJOR++")]
        static public void incPublishMajor()
        {
            var prof = BuildorHelpers.Profile;
            prof.versionPublish.incrementMajor();
            apply(true);
        }

        [MenuItem("Version/Publish/MINOR++")]
        static public void incPublishMinor()
        {
            var prof = BuildorHelpers.Profile;
            prof.versionPublish.incrementMinor();
            apply(true);
        }

        [MenuItem("Version/Publish/FIX++")]
        static public void incPublishFix()
        {
            var prof = BuildorHelpers.Profile;
            prof.versionPublish.incrementFix();
            apply(true);
        }

        static public void apply(bool publish)
        {
            var prof = BuildorHelpers.Profile;
            Debug.Assert(prof != null);
            Debug.Assert(prof.versionInternal != null);

            Debug.Log(prof.GetType());

            DataBuildSettingVersion v = publish ? prof.versionPublish : prof.versionInternal;
            Debug.Log(v.GetType() + " | " + v.getFormated());

            v.applyVersionToEditor();
        }


	}

}
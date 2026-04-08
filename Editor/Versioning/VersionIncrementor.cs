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
        static DataBuildSettingProfile Profile => BuildorVars.Profile;

        [MenuItem("Version/Internal/MAJOR++")]
        static public void incInternalMajor()
        {
            Profile.versionInternal.incrementMajor();
            apply(false);
        }

        [MenuItem("Version/Internal/MINOR++")]
        static public void incInternalMinor()
        {
            Profile.versionInternal.incrementMinor();
            apply(false);
        }

        [MenuItem("Version/Internal/FIX++")]
        static public void incInternalFix()
        {
            Profile.versionInternal.incrementFix();
            apply(false);
        }


        [MenuItem("Version/Publish/MAJOR++")]
        static public void incPublishMajor()
        {
            Profile.versionPublish.incrementMajor();
            apply(true);
        }

        [MenuItem("Version/Publish/MINOR++")]
        static public void incPublishMinor()
        {
            Profile.versionPublish.incrementMinor();
            apply(true);
        }

        [MenuItem("Version/Publish/FIX++")]
        static public void incPublishFix()
        {
            Profile.versionPublish.incrementFix();
            apply(true);
        }

        static public void apply(bool publish)
        {
            if (Profile == null) return;

            Debug.Assert(Profile.versionInternal != null);

            Debug.Log(Profile.GetType());

            DataBuildSettingVersion v = publish ? Profile.versionPublish : Profile.versionInternal;
            
            Debug.Log(v.GetType() + " | " + v.getFormated());

            v.applyVersionToEditor();
        }


    }

}
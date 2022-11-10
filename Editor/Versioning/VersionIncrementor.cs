using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.buildor.version
{
    using fwp.buildor.editor;

    public class VersionIncrementor
    {
        
        [MenuItem("Version/Internal/MAJOR++")]
        static public void incInternalMajor()
        {
            DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();
            prof.internalVersion.incrementMajor();
            apply(false);
        }

        [MenuItem("Version/Internal/MINOR++")]
        static public void incInternalMinor()
        {
            DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();
            prof.internalVersion.incrementMinor();
            apply(false);
        }

        [MenuItem("Version/Internal/FIX++")]
        static public void incInternalFix()
        {
            DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();
            prof.internalVersion.incrementFix();
            apply(false);
        }


        [MenuItem("Version/Publish/MAJOR++")]
        static public void incPublishMajor()
        {
            DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();
            prof.publishVersion.incrementMajor();
            apply(true);
        }

        [MenuItem("Version/Publish/MINOR++")]
        static public void incPublishMinor()
        {
            DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();
            prof.publishVersion.incrementMinor();
            apply(true);
        }

        [MenuItem("Version/Publish/FIX++")]
        static public void incPublishFix()
        {
            DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();
            prof.publishVersion.incrementFix();
            apply(true);
        }

        static public void apply(bool publish)
        {
            DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();
            Debug.Assert(prof != null);
            Debug.Assert(prof.internalVersion != null);

            Debug.Log(prof.GetType());

            DataBuildSettingVersion v = publish ? prof.publishVersion : prof.internalVersion;
            Debug.Log(v.GetType() + " | " + v.getFormated());

            v.applyVersionToEditor();
        }

    }

}
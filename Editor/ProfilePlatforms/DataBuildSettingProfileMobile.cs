using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace fwp.buildor.editor
{


    abstract public class DataBuildSettingProfileMobile : DataBuildSettingProfile
    {
        [Header("mobile section")]

        public string package_name_debug = "com.*.*"; // dev build
        public string package_name_release = "com.*.*"; // release

        [Header("device")]

        public bool use_joystick_visualisation = true;

        public UIOrientation orientation_default = UIOrientation.Portrait;

        [Header("archi")]
        public bool sixtyFourBits = true;

        public override void applyProfilEditor(bool usePublishVersion = false)
        {
            base.applyProfilEditor(usePublishVersion);

            Debug.Log("~Mobile");

            // mobile specifics

            PlayerSettings.defaultInterfaceOrientation = orientation_default;
            Debug.Log(" L " + PlayerSettings.defaultInterfaceOrientation);

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, getPackageName());
            Debug.Log(" L " + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android));
        }

        public string getPackageName()
        {
#if debug
    return package_name_debug;
#else
            return package_name_release;
#endif
        }
    }
}
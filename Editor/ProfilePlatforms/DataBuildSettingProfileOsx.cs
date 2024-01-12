using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// (ratio iphone)
/// 562x1000 
/// 506x900
/// </summary>

namespace fwp.buildor
{
    [CreateAssetMenu(menuName = "buildor/new profil:osx", order = 100)]
    public class DataBuildSettingProfileOsx : DataBuildSettingProfile
    {
        public override string getExtension() => "app";

        public override BuildTarget getPlatformTarget() => BuildTarget.StandaloneOSX;
        public override BuildTargetGroup getPlatformTargetGroup() => BuildTargetGroup.iOS;

    }

}

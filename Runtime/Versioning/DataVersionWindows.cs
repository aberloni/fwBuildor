using System.Collections.Generic;
using UnityEngine;


namespace fwp.buildor.version
{
    [CreateAssetMenu(menuName = "buildor/version/new windows", order = 100)]
    public class DataVersionWindows : DataBuildSettingVersion
    {
        public override void applyVersionToEditor()
        {
#if UNITY_EDITOR
            //short version
            UnityEditor.PlayerSettings.bundleVersion = version;
#endif
        }
    }

}

using UnityEngine;

namespace fwp.version
{
    [CreateAssetMenu(menuName = "buildor/version/new windows", order = 100)]
    public class DataVersionWindows : DataBuildSettingVersion
    {
        
#if UNITY_EDITOR
        public override void applyVersionToEditor()
        {
            //short version
            UnityEditor.PlayerSettings.bundleVersion = Version;

        }
#endif

    }

}

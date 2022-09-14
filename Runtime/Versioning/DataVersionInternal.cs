using UnityEngine;

namespace fwp.buildor.version
{
    [CreateAssetMenu(menuName = "buildor/version/new Internal", order = 100)]
    public class DataVersionInternal : DataBuildSettingVersion
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

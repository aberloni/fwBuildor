using UnityEngine;

namespace fwp.version
{
	[CreateAssetMenu(menuName = "buildor/version/new Internal", order = 100)]
	public class DataVersionInternal : DataBuildSettingVersion
	{
		public override void applyVersionToEditor()
		{
#if UNITY_EDITOR
			//ios specific version
			UnityEditor.PlayerSettings.iOS.buildNumber = version;

			//android bundle version code
			UnityEditor.PlayerSettings.Android.bundleVersionCode = buildNumber;

			//version under compagny name & product name
			UnityEditor.PlayerSettings.bundleVersion = version;
			//Debug.Log("+bundleVersion=" + UnityEditor.PlayerSettings.bundleVersion);
#endif
		}
	}

}

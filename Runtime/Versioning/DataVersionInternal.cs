using UnityEngine;

namespace fwp.version
{
	[CreateAssetMenu(menuName = "buildor/version/+internal", order = 100)]
	public class DataVersionInternal : DataBuildSettingVersion
	{
		
#if UNITY_EDITOR
		public override void applyVersionToEditor()
		{
			//ios specific version
			UnityEditor.PlayerSettings.iOS.buildNumber = Version;

			//android bundle version code
			UnityEditor.PlayerSettings.Android.bundleVersionCode = BuildNumber;

			//version under compagny name & product name
			UnityEditor.PlayerSettings.bundleVersion = Version;
			//Debug.Log("+bundleVersion=" + UnityEditor.PlayerSettings.bundleVersion);
		}
#endif

	}

}

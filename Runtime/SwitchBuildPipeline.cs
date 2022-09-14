using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// https://developer.nintendo.com/group/development/g1kr9vj6/forums/english/-/gts_message_boards/thread/285503066
/// </summary>

static class SwitchBuildPipeline
{

#if UNITY_EDITOR && UNITY_SWITCH
	[MenuItem("Build/Nintendo Switch")]
	static void DoMenuItem()
	{
		/*
		// Check if %NINTENDO_SDK_ROOT% environment variable exists
		var nintendoSdkRoot = System.Environment.ExpandEnvironmentVariables("%NINTENDO_SDK_ROOT%");
		if (nintendoSdkRoot == "%NINTENDO_SDK_ROOT%")
		{
			Debug.LogError("Could not find %NINTENDO_SDK_ROOT% environment variable.");
			return;
		}

		// Check if the actual %NINTENDO_SDK_ROOT% directory exists
		if (!System.IO.Directory.Exists(nintendoSdkRoot))
		{
			Debug.LogErrorFormat("The %NINTENDO_SDK_ROOT% directory '{0}' does not exist.", nintendoSdkRoot);
			return;
		}

		// Check if it contains the UnityForNintendoSwitch directory
		var unityForSwitchDirectory = System.IO.Path.Combine(System.IO.Directory.GetParent(nintendoSdkRoot).FullName, "UnityForNintendoSwitch");
		if (!System.IO.Directory.Exists(nintendoSdkRoot))
		{
			Debug.LogErrorFormat("The directory '{0}' does not exist.", unityForSwitchDirectory);
			return;
		}

		// Check if it contains the Unity installer of the editor version that is running this code.
		// If the installer version does not match, then it's the wrong SDK.
		var unityInstaller = string.Format("UnitySetup-Nintendo-Switch-Support-for-Editor-{0}-SDK-*.exe", Application.unityVersion);
		if (System.IO.Directory.GetFiles(unityForSwitchDirectory, unityInstaller, System.IO.SearchOption.TopDirectoryOnly).Length == 0)
		{
			Debug.LogErrorFormat("%NINTENDO_SDK_ROOT% seems to point to the wrong SDK.\n{0}", nintendoSdkRoot);
			Debug.LogError(unityInstaller);

			return;
		}

		// Call UnityEditor.BuildPipeline.BuildPlayer()
		*/
	}
#endif
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBuildUtils : MonoBehaviour
{
	/// <summary>
	/// https://docs.unity3d.com/Manual/class-PlayerSettingsAndroid.html#Identification
	/// doc note : Keep this number under 100000 if Split APKs by target architecture is enabled.
	/// 
	/// https://stackoverflow.com/questions/9720229/unity-3d-what-is-the-android-bundle-version-and-version-code-and-how-do-they-re
	/// meant to return the google formula bundle code
	/// 
	/// must follow format : X.YY.ZZZ
	/// 
	/// THIS CANNOT BE USE FOR SPLIT APK
	/// </summary>
	/// <returns></returns>
	static public string buildSetsGetAndroidBunleCode()
	{
		string v = buildSetsGetAndroidVersion();

		//major*100000 + minor*1000 + build formula
		//major*X[YY][ZZZ]
		//minor*Y[ZZZ]
		//this is ARBITRARY and aiming for format : X YY ZZZ

		//3.43.765 => 30000 + 43000 + 765
		string[] split = v.Split('.');
		int[] vals = new int[split.Length];
		for (int i = 0; i < vals.Length; i++)
		{
			vals[i] = int.Parse(split[i]);
		}

		// MAJOR, MINOR, BUILD

		int[] muls = new int[3] { 100000, 1000, 1};
		string output = string.Empty;
		for (int i = vals.Length-1; i >= 0; i--)
		{
			output += (vals[i] * muls[i]).ToString();
		}

		return output;
	}

	/// <summary>
	/// https://docs.unity3d.com/ScriptReference/Application-version.html
	/// WINDOWS : version number under product name, top of build settings
	/// OTHER ?
	/// </summary>
	/// <returns></returns>
	static public string buildSetsGetAndroidVersion()
	{
		throw new System.NotImplementedException();
		//return string.Empty;
	}

	/// <summary>
	/// top of build settings
	/// </summary>
	/// <returns></returns>
	static public string buildSetsProductName()
	{
		return Application.productName;
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem("Tools/Buildor/log app.version")]
	static public void logAppVersion() => Debug.Log(Application.version);
#endif
}

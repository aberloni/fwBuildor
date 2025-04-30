using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics.Contracts;
using System.Linq;

/// <summary>
/// this is meant to store an abstract of the version number of the app
/// is base on each platform format and accessor
/// </summary>

namespace fwp.version
{
	public enum BuildSettingVersionType
	{
		vInternal, vPublish
	}

	[System.Serializable]
	abstract public class DataBuildSettingVersion : ScriptableObject
	{
		[Header("version")]
		public string version = "0.0.1";
		public int buildNumber = 1;

		/// <summary>
		/// x.y.z
		/// </summary>
		/// <returns></returns>
		virtual public string getDataVersion()
		{
			//return VersionManager.getFormatedVersion(version);
			return version;
		}

		/// <summary>
		/// int[] [x],[y],[z]
		/// </summary>
		/// <returns></returns>
		public int[] getDataVersionInts()
		{
			List<string> list = new List<string>();
			list.AddRange(version.Split(VersionManager.versionSeparator));

			List<int> output = new List<int>();
			for (int i = 0; i < list.Count; i++)
			{
				output.Add(int.Parse(list[i]));
			}
			return output.ToArray();
		}

		public void incrementMajor()
		{
			int[] v = getDataVersionInts();

			v[0]++;
			if (v.Length > 1) v[1] = 0;
			if (v.Length > 2) v[2] = 0;

			buildNumber++;
			applyInts(v);

			applyVersionToEditor();
		}

		public void incrementMinor()
		{
			int[] v = getDataVersionInts();

			if (v.Length < 2)
			{
				List<int> tmp = new List<int>();
				tmp.AddRange(v);
				tmp.Add(0);
				v = tmp.ToArray();
			}

			v[1]++;
			if (v.Length > 2) v[2] = 0;

			buildNumber++;
			applyInts(v);

			applyVersionToEditor();
		}

		public void incrementFix()
		{
			int[] v = getDataVersionInts();

			if (v.Length < 3)
			{
				List<int> tmp = new List<int>();
				tmp.AddRange(v);
				tmp.Add(0);
				v = tmp.ToArray();
			}

			v[2]++;

			buildNumber++;
			applyInts(v);

			applyVersionToEditor();
		}

		void applyInts(int[] vs)
		{
			version = vs[0] + "." + vs[1] + "." + vs[2];

#if UNITY_EDITOR
			//make that scriptable dirty to be saved
			EditorUtility.SetDirty(this);
#endif
		}

		virtual public string getFormated()
		{
			return version + "@" + buildNumber;
		}

		abstract public void applyVersionToEditor();

		//temp

		static protected void applyVersionIos(DataBuildSettingVersion v)
		{
#if UNITY_EDITOR
			//version
			PlayerSettings.iOS.buildNumber = v.version;

			//short version
			PlayerSettings.bundleVersion = v.version;
#endif
		}


		static public void applyVersionToEditor(DataBuildSettingVersion v)
		{
#if UNITY_EDITOR
			//version
			PlayerSettings.Android.bundleVersionCode = v.buildNumber;

			//short version
			PlayerSettings.bundleVersion = v.version;
#endif
		}

		static public DataBuildSettingVersion[] getScriptables(string filter = null)
		{
			string[] all = AssetDatabase.FindAssets("t:DataBuildSettingVersion");
			if (all.Length <= 0) return null;

			List<DataBuildSettingVersion> ret = new();
			for (int i = 0; i < all.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(all[i]);

				if (!string.IsNullOrEmpty(filter))
				{
					if (!path.Contains(filter)) continue;
				}

				Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(DataBuildSettingVersion));
				DataBuildSettingVersion data = obj as DataBuildSettingVersion;
				if (data != null) ret.Add(data);
			}
			return ret.ToArray();
		}

		static public DataBuildSettingVersion getScriptable(string filter = null) 
			=> getScriptables(filter).FirstOrDefault();

	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEditor;

/// <summary>
/// this is meant to store an abstract of the version number of the app
/// is base on each platform format and accessor
/// </summary>

namespace fwp.version
{
	[System.Serializable]
	abstract public class DataBuildSettingVersion : ScriptableObject
	{
		[Header("version")]
		public string version = "0.0.1";
		public int buildNumber = 1;

		[Header("timestamp")]

		public string timestamp_incr = "-never-";
		public string timestamp_build = "-never-";

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

		virtual public string getFormated()
		{
			return version + "@" + buildNumber;
		}

		public string getTimestamps()
		{
			return "incr? " + timestamp_incr + " & build? " + timestamp_build;
		}

#if UNITY_EDITOR
		public void event_build()
		{
			timestamp_build = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

			EditorUtility.SetDirty(this);
		}

		void event_incr()
		{
			timestamp_incr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			EditorUtility.SetDirty(this);
		}

		public void incrementMajor()
		{
			int[] v = getDataVersionInts();

			v[0]++;
			if (v.Length > 1) v[1] = 0;
			if (v.Length > 2) v[2] = 0;

			buildNumber++;
			injectInts(v);

			applyVersionToEditor();
			event_incr(); // +dirty
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
			injectInts(v);

			applyVersionToEditor();
			event_incr(); // +dirty
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
			injectInts(v);

			applyVersionToEditor();
			event_incr(); // +dirty
		}

		void injectInts(int[] vs)
		{
			version = vs[0] + "." + vs[1] + "." + vs[2];
		}

		/// <summary>
		/// describe how to inject version into editor 
		/// project settings > player settings
		/// </summary>
		abstract public void applyVersionToEditor();

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

				UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(DataBuildSettingVersion));
				DataBuildSettingVersion data = obj as DataBuildSettingVersion;
				if (data != null) ret.Add(data);
			}
			return ret.ToArray();
		}

		static public DataBuildSettingVersion getScriptable(string filter = null)
		{
			var ret = getScriptables(filter);
			if (ret.Length > 0) return ret[0];
			return null;
		}
#endif

	}

}

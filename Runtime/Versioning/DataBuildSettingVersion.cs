using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEditor;
using System.IO;

/// <summary>
/// this is meant to store an abstract of the version number of the app
/// is base on each platform format and accessor
/// </summary>

namespace fwp.version
{
	[System.Serializable]
	abstract public class DataBuildSettingVersion : ScriptableObject
	{
		public const char separator = '.';

		[Header("version")]
		[SerializeField] protected int major;
		[SerializeField] protected int minor;
		[SerializeField] protected int patch;

		/// <summary>
		/// incremental number
		/// </summary>
		[SerializeField]protected int buildNumber = 1;
		public int BuildNumber => buildNumber;
		
		public string Version => major + "." + minor + "." + patch;

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
			return Version;
		}

		/// <summary>
		/// int[] [x],[y],[z]
		/// </summary>
		/// <returns></returns>
		public int[] getDataVersionInts() => new int[] { major, minor, patch };

		virtual public string getFormated()
		{
			return Version + "@" + buildNumber;
		}

		public string getTimestamps()
		{
			return "incr? " + timestamp_incr + " & build? " + timestamp_build;
		}

        public override string ToString()
        {
            return getFormated();
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
			major++;
			buildNumber++;
			applyVersionToEditor();
			event_incr(); // +dirty
		}

		public void incrementMinor()
		{
			minor++;
			buildNumber++;
			applyVersionToEditor();
			event_incr(); // +dirty
		}

		public void incrementFix()
		{
			patch++;
			buildNumber++;
			applyVersionToEditor();
			event_incr(); // +dirty
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

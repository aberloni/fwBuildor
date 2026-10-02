using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// platform version : how a DataVersion (X.Y.Z + build number) is applied to a platform
/// multiple platform versions can share the same DataVersion
/// subclasses add platform specific content (ie: switch release)
/// </summary>

namespace fwp.version
{
	[System.Serializable]
	abstract public class DataBuildSettingVersion : ScriptableObject
	{
		public const char separator = DataVersion.separator;

		[Header("version")]
		[Tooltip("X.Y.Z + build number, can be shared by multiple platforms")]
		[SerializeField] protected DataVersion data;

		public DataVersion Data => data;
		public bool HasData => data != null;

		public int BuildNumber => HasData ? data.BuildNumber : 0;

		/// <summary>
		/// X.Y.Z
		/// </summary>
		public string Version => HasData ? data.Version : "0.0.0";

		/// <summary>
		/// x.y.z
		/// </summary>
		virtual public string getDataVersion() => Version;

		/// <summary>
		/// int[] [x],[y],[z]
		/// </summary>
		public int[] getDataVersionInts() => HasData ? data.getDataVersionInts() : new int[3];

		/// <summary>
		/// X.Y.Z@B
		/// </summary>
		virtual public string getFormated() => HasData ? data.getFormated() : "(no DataVersion)";

		public string getTimestamps() => HasData ? data.getTimestamps() : "(no DataVersion)";

		public override string ToString() => getFormated();

#if UNITY_EDITOR

		bool checkData()
		{
			if (HasData) return true;
			Debug.LogError(name + " : no DataVersion assigned", this);
			return false;
		}

		public void event_build()
		{
			if (!checkData()) return;
			data.event_build();
		}

		public void incrementMajor()
		{
			if (!checkData()) return;
			data.incrementMajor();
			applyVersionToEditor();
		}

		public void incrementMinor()
		{
			if (!checkData()) return;
			data.incrementMinor();
			applyVersionToEditor();
		}

		public void incrementFix()
		{
			if (!checkData()) return;
			data.incrementFix();
			applyVersionToEditor();
		}

		/// <summary>
		/// describe how to inject version into editor
		/// project settings > player settings
		/// </summary>
		abstract public void applyVersionToEditor();

		static public DataBuildSettingVersion[] getScriptables(string filter = null)
		{
			string[] all = UnityEditor.AssetDatabase.FindAssets("t:DataBuildSettingVersion");
			if (all.Length <= 0) return null;

			List<DataBuildSettingVersion> ret = new();
			for (int i = 0; i < all.Length; i++)
			{
				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(all[i]);

				if (!string.IsNullOrEmpty(filter))
				{
					if (!path.Contains(filter)) continue;
				}

				UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(DataBuildSettingVersion));
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

#if UNITY_EDITOR
	/// <summary>
	/// platform version inspector : shared DataVersion summary on top
	/// </summary>
	[UnityEditor.CustomEditor(typeof(DataBuildSettingVersion), true)]
	public class DataBuildSettingVersionEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var v = (DataBuildSettingVersion)target;

			if (v.HasData)
			{
				UnityEditor.EditorGUILayout.HelpBox("version : " + v.getFormated() + "\n" + v.getTimestamps(), UnityEditor.MessageType.None);
			}
			else
			{
				UnityEditor.EditorGUILayout.HelpBox("no DataVersion assigned", UnityEditor.MessageType.Warning);
			}

			base.OnInspectorGUI();
		}
	}
#endif

}

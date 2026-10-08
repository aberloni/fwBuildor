using UnityEngine;
using System;

namespace fwp.version
{
	/// <summary>
	/// app version numbers : X.Y.Z + build number
	/// platform agnostic, can be shared by multiple platform versions (DataVersion[Platform])
	/// </summary>
	[CreateAssetMenu(menuName = "buildor/version/+version", order = 99)]
	public class DataVersion : ScriptableObject
	{
		public const char separator = '.';

		[System.Serializable]
		public struct VersionSlot
		{
			[Header("version")]
			public int[] slots; // major, minor, patch

			/// <summary>
			/// X.Y.Z
			/// </summary>
			public string Display
			{
				get
				{
					string ret = string.Empty;
					for (int i = 0; i < slots.Length; i++)
					{
						if (i > 0 && slots.Length > 1) ret += separator;
						ret += slots[i];
					}
					return ret;
				}
			}
		}

		[Header("version")]
		[SerializeField] int major;
		[SerializeField] int minor;
		[SerializeField] int patch;

		/// <summary>
		/// incremental number
		/// </summary>
		[SerializeField] int buildNumber = 1;

		public int Major => major;
		public int Minor => minor;
		public int Patch => patch;
		public int BuildNumber => buildNumber;

		/// <summary>
		/// X.Y.Z
		/// </summary>
		public string Version => major.ToString() + separator + minor + separator + patch;

		[Header("timestamp")]

		public string timestamp_incr = "-never-";
		public string timestamp_build = "-never-";

		/// <summary>
		/// int[] [x],[y],[z]
		/// </summary>
		public int[] getDataVersionInts() => new int[] { major, minor, patch };

		/// <summary>
		/// X.Y.Z@B
		/// </summary>
		public string getFormated() => Version + "@" + buildNumber;

		public string getTimestamps()
		{
			return "incr? " + timestamp_incr + " & build? " + timestamp_build;
		}

		public override string ToString() => getFormated();

#if UNITY_EDITOR

		/// <summary>
		/// platform agnostic : X.Y.Z to PlayerSettings.bundleVersion
		/// platform specifics are applied by platform versions (DataBuildSettingVersion)
		/// </summary>
		public void applyVersionToEditor()
		{
			UnityEditor.PlayerSettings.bundleVersion = Version;
		}

		public void event_build()
		{
			timestamp_build = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			UnityEditor.EditorUtility.SetDirty(this);
		}

		void event_incr()
		{
			timestamp_incr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			UnityEditor.EditorUtility.SetDirty(this);
		}

		/// <summary>
		/// "are you sure" dialog before a MAJOR++ (buttons only)
		/// </summary>
		public bool confirmMajor()
		{
			string next = (major + 1).ToString() + separator + 0 + separator + 0;
			return UnityEditor.EditorUtility.DisplayDialog("MAJOR++",
				name + "\n" + Version + " → " + next + "\n\nare you sure ?", "yes", "no");
		}

		public void incrementMajor()
		{
			patch = 0;
			minor = 0;
			major++;
			buildNumber++;
			event_incr(); // +dirty
		}

		public void incrementMinor()
		{
			patch = 0;
			minor++;
			buildNumber++;
			event_incr(); // +dirty
		}

		public void incrementFix()
		{
			patch++;
			buildNumber++;
			event_incr(); // +dirty
		}
#endif

	}

#if UNITY_EDITOR
	/// <summary>
	/// increment buttons, each one also refreshes increment timestamp
	/// not applied to PlayerSettings (done by platform versions)
	/// </summary>
	[UnityEditor.CustomEditor(typeof(DataVersion))]
	public class DataVersionEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var v = (DataVersion)target;

			UnityEditor.EditorGUILayout.HelpBox("version : " + v.getFormated() + "\n" + v.getTimestamps(), UnityEditor.MessageType.None);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("MAJOR++") && v.confirmMajor()) { UnityEditor.Undo.RecordObject(v, "MAJOR++"); v.incrementMajor(); }
			if (GUILayout.Button("MINOR++")) { UnityEditor.Undo.RecordObject(v, "MINOR++"); v.incrementMinor(); }
			if (GUILayout.Button("PATCH++")) { UnityEditor.Undo.RecordObject(v, "PATCH++"); v.incrementFix(); }
			GUILayout.EndHorizontal();

			GUILayout.Space(10f);

			base.OnInspectorGUI();
		}
	}
#endif

}

#if UNITY_EDITOR
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace fwp.version
{
	/// <summary>
	/// nintendo sdk & .nmeta tools
	/// shared by DataVersionSwitch inspector, nmeta window & build preprocess
	///
	/// nmeta : ReleaseVersion (DataVersionSwitch.release), DisplayVersion (X.Y.Z)
	/// </summary>
	static public class SwitchNmeta
	{
		public const string env_sdk = "NINTENDO_SDK_ROOT";

		/// <summary>
		/// unity process env : what the switch build pipeline will see
		/// </summary>
		static public string SdkRootProcess => Environment.GetEnvironmentVariable(env_sdk);

		/// <summary>
		/// user/machine env : may be set after unity was launched
		/// </summary>
		static public string SdkRootSystem =>
			Environment.GetEnvironmentVariable(env_sdk, EnvironmentVariableTarget.User)
			?? Environment.GetEnvironmentVariable(env_sdk, EnvironmentVariableTarget.Machine);

		/// <summary>
		/// process first, system as fallback
		/// </summary>
		static public string SdkRoot => !string.IsNullOrEmpty(SdkRootProcess) ? SdkRootProcess : SdkRootSystem;

		static public bool isSdkRootValid(string root) => !string.IsNullOrEmpty(root) && Directory.Exists(Path.Combine(root, "Tools"));

		/// <summary>
		/// build requirement : env var visible to unity process & has Tools/
		/// logs reason on failure
		/// </summary>
		static public bool checkSdk()
		{
			string root = SdkRootProcess;

			if (string.IsNullOrEmpty(root))
			{
				Debug.LogError(env_sdk + " is not set");

				if (!string.IsNullOrEmpty(SdkRootSystem))
				{
					Debug.LogError(env_sdk + " exists in system env vars : restart unity (and hub) to catch it");
				}

				return false;
			}

			if (!Directory.Exists(root))
			{
				Debug.LogError(env_sdk + " folder doesn't exist @" + root);
				return false;
			}

			if (!isSdkRootValid(root))
			{
				Debug.LogError(env_sdk + " has no Tools/ folder, wrong sdk root ? @" + root);
				return false;
			}

			Debug.Log(env_sdk + " ok @" + root);
			return true;
		}

		static public void openEnvironmentVariables()
		{
			System.Diagnostics.Process.Start("rundll32.exe", "sysdm.cpl,EditEnvironmentVariables");
		}

		/// <summary>
		/// first Authoring*.exe within sdk Tools/
		/// </summary>
		static public string findAuthoringTool()
		{
			string root = SdkRoot;
			if (!isSdkRootValid(root)) return null;

			return Directory.EnumerateFiles(Path.Combine(root, "Tools"), "Authoring*.exe", SearchOption.AllDirectories)
				.FirstOrDefault();
		}

		static public void openWithAuthoringTool(string nmetaPath)
		{
			string exe = findAuthoringTool();
			if (string.IsNullOrEmpty(exe))
			{
				Debug.LogError("no Authoring*.exe found in " + env_sdk + "/Tools/");
				return;
			}

			System.Diagnostics.Process.Start(exe, "\"" + nmetaPath + "\"");
		}

		// --- nmeta file

		/// <summary>
		/// nmeta file path, per machine & per project
		/// </summary>
		static string PrefNmetaPath => "buildor.nmeta.path." + UnityEditor.PlayerSettings.productGUID;

		static public string NmetaPath
		{
			get => UnityEditor.EditorPrefs.GetString(PrefNmetaPath, string.Empty);
			set => UnityEditor.EditorPrefs.SetString(PrefNmetaPath, value);
		}

		static public bool HasNmeta => !string.IsNullOrEmpty(NmetaPath) && File.Exists(NmetaPath);

		// read cache : don't parse xml every gui frame
		static string _cachePath;
		static DateTime _cacheTime;
		static string _cacheRelease;
		static string _cacheDisplay;

		/// <summary>
		/// ReleaseVersion & DisplayVersion of nmeta file
		/// </summary>
		static public void read(string nmetaPath, out string release, out string display)
		{
			release = display = string.Empty;
			if (string.IsNullOrEmpty(nmetaPath) || !File.Exists(nmetaPath)) return;

			DateTime time = File.GetLastWriteTime(nmetaPath);
			if (_cachePath != nmetaPath || _cacheTime != time)
			{
				XDocument doc = XDocument.Load(nmetaPath);
				_cacheRelease = doc.Descendants("ReleaseVersion").FirstOrDefault()?.Value ?? "N/A";
				_cacheDisplay = doc.Descendants("DisplayVersion").FirstOrDefault()?.Value ?? "N/A";
				_cachePath = nmetaPath;
				_cacheTime = time;
			}

			release = _cacheRelease;
			display = _cacheDisplay;
		}

		/// <summary>
		/// write ReleaseVersion & DisplayVersion into nmeta file
		/// </summary>
		static public void inject(string nmetaPath, string displayVersion, int releaseVersion)
		{
			XDocument doc = XDocument.Load(nmetaPath);

			setElement(doc, "ReleaseVersion", releaseVersion.ToString());
			setElement(doc, "DisplayVersion", displayVersion);

			doc.Save(nmetaPath);
			Debug.Log($"nmeta updated : ReleaseVersion={releaseVersion}, DisplayVersion={displayVersion} ({nmetaPath})");
		}

		static void setElement(XDocument doc, string name, string value)
		{
			XElement elem = doc.Descendants(name).FirstOrDefault();
			if (elem == null) throw new Exception($"{name} not found in nmeta file");
			elem.Value = value;
		}

		static public void inject(string nmetaPath, DataVersionSwitch version)
		{
			if (version == null || !version.HasData)
			{
				Debug.LogError("nmeta inject : no switch version (or no DataVersion)");
				return;
			}

			inject(nmetaPath, version.Version, version.VersionRelease);
		}

		// --- gui

		/// <summary>
		/// nmeta file, values vs switch version, inject & tools, sdk status
		/// version can be null (no inject)
		/// </summary>
		static public void drawGUI(DataVersionSwitch version)
		{
			GUILayout.Label("nmeta", UnityEditor.EditorStyles.boldLabel);

			// file
			GUILayout.BeginHorizontal();
			string path = NmetaPath;
			UnityEditor.EditorGUILayout.LabelField("file", string.IsNullOrEmpty(path) ? "-none-" : path);
			if (GUILayout.Button("browse", GUILayout.Width(70f)))
			{
				string folder = string.IsNullOrEmpty(path) ? string.Empty : Path.GetDirectoryName(path);
				string picked = UnityEditor.EditorUtility.OpenFilePanel("Select .nmeta file", folder, "nmeta");
				if (!string.IsNullOrEmpty(picked)) NmetaPath = picked;
			}
			GUILayout.EndHorizontal();

			bool hasNmeta = HasNmeta;

			// values : nmeta vs switch version
			if (hasNmeta)
			{
				read(NmetaPath, out string release, out string display);
				drawCompare("ReleaseVersion", release, version != null ? version.VersionRelease.ToString() : null);
				drawCompare("DisplayVersion", display, version != null ? version.Version : null);
			}
			else if (!string.IsNullOrEmpty(path))
			{
				UnityEditor.EditorGUILayout.HelpBox("nmeta file not found", UnityEditor.MessageType.Warning);
			}

			// actions
			GUILayout.BeginHorizontal();
			GUI.enabled = hasNmeta && version != null && version.HasData;
			if (GUILayout.Button("inject version")) inject(NmetaPath, version);
			GUI.enabled = hasNmeta;
			if (GUILayout.Button("open file")) UnityEditor.EditorUtility.OpenWithDefaultApp(NmetaPath);
			GUI.enabled = hasNmeta && isSdkRootValid(SdkRoot);
			if (GUILayout.Button("open authoring")) openWithAuthoringTool(NmetaPath);
			GUI.enabled = true;
			GUILayout.EndHorizontal();

			drawSdkGUI();
		}

		static void drawCompare(string label, string nmetaValue, string versionValue)
		{
			bool same = versionValue == null || nmetaValue == versionValue;

			GUIStyle style = new GUIStyle(UnityEditor.EditorStyles.label);
			if (!same) style.normal.textColor = new Color(1f, 0.4f, 0.4f);

			string txt = nmetaValue;
			if (!same) txt += "  ≠  " + versionValue + " (version)";
			UnityEditor.EditorGUILayout.LabelField(label, txt, style);
		}

		/// <summary>
		/// NINTENDO_SDK_ROOT status
		/// </summary>
		static public void drawSdkGUI()
		{
			string process = SdkRootProcess;
			string root = SdkRoot;
			bool valid = isSdkRootValid(root);

			GUIStyle style = new GUIStyle(UnityEditor.EditorStyles.label);
			style.normal.textColor = valid ? new Color(0.4f, 0.9f, 0.4f) : new Color(1f, 0.4f, 0.4f);

			string status = valid ? "OK" : "NOK - Tools/ folder not found";
			if (valid && string.IsNullOrEmpty(process)) status = "OK in system env, not seen by unity : restart unity";

			GUILayout.BeginHorizontal();
			UnityEditor.EditorGUILayout.LabelField(env_sdk, (root ?? "<not set>") + "  " + status, style);
			if (!valid && GUILayout.Button("env vars", GUILayout.Width(70f))) openEnvironmentVariables();
			GUILayout.EndHorizontal();
		}
	}
}
#endif

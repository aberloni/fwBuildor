using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

/// <summary>
/// Serializable config that defines what gets copied into StreamingAssets for a given platform.
/// Create one ScriptableObject per platform and wire them into your build pipeline.
/// </summary>
[CreateAssetMenu(menuName = "Build/StreamingAssets Copy Config", fileName = "StreamingAssetsCopyConfig")]
public class StreamingAssetsCopyConfig : ScriptableObject
{
    [Tooltip("Source folder path, relative to the project root (i.e. sibling of Assets/).")]
    public string sourceFolderPath = "../MyExternalAssets";

    [Tooltip("Destination path relative to StreamingAssets/.")]
    public string destinationSubFolder = "Data";

    [Tooltip("Target build platform this config applies to.")]
    public BuildTarget targetPlatform = BuildTarget.StandaloneWindows64;

    // ── Filtering ──────────────────────────────────────────────────────────────

    [Header("Extension filters (e.g. .png  .json)  — leave both empty to allow all")]
    [Tooltip("Only copy files whose extension is in this list. Checked before blacklist.")]
    public List<string> extensionWhitelist = new();

    [Tooltip("Never copy files whose extension is in this list.")]
    public List<string> extensionBlacklist = new();

    [Header("Filename pattern filters (substring match, case-insensitive)")]
    [Tooltip("Only copy files whose name contains at least one of these patterns. Empty = allow all.")]
    public List<string> namePatternWhitelist = new();

    [Tooltip("Never copy files whose name contains any of these patterns.")]
    public List<string> namePatternBlacklist = new();

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the resolved absolute source path.
    /// </summary>
    public string ResolvedSourcePath =>
        Path.GetFullPath(Path.Combine(Application.dataPath, "..", sourceFolderPath));

    /// <summary>
    /// Returns the resolved absolute destination path inside StreamingAssets.
    /// </summary>
    public string ResolvedDestinationPath =>
        Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, destinationSubFolder));

    /// <summary>
    /// Executes the copy. Call this from a build pre-process step or manually from the editor.
    /// </summary>
    public void Execute()
    {
        string src = ResolvedSourcePath;
        string dst = ResolvedDestinationPath;

        if (!Directory.Exists(src))
        {
            Debug.LogError($"[StreamingAssetsCopyConfig] Source folder not found: {src}");
            return;
        }

        int copied = 0, skipped = 0;

        foreach (string filePath in Directory.EnumerateFiles(src, "*", SearchOption.AllDirectories))
        {
            if (!PassesFilter(filePath)) { skipped++; continue; }

            string relative = Path.GetRelativePath(src, filePath);
            string target   = Path.Combine(dst, relative);

            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(filePath, target, overwrite: true);
            copied++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"[StreamingAssetsCopyConfig] '{name}' — {copied} file(s) copied, {skipped} skipped.");
    }

    /// <summary>
    /// Removes all files in the destination folder that were sourced from this config.
    /// </summary>
    public void Clean()
    {
        string dst = ResolvedDestinationPath;
        if (!Directory.Exists(dst)) return;

        Directory.Delete(dst, recursive: true);
        AssetDatabase.Refresh();
        Debug.Log($"[StreamingAssetsCopyConfig] '{name}' — destination cleaned: {dst}");
    }

    // ── Filtering internals ────────────────────────────────────────────────────

    private bool PassesFilter(string filePath)
    {
        string fileName  = Path.GetFileName(filePath);
        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        string nameLower = fileName.ToLowerInvariant();

        // Extension whitelist
        if (extensionWhitelist.Count > 0 &&
            !extensionWhitelist.Any(e => e.ToLowerInvariant() == extension))
            return false;

        // Extension blacklist
        if (extensionBlacklist.Any(e => e.ToLowerInvariant() == extension))
            return false;

        // Name pattern whitelist
        if (namePatternWhitelist.Count > 0 &&
            !namePatternWhitelist.Any(p => nameLower.Contains(p.ToLowerInvariant())))
            return false;

        // Name pattern blacklist
        if (namePatternBlacklist.Any(p => nameLower.Contains(p.ToLowerInvariant())))
            return false;

        return true;
    }
}

// ── Custom Inspector ───────────────────────────────────────────────────────────

[CustomEditor(typeof(StreamingAssetsCopyConfig))]
public class StreamingAssetsCopyConfigEditor : Editor
{
    // Cached button style with fixed width so Browse/Clear don't stretch
    private static GUILayoutOption[] _btnOpts;
    private static GUILayoutOption[] BtnOpts =>
        _btnOpts ??= new GUILayoutOption[] { GUILayout.Width(64) };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var cfg = (StreamingAssetsCopyConfig)target;

        // ── Source path ────────────────────────────────────────────────────────
        EditorGUILayout.LabelField("Source Folder", EditorStyles.boldLabel);
        DrawFolderRow(
            serializedObject.FindProperty(nameof(StreamingAssetsCopyConfig.sourceFolderPath)),
            browseTitle: "Select source folder",
            browseRoot:  Path.GetFullPath(Path.Combine(Application.dataPath, "..")),
            makeRelativeTo: Path.GetFullPath(Path.Combine(Application.dataPath, ".."))
        );

        EditorGUILayout.Space(4);

        // ── Destination sub-folder ─────────────────────────────────────────────
        EditorGUILayout.LabelField("Destination Sub-folder  (inside StreamingAssets/)", EditorStyles.boldLabel);
        DrawFolderRow(
            serializedObject.FindProperty(nameof(StreamingAssetsCopyConfig.destinationSubFolder)),
            browseTitle: "Select destination folder inside StreamingAssets",
            browseRoot:  Application.streamingAssetsPath,
            makeRelativeTo: Application.streamingAssetsPath
        );

        EditorGUILayout.Space(6);

        // ── Rest of the fields (platform + filters) ────────────────────────────
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(StreamingAssetsCopyConfig.targetPlatform)));

        EditorGUILayout.Space(4);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(StreamingAssetsCopyConfig.extensionWhitelist)), true);
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(StreamingAssetsCopyConfig.extensionBlacklist)), true);
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(StreamingAssetsCopyConfig.namePatternWhitelist)), true);
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(StreamingAssetsCopyConfig.namePatternBlacklist)), true);

        serializedObject.ApplyModifiedProperties();

        // ── Resolved paths info ────────────────────────────────────────────────
        EditorGUILayout.Space(6);
        EditorGUILayout.HelpBox(
            $"Source      : {cfg.ResolvedSourcePath}\nDestination : {cfg.ResolvedDestinationPath}",
            MessageType.None);

        // ── Actions ────────────────────────────────────────────────────────────
        EditorGUILayout.Space(4);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Copy Now"))
                cfg.Execute();

            if (GUILayout.Button("Clean Destination"))
            {
                if (EditorUtility.DisplayDialog("Clean?",
                        $"Delete all files in:\n{cfg.ResolvedDestinationPath}?", "Delete", "Cancel"))
                    cfg.Clean();
            }
        }
    }

    /// <summary>
    /// Draws a single-line row:  [ text field (flex) ] [ Browse ] [ Clear ]
    /// </summary>
    private void DrawFolderRow(
        SerializedProperty prop,
        string browseTitle,
        string browseRoot,
        string makeRelativeTo)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.PropertyField(prop, GUIContent.none);

            if (GUILayout.Button("Browse", BtnOpts))
            {
                // Ensure the panel opens at a sensible location
                string startPath = string.IsNullOrEmpty(prop.stringValue)
                    ? browseRoot
                    : Path.GetFullPath(Path.Combine(makeRelativeTo, prop.stringValue));

                if (!Directory.Exists(startPath))
                    startPath = browseRoot;

                string chosen = EditorUtility.OpenFolderPanel(browseTitle, startPath, "");

                if (!string.IsNullOrEmpty(chosen))
                {
                    // Store as relative path when possible, absolute otherwise
                    try
                    {
                        prop.stringValue = Path.GetRelativePath(makeRelativeTo, chosen);
                    }
                    catch
                    {
                        prop.stringValue = chosen;
                    }
                }
            }

            if (GUILayout.Button("Clear", BtnOpts))
                prop.stringValue = string.Empty;
        }
    }
}

// ── Optional: auto-run before build ───────────────────────────────────────────

/// <summary>
/// Finds every StreamingAssetsCopyConfig whose targetPlatform matches the active build target
/// and runs it automatically before the build starts.
/// Remove or comment out this class if you prefer to trigger copies manually.
/// </summary>
public class StreamingAssetsBuildPreProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        string[] guids = AssetDatabase.FindAssets($"t:{nameof(StreamingAssetsCopyConfig)}");

        foreach (string guid in guids)
        {
            var cfg = AssetDatabase.LoadAssetAtPath<StreamingAssetsCopyConfig>(
                AssetDatabase.GUIDToAssetPath(guid));

            if (cfg != null && cfg.targetPlatform == report.summary.platform)
            {
                Debug.Log($"[Build] Running StreamingAssetsCopyConfig: {cfg.name}");
                cfg.Execute();
            }
        }
    }
}
#endif
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;
using UnityEditor;

namespace fwp.buildor
{

    /// <summary>
    /// Serializable config that defines what gets copied into StreamingAssets for a given platform.
    /// Create one ScriptableObject per platform and wire them into your build pipeline.
    /// </summary>
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+streamingassetcopy", fileName = "BoduleStreamingAssetsCopy")]
    public class BoduleStreamingAssetsCopy : BuildModule
    {
        [Tooltip("Source folder path, relative to the project root (i.e. sibling of Assets/).")]
        public string sourceFolderPath = "../MyExternalAssets";

        [Tooltip("Destination path relative to StreamingAssets/.")]
        public string destinationSubFolder = "Data";

        [Tooltip("Target build platform this config applies to.")]
        public BuildTarget targetPlatform = BuildTarget.StandaloneWindows64;

        // ── Filtering ──────────────────────────────────────────────────────────────

        [Header("Extension filters (e.g. png  or  .png) — leave both empty to allow all")]
        [Tooltip("Only copy files whose extension matches. Dot is optional. Checked before blacklist.")]
        public List<string> extensionWhitelist = new();

        [Tooltip("Never copy files whose extension matches. Dot is optional.")]
        public List<string> extensionBlacklist = new();

        [Header("Path pattern filters (substring match against full path, case-insensitive)")]
        [Tooltip("Only copy files whose full path contains at least one of these patterns. Empty = allow all.")]
        public List<string> pathPatternWhitelist = new();

        [Tooltip("Never copy files whose full path contains any of these patterns.")]
        public List<string> pathPatternBlacklist = new();

        // ── Public API ─────────────────────────────────────────────────────────────

        public string ResolvedSourcePath =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "..", sourceFolderPath));

        public string ResolvedDestinationPath =>
            Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, destinationSubFolder));

        /// <summary>
        /// Returns all files that would be copied, as (relativePath, absoluteSourcePath) pairs.
        /// </summary>
        public List<(string relative, string absolute)> GetTargetedFiles()
        {
            var result = new List<(string, string)>();
            string src = ResolvedSourcePath;

            if (!Directory.Exists(src)) return result;

            foreach (string filePath in Directory.EnumerateFiles(src, "*", SearchOption.AllDirectories))
            {
                if (!PassesFilter(filePath)) continue;
                result.Add((Path.GetRelativePath(src, filePath), filePath));
            }

            return result;
        }

        /// <summary>
        /// Executes the copy. Call this from a build pre-process step or manually from the editor.
        /// </summary>
        public override void Apply()
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
                string target = Path.Combine(dst, relative);

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

        // Normalizes an extension entry so it always starts with a dot.
        private static string NormalizeExtension(string raw)
        {
            raw = raw.Trim().ToLowerInvariant();
            return raw.StartsWith(".") ? raw : "." + raw;
        }

        private bool PassesFilter(string filePath)
        {
            string pathLower = filePath.Replace('\\', '/').ToLowerInvariant();
            string extension = Path.GetExtension(filePath).ToLowerInvariant(); // always has dot

            // Extension whitelist
            if (extensionWhitelist.Count > 0 &&
                !extensionWhitelist.Any(e => NormalizeExtension(e) == extension))
                return false;

            // Extension blacklist
            if (extensionBlacklist.Any(e => NormalizeExtension(e) == extension))
                return false;

            // Path pattern whitelist — matched against the full absolute path
            if (pathPatternWhitelist.Count > 0 &&
                !pathPatternWhitelist.Any(p => pathLower.Contains(p.Trim().ToLowerInvariant())))
                return false;

            // Path pattern blacklist — matched against the full absolute path
            if (pathPatternBlacklist.Any(p => pathLower.Contains(p.Trim().ToLowerInvariant())))
                return false;

            return true;
        }
    }

    // ── Custom Inspector ───────────────────────────────────────────────────────────

    [CustomEditor(typeof(BoduleStreamingAssetsCopy))]
    public class StreamingAssetsCopyConfigEditor : Editor
    {
        private static GUILayoutOption[] _btnOpts;
        private static GUILayoutOption[] BtnOpts =>
            _btnOpts ??= new GUILayoutOption[] { GUILayout.Width(64) };

        private List<(string relative, string absolute)> _preview;
        private Vector2 _previewScroll;
        private bool _previewFoldout = true;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var cfg = (BoduleStreamingAssetsCopy)target;

            // ── Source path ────────────────────────────────────────────────────────
            EditorGUILayout.LabelField("Source Folder", EditorStyles.boldLabel);
            DrawFolderRow(
                serializedObject.FindProperty(nameof(BoduleStreamingAssetsCopy.sourceFolderPath)),
                browseTitle: "Select source folder",
                browseRoot: Path.GetFullPath(Path.Combine(Application.dataPath, "..")),
                makeRelativeTo: Path.GetFullPath(Path.Combine(Application.dataPath, "..")),
                absolutePath: cfg.ResolvedSourcePath
            );
            DrawAbsolutePath(cfg.ResolvedSourcePath);

            EditorGUILayout.Space(4);

            // ── Destination sub-folder ─────────────────────────────────────────────
            EditorGUILayout.LabelField("Destination Sub-folder  (inside StreamingAssets/)", EditorStyles.boldLabel);
            DrawFolderRow(
                serializedObject.FindProperty(nameof(BoduleStreamingAssetsCopy.destinationSubFolder)),
                browseTitle: "Select destination folder inside StreamingAssets",
                browseRoot: Application.streamingAssetsPath,
                makeRelativeTo: Application.streamingAssetsPath,
                absolutePath: cfg.ResolvedDestinationPath
            );
            DrawAbsolutePath(cfg.ResolvedDestinationPath);

            EditorGUILayout.Space(6);

            // ── Rest of the fields (platform + filters) ────────────────────────────
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(nameof(BoduleStreamingAssetsCopy.targetPlatform)));

            EditorGUILayout.Space(4);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(nameof(BoduleStreamingAssetsCopy.extensionWhitelist)), true);
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(nameof(BoduleStreamingAssetsCopy.extensionBlacklist)), true);
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(nameof(BoduleStreamingAssetsCopy.pathPatternWhitelist)), true);
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty(nameof(BoduleStreamingAssetsCopy.pathPatternBlacklist)), true);

            serializedObject.ApplyModifiedProperties();

            // ── Preview ────────────────────────────────────────────────────────────
            EditorGUILayout.Space(6);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Preview Files"))
                {
                    _preview = cfg.GetTargetedFiles();
                    _previewFoldout = true;
                }

                if (_preview != null && GUILayout.Button("Clear Preview", BtnOpts))
                    _preview = null;
            }

            if (_preview != null)
            {
                EditorGUILayout.Space(2);
                _previewFoldout = EditorGUILayout.Foldout(_previewFoldout,
                    $"Targeted files — {_preview.Count}", toggleOnLabelClick: true);

                if (_previewFoldout)
                {
                    float rowH = EditorGUIUtility.singleLineHeight;
                    float maxH = rowH * 12 + 4;
                    float totalH = rowH * Mathf.Max(_preview.Count, 1) + 4;

                    _previewScroll = EditorGUILayout.BeginScrollView(
                        _previewScroll, GUILayout.Height(Mathf.Min(totalH, maxH)));

                    var dimStyle = new GUIStyle(EditorStyles.miniLabel)
                    { normal = { textColor = Color.gray } };

                    if (_preview.Count == 0)
                    {
                        EditorGUILayout.LabelField("  (no files match the current filters)", dimStyle);
                    }
                    else
                    {
                        foreach (var (relative, absolute) in _preview)
                        {
                            var rect = EditorGUILayout.GetControlRect(false, rowH);
                            EditorGUI.LabelField(rect, "  " + relative, dimStyle);

                            // Hover tooltip shows the full absolute path
                            if (rect.Contains(Event.current.mousePosition))
                                GUI.Label(rect, new GUIContent("", absolute));
                        }
                    }

                    EditorGUILayout.EndScrollView();
                }
            }

            // ── Actions ────────────────────────────────────────────────────────────
            EditorGUILayout.Space(4);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Copy Now"))
                {
                    cfg.Apply();
                    _preview = null;
                }

                if (GUILayout.Button("Clean Destination"))
                {
                    if (EditorUtility.DisplayDialog("Clean?",
                            $"Delete all files in:\n{cfg.ResolvedDestinationPath}?", "Delete", "Cancel"))
                        cfg.Clean();
                }
            }
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Draws a single-line row:  [ text field (flex) ] [ Browse ] [ Open ] [ Clear ]
        /// </summary>
        private void DrawFolderRow(
            SerializedProperty prop,
            string browseTitle,
            string browseRoot,
            string makeRelativeTo,
            string absolutePath)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(prop, GUIContent.none);

                if (GUILayout.Button("Browse", BtnOpts))
                {
                    string startPath = string.IsNullOrEmpty(prop.stringValue)
                        ? browseRoot
                        : Path.GetFullPath(Path.Combine(makeRelativeTo, prop.stringValue));

                    if (!Directory.Exists(startPath))
                        startPath = browseRoot;

                    string chosen = EditorUtility.OpenFolderPanel(browseTitle, startPath, "");

                    if (!string.IsNullOrEmpty(chosen))
                    {
                        try { prop.stringValue = Path.GetRelativePath(makeRelativeTo, chosen); }
                        catch { prop.stringValue = chosen; }
                    }
                }

                using (new EditorGUI.DisabledScope(!Directory.Exists(absolutePath)))
                {
                    if (GUILayout.Button("Open", BtnOpts))
                        EditorUtility.RevealInFinder(absolutePath);
                }

                if (GUILayout.Button("Clear", BtnOpts))
                    prop.stringValue = string.Empty;
            }
        }

        /// <summary>
        /// Draws the resolved absolute path as a small read-only label with a tooltip on hover.
        /// </summary>
        private static void DrawAbsolutePath(string absolutePath)
        {
            bool exists = Directory.Exists(absolutePath);

            var style = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = exists ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.85f, 0.4f, 0.4f) },
                padding = new RectOffset(4, 0, 0, 2),
            };

            var content = new GUIContent(
                (exists ? "" : "\u26a0 ") + absolutePath,   // ⚠ prefix when folder is missing
                absolutePath                                  // full path in tooltip on hover
            );

            var rect = EditorGUILayout.GetControlRect(false, EditorStyles.miniLabel.lineHeight + 2);
            EditorGUI.LabelField(rect, content, style);
        }
    }

    /*
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
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(BoduleStreamingAssetsCopy)}");

            foreach (string guid in guids)
            {
                var cfg = AssetDatabase.LoadAssetAtPath<BoduleStreamingAssetsCopy>(
                    AssetDatabase.GUIDToAssetPath(guid));

                if (cfg != null && cfg.targetPlatform == report.summary.platform)
                {
                    Debug.Log($"[Build] Running: {cfg.name}");
                    cfg.Apply();
                }
            }
        }
    }
    */

}
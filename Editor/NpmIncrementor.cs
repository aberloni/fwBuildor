using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace fwp.buildor.editor
{
    /// <summary>
    /// increment version of a package.json (npm package, not app version)
    /// right click on any asset/folder within a package (project window)
    /// only embedded or local (file:) packages, others are read-only
    /// </summary>
    static public class NpmIncrementor
    {
        const string _menu = "Assets/Package version/";

        // "version": "X.Y.Z"
        static readonly Regex rgxVersion = new Regex("\"version\"\\s*:\\s*\"(\\d+)\\.(\\d+)\\.(\\d+)\"");

        [MenuItem(_menu + "MAJOR++", false, 2000)] static void miMajor() => incrementSelection(0);
        [MenuItem(_menu + "MINOR++", false, 2001)] static void miMinor() => incrementSelection(1);
        [MenuItem(_menu + "PATCH++", false, 2002)] static void miPatch() => incrementSelection(2);

        [MenuItem(_menu + "MAJOR++", true)]
        [MenuItem(_menu + "MINOR++", true)]
        [MenuItem(_menu + "PATCH++", true)]
        static bool miValidate() => getSelectedPackages().Count > 0;

        static void incrementSelection(int slot)
        {
            List<string> updated = new();
            foreach (var p in getSelectedPackages())
            {
                if (increment(p, slot)) updated.Add(p.assetPath + "/package.json"); // Packages/[name]/package.json
            }

            AssetDatabase.Refresh();

            // select updated package.json file(s)
            List<Object> files = new();
            foreach (string path in updated)
            {
                var file = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (file != null) files.Add(file);
            }

            if (files.Count > 0)
            {
                Selection.objects = files.ToArray();
                EditorGUIUtility.PingObject(files[0]);
            }

            Client.Resolve(); // make package manager catch up with new version
        }

        /// <summary>
        /// distinct writable packages containing selected assets
        /// </summary>
        static List<PackageInfo> getSelectedPackages()
        {
            List<PackageInfo> ret = new();
            foreach (string guid in Selection.assetGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path)) continue;

                var p = PackageInfo.FindForAssetPath(path);
                if (p == null) continue;
                if (p.source != PackageSource.Embedded && p.source != PackageSource.Local) continue;
                if (ret.Exists(x => x.name == p.name)) continue;

                ret.Add(p);
            }
            return ret;
        }

        /// <summary>
        /// slot : 0 major, 1 minor, 2 patch
        /// lower slots are reset to 0
        /// </summary>
        static public bool increment(PackageInfo package, int slot)
        {
            string path = Path.Combine(package.resolvedPath, "package.json");
            if (!File.Exists(path))
            {
                Debug.LogError("no package.json @" + path);
                return false;
            }

            string content = File.ReadAllText(path);

            Match m = rgxVersion.Match(content);
            if (!m.Success)
            {
                Debug.LogError(package.name + " : no X.Y.Z version found in " + path);
                return false;
            }

            int[] v = new int[]
            {
                int.Parse(m.Groups[1].Value),
                int.Parse(m.Groups[2].Value),
                int.Parse(m.Groups[3].Value),
            };

            string before = string.Join(".", v);

            v[slot]++;
            for (int i = slot + 1; i < v.Length; i++) v[i] = 0;

            string after = string.Join(".", v);

            // replace only version value, keep file formatting
            content = content.Substring(0, m.Index)
                + "\"version\": \"" + after + "\""
                + content.Substring(m.Index + m.Length);

            File.WriteAllText(path, content);

            Debug.Log(package.name + " : " + before + " → <b>" + after + "</b>");
            return true;
        }
    }
}

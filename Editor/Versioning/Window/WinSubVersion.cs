using UnityEngine;
using UnityEditor;

namespace fwp.version.editor
{
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public class WinSubVersion
    {
        readonly GUILayoutOption bS = GUILayout.Width(75f);
        readonly GUILayoutOption bSM = GUILayout.Width(90f);
        readonly GUILayoutOption bM = GUILayout.Width(105f);
        
        public void drawVersion(DataBuildSettingVersion version, bool controls = true)
        {
            if (version == null) return;

            GUILayout.BeginHorizontal();

            GUI.enabled = false;
            EditorGUILayout.ObjectField(version, typeof(DataBuildSettingVersion), true);
            GUI.enabled = true;

            if (version != null && controls)
            {
                GUILayout.Label(version.getFormated(), bS);

                if (GUILayout.Button("MAJOR", bS))
                {
                    version.incrementMajor();
                }
                if (GUILayout.Button("MINOR", bSM))
                {
                    version.incrementMinor();
                }
                if (GUILayout.Button("FIX", bM))
                {
                    version.incrementFix();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("last incremented: " + version.timestamp_incr);
            GUILayout.Label("last built: " + version.timestamp_build);
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
        }

        //[MenuItem("Test/dump version")]
        //static void dumpTest() => dumpVersion(Application.dataPath);

        /// <summary>
        /// https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-write-text-to-a-file
        /// </summary>
        /// <param name="path"></param>
        static public void dumpVersion(string path = null)
        {
            if (path == null) path = Application.dataPath;

            var item = DataBuildSettingVersion.getScriptable();
            if (item == null)
            {
                Debug.LogWarning("no version present");
                return;
            }

            path = Path.Combine(path, "version.md");

            Debug.Log(path);

            File.WriteAllText(path, item.getFormated());
        }
    }

}

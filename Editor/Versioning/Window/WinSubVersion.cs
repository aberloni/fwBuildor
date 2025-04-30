using UnityEngine;
using UnityEditor;

namespace fwp.version.editor
{
    using fwp.buildor;
    using System.IO;

	public class WinSubVersion
    {

        /// <summary>
        /// result is stored in flagsBuild
        /// </summary>
        virtual public void draw(DataBuildSettingVersion version)
        {

            GUILayout.Space(20f);
            GUILayout.Label("version", BuildorHelperGuiStyle.getCategoryBold());

            GUILayout.BeginHorizontal();

            GUI.enabled = false;
            //var version = getActiveVersion(win);

            EditorGUILayout.ObjectField(version, typeof(DataBuildSettingVersion), true);
            GUI.enabled = true;

            if (version != null)
            {
                GUILayout.Label(version.getFormated());

                if (GUILayout.Button("MAJOR"))
                {
                    version.incrementMajor();
                }
                if (GUILayout.Button("MINOR"))
                {
                    version.incrementMinor();
                }
                if (GUILayout.Button("FIX"))
                {
                    version.incrementFix();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("version manager output : " + VersionManager.getDisplayVersion());

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
            if(item == null)
            {
                Debug.LogWarning("no version present");
                return;
            }

            path = Path.Combine(path, "version.md");

            Debug.Log(path);

            File.WriteAllText(path, item.getFormated());
            
            /*
			byte[] output = null;
            try
            {
				output = SaveFileSystem.serializeObject(item);

				path = System.IO.Path.Combine(path, "version.md");
				Debug.Log(path);

				SaveFileSystemWindows.fsys.writeBytes(path, output);
			}
            catch
            {
                Debug.LogError("cant serialize : " + item, item);
            }
            */
        }
    }

}

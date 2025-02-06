using UnityEngine;

using UnityEditor;
using fwp.buildor.version;

namespace fwp.buildor.editor
{
    public class WinSubVersion
    {

        /// <summary>
        /// result is stored in flagsBuild
        /// </summary>
        public void draw(WinEdBuildor win)
        {

            GUILayout.Space(20f);
            GUILayout.Label("version", BuildorHelperGuiStyle.getCategoryBold());

            BuildSettingVersionType vType = BuildorWinEdHelper.drawEnum<BuildSettingVersionType>("publish type", "publish", 0);
            win.parameters.buildFlags.isPublishingBuild = vType == BuildSettingVersionType.vPublish;

            GUILayout.BeginHorizontal();

            GUI.enabled = false;
            var version = getActiveVersion(win);

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

        public DataBuildSettingVersion getActiveVersion(WinEdBuildor win)
        {
            var version = win.parameters.buildFlags.isPublishingBuild 
                ? win.activeProfil.publishVersion : win.activeProfil.internalVersion;

            return version;
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using fwp.buildor.version;

namespace fwp.buildor.editor
{
    public class WinEdBuildor : EditorWindow
    {

        [MenuItem("Buildor/(winEd) open buildor", false, 0)]
        static void init()
        {
            EditorWindow.GetWindow(typeof(WinEdBuildor));
        }

        Vector2 scroll;

        void OnGUI()
        {
            GUILayout.Label("Buildor", BuildorHelperGuiStyle.getWinTitle());

            scroll = GUILayout.BeginScrollView(scroll);
            DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();

            GUILayout.Label("platform", BuildorHelperGuiStyle.getCategoryBold());
            GUI.enabled = false;
            EditorGUILayout.ObjectField(prof, typeof(DataBuildSettingProfile), true);
            GUI.enabled = true;

            GUILayout.Space(20f);
            GUILayout.Label("version", BuildorHelperGuiStyle.getCategoryBold());

            BuildSettingVersionType vType = BuildorWinEdHelper.drawEnum<BuildSettingVersionType>("publish type", "publish", 0);
            bool _publish = vType == BuildSettingVersionType.vPublish;

            GUILayout.BeginHorizontal();

            GUI.enabled = false;
            var version = vType == BuildSettingVersionType.vPublish ? prof.publishVersion : prof.internalVersion;
            EditorGUILayout.ObjectField(version, typeof(DataBuildSettingVersion), true);
            GUI.enabled = true;

            if(version != null)
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

            GUILayout.Space(20f);
            GUILayout.Label("misc", BuildorHelperGuiStyle.getCategoryBold());

            WinEdFieldsHelper.drawCopyPastablePath("output folder :", prof.getBasePath());
            WinEdFieldsHelper.drawCopyPastablePath("build name :", prof.getBuildFullName(true));
            WinEdFieldsHelper.drawCopyPastablePath("zip name suggestion :", prof.getZipName());
            GUILayout.Space(10f);
            WinEdFieldsHelper.drawCopyPastablePath("full path :", prof.getFullPath());

            if(GUILayout.Button("open output folder"))
            {
                os_openFolder(prof.getExportFolderPath());
            }

            GUILayout.Space(20f);
            GUILayout.Label("toggles for build", BuildorHelperGuiStyle.getCategoryBold());

            bool autorun = WinEdFieldsHelper.drawToggle("autorun", "autorun");
            bool incVersion = WinEdFieldsHelper.drawToggle("incVersion", "incVersion");
            bool openAfterBuild = WinEdFieldsHelper.drawToggle("open folder after build", "openAfterBuild");

            prof.developement_build = GUILayout.Toggle(prof.developement_build, "dev build");
            if(prof.developement_build != EditorUserBuildSettings.development)
            {
                EditorUserBuildSettings.development = prof.developement_build;
            }

            GUILayout.Space(20f);
            if (GUILayout.Button("BUILD", BuildorHelperGuiStyle.getButtonBig(50f)))
            {
                new BuildHelperBase(_publish, autorun, incVersion, openAfterBuild);
            }

            GUILayout.EndScrollView();
        }



        /// <summary>
        /// open explorer at path
        /// </summary>
        /// <param name="folderPath"></param>
        static public void os_openFolder(string folderPath, bool selectFolder = false)
        {
            folderPath = folderPath.Replace(@"/", @"\");   // explorer doesn't like front slashes

            string argument = string.Empty;

            if (selectFolder)
            {
                //https://stackoverflow.com/questions/334630/opening-a-folder-in-explorer-and-selecting-a-file
                argument = "/select, ";
            }

            argument += "\"" + folderPath + "\"";

            UnityEngine.Debug.Log("cmd:opening : " + argument);

            //https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.start?view=netframework-4.7.2#System_Diagnostics_Process_Start_System_String_System_String_
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

    }

}

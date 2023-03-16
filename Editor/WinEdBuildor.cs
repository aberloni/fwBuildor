using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using fwp.buildor.version;

namespace fwp.buildor.editor
{
    public class WinEdBuildor : EditorWindow
    {

        [MenuItem("Window/Buildor/(window) open buildor", false, 0)]
        static void init()
        {
            EditorWindow.GetWindow(typeof(WinEdBuildor));
        }

        Vector2 scroll;

        BuildHelperFlags flagsBuild;
        BuildPathFlags flagsPath;

        private void Update()
        { }

        void OnGUI()
        {
            GUILayout.Label("Buildor", BuildorHelperGuiStyle.getWinTitle());
            
            DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();
            if (prof == null)
            {
                GUILayout.Label("this view needs some active profil setup");
                return;
            }

            scroll = GUILayout.BeginScrollView(scroll);
            
            GUILayout.Label("platform", BuildorHelperGuiStyle.getCategoryBold());
            GUI.enabled = false;
            EditorGUILayout.ObjectField(prof, typeof(DataBuildSettingProfile), true);
            GUI.enabled = true;

            GUILayout.Space(20f);
            GUILayout.Label("version", BuildorHelperGuiStyle.getCategoryBold());

            BuildSettingVersionType vType = BuildorWinEdHelper.drawEnum<BuildSettingVersionType>("publish type", "publish", 0);
            flagsBuild.isPublishingBuild = vType == BuildSettingVersionType.vPublish;

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

            GUILayout.Space(10f);

            GUILayout.Label("build flags", BuildorHelperGuiStyle.getCategoryBold());

            flagsBuild.autorun = WinEdFieldsHelper.drawToggle("autorun", "autorun");
            flagsBuild.incVersion = WinEdFieldsHelper.drawToggle("incVersion", "incVersion");
            flagsBuild.openFolderOnSucess = WinEdFieldsHelper.drawToggle("open folder after build", "openAfterBuild");

            prof.developement_build = GUILayout.Toggle(prof.developement_build, "dev build");
            if (prof.developement_build != EditorUserBuildSettings.development)
            {
                EditorUserBuildSettings.development = prof.developement_build;
                Debug.LogWarning("changed dev build : " + EditorUserBuildSettings.development);

                UnityEditor.EditorUtility.SetDirty(prof);
            }

            GUILayout.Label("path modifiers", BuildorHelperGuiStyle.getCategoryBold());

            GUILayout.BeginHorizontal();
            flagsPath.pathIncludePrefix = WinEdFieldsHelper.drawToggle("prefix", "pathIncludePrefix");
            flagsPath.pathIncludePlatform = WinEdFieldsHelper.drawToggle("platform", "pathIncludePlatform");
            flagsPath.pathIncludeDate = WinEdFieldsHelper.drawToggle("date", "pathIncludeDate");
            flagsPath.pathIncludeVersion = WinEdFieldsHelper.drawToggle("version", "pathIncludeVersion");
            GUILayout.EndHorizontal();

            GUILayout.Space(20f);
            GUILayout.Label("outputs", BuildorHelperGuiStyle.getCategoryBold());

            string outputFolder = prof.getAbsoluteBuildFolderPath(flagsPath);
            
            WinEdFieldsHelper.drawCopyPastablePath("base path : ", outputFolder);
            WinEdFieldsHelper.drawCopyPastablePath("app name :", prof.getAppName());
            WinEdFieldsHelper.drawCopyPastablePath("zip name :", prof.getZipName());
            
            string fullPath = Path.Combine(outputFolder, prof.getAppName());
            WinEdFieldsHelper.drawCopyPastablePath("full path :", fullPath);

            if(GUILayout.Button("open output folder"))
            {
                os_openFolder(outputFolder);
            }

            GUILayout.Space(20f);
            if (GUILayout.Button("BUILD", BuildorHelperGuiStyle.getButtonBig(50f)))
            {
                new BuildHelperBase(flagsBuild, flagsPath);
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

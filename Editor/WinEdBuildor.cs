using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using fwp.buildor.version;
using fwp.halpers;


namespace fwp.buildor
{

    public class WinEdBuildor : EditorWindow
    {

        [MenuItem("Build/open buildor", false, 0)]
        static void init()
        {
            EditorWindow.GetWindow(typeof(WinEdBuildor));
        }

        Vector2 scroll;

        void OnGUI()
        {
            GUILayout.Label("Buildor", HalperGuiStyle.getWinTitle());

            scroll = GUILayout.BeginScrollView(scroll);
            DataBuildSettingProfile prof = BuildHelperBase.getActiveProfile();

            GUILayout.Label("platform", HalperGuiStyle.getCategoryBold());
            GUI.enabled = false;
            EditorGUILayout.ObjectField(prof, typeof(DataBuildSettingProfile), true);
            GUI.enabled = true;

            GUILayout.Space(20f);
            GUILayout.Label("version", HalperGuiStyle.getCategoryBold());

            BuildSettingVersionType vType = WinEdFieldsHelper.drawEnum<BuildSettingVersionType>("publish type", "publish", 0);
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
            GUILayout.Label("misc", HalperGuiStyle.getCategoryBold());

            WinEdFieldsHelper.drawCopyPastablePath("output folder :", prof.getBasePath());
            WinEdFieldsHelper.drawCopyPastablePath("build name :", prof.getBuildFullName(true));
            WinEdFieldsHelper.drawCopyPastablePath("zip name suggestion :", prof.getZipName());
            GUILayout.Space(10f);
            WinEdFieldsHelper.drawCopyPastablePath("full path :", prof.getFullPath());

            if(GUILayout.Button("open output folder"))
            {
                HalperNatives.os_openFolder(prof.getExportFolderPath());
            }

            GUILayout.Space(20f);
            GUILayout.Label("toggles for build", HalperGuiStyle.getCategoryBold());

            bool autorun = WinEdFieldsHelper.drawToggle("autorun", "autorun");
            bool incVersion = WinEdFieldsHelper.drawToggle("incVersion", "incVersion");
            bool openAfterBuild = WinEdFieldsHelper.drawToggle("open folder after build", "openAfterBuild");

            prof.developement_build = GUILayout.Toggle(prof.developement_build, "dev build");
            if(prof.developement_build != EditorUserBuildSettings.development)
            {
                EditorUserBuildSettings.development = prof.developement_build;
            }

            GUILayout.Space(20f);
            if (GUILayout.Button("BUILD", HalperGuiStyle.getButtonBig(50f)))
            {
                new BuildHelperBase(_publish, autorun, incVersion, openAfterBuild);
            }

            GUILayout.EndScrollView();
        }

    }

}

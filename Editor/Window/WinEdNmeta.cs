using UnityEngine;
using UnityEditor;
using fwp.version;

namespace fwp.buildor.editor
{
    /// <summary>
    /// inject switch version into .nmeta file
    /// same content as DataVersionSwitch inspector (SwitchNmeta)
    /// </summary>
    public class WinEdNmeta : EditorWindow
    {
        [MenuItem(BuildorVerbosity._buildor_menuitem_path + "nmeta version injector (win)", false, 20)]
        static void init()
        {
            GetWindow<WinEdNmeta>("Nmeta");
        }

        DataVersionSwitch version;

        void OnEnable()
        {
            // default : switch version of active profil
            if (version == null) version = BuildorVars.Profile?.Version as DataVersionSwitch;
        }

        void OnGUI()
        {
            version = (DataVersionSwitch)EditorGUILayout.ObjectField("switch version", version, typeof(DataVersionSwitch), false);

            if (version != null)
            {
                EditorGUILayout.LabelField("version", version.HasData ? version.getFormated() : "(no DataVersion)");
                EditorGUILayout.LabelField("release", version.VersionRelease.ToString());
            }

            EditorGUILayout.Space();

            SwitchNmeta.drawGUI(version);
        }
    }
}

using fwp.buildor;
using UnityEngine;

abstract public class ProfilParameter
{
    abstract public string GetUid();

    public BuildModule[] modules = new BuildModule[0];

    [Tooltip("meant for override, folder where build is located each time, build name is then static")]
    public string output_folder_specific;
    public bool HasSpecificFolder => !string.IsNullOrEmpty(output_folder_specific);

    readonly GUIContent gui_btn_browse = new GUIContent("browse");

    public void drawFolderSelector()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(GetUid() + " specific/", HelperGui.gBold);

        if (HasSpecificFolder)
        {
            GUILayout.Label(output_folder_specific);

            if (output_folder_specific.Length > 0 && GUILayout.Button("clear", GUILayout.Width(100f)))
            {
                output_folder_specific = string.Empty;
            }
        }
        
        if (GUILayout.Button(gui_btn_browse, GUILayout.Width(100f)))
        {
            string _path = UnityEditor.EditorUtility.OpenFolderPanel(
                "Select export folder", output_folder_specific, string.Empty);

            if (output_folder_specific != _path) output_folder_specific = _path;
        }

        GUILayout.EndHorizontal();
    }

    virtual public void applyProfil()
    { }

    public void ApplyModules()
    {
        if (modules == null) return;

        foreach (var m in modules)
        {
            m?.Apply();
        }
    }
}

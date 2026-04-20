using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// will copy content from specified folder
/// to StreamingAssets/Medias/
/// </summary>
[System.Serializable]
public class BuildExtensionMedias
{

    readonly GUIContent gui_btn_browse = new GUIContent("browse");
    [Tooltip("folder with all data to copy"), SerializeField]
    string originInput;

    [Tooltip("folder within StreaminAssets/"), SerializeField]
    string streamingOutput = "medias";

    string DestiPath => Path.Combine(Application.streamingAssetsPath, streamingOutput);

    public void draw()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(gui_btn_browse, GUILayout.Width(100f)))
        {
            originInput = EditorUtility.OpenFolderPanel(
                "Select export folder", originInput, string.Empty);
        }
        else if (!string.IsNullOrEmpty(originInput))
        {
            GUILayout.Label(originInput);

            if (originInput.Length > 0 && GUILayout.Button("clear", GUILayout.Width(100f)))
            {
                originInput = string.Empty;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Label(DestiPath);
    }

}

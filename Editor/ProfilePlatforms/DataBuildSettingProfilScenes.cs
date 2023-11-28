using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// 
/// https://docs.unity3d.com/ScriptReference/EditorBuildSettingsScene.html
/// 
/// </summary>

[CreateAssetMenu(menuName = "buildor/merger/create DataBuildSettingProfilScenes", order = 100)]
public class DataBuildSettingProfilScenes : ScriptableObject
{
    [Header("params")]
    
    [Tooltip("all string pattern that will be recorded")]
    public string[] includesFilter; // filter a string that the scene NEEDs to have, CONTAINS
    public bool includesAND;

    [Tooltip("all string pattern that will be ignored")]
    public string[] excludesFilter; // filter things that are excluded, CONTAINS
    public bool excludesAND;

    [Header("result")]
    public string[] paths; // can't use Scene type :/ (not serializable)

#if UNITY_EDITOR

    [ContextMenu("add")]
    public void add()
    {
        //keep existing
        List<EditorBuildSettingsScene> tmp = new List<EditorBuildSettingsScene>();
        tmp.AddRange(EditorBuildSettings.scenes);

        for (int i = 0; i < paths.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < tmp.Count; j++)
            {
                if (tmp[j].path == paths[i])
                {
                    found = true;
                }
            }

            if (!found)
            {
                tmp.Add(new EditorBuildSettingsScene(paths[i], true));
            }
        }

        EditorBuildSettings.scenes = tmp.ToArray();
    }

    [ContextMenu("record")]
    public void record()
    {
        List<string> tmp = new List<string>();

        EditorBuildSettingsScene[] edScenes = EditorBuildSettings.scenes;
        foreach (EditorBuildSettingsScene sc in edScenes)
        {
            bool toAdd = includesAND;

            // must include ALL
            if(includesFilter != null)
            {
                foreach(string filter in includesFilter)
                {
                    if (filter.Length <= 0) continue;
                    
                    if(includesAND && !sc.path.Contains(filter)) toAdd = false;
                    else if(!includesAND && sc.path.Contains(filter)) toAdd = true;
                }
            }

            bool toExclude = excludesAND;

            if(excludesFilter != null)
            {
                foreach (string exclude in excludesFilter)
                {
                    if (exclude.Length <= 0) continue;

                    if (excludesAND && !sc.path.Contains(exclude)) toExclude = false;
                    else if (!excludesAND && sc.path.Contains(exclude)) toExclude = true;
                }
            }

            if (toAdd && !toExclude)
            {
                tmp.Add(sc.path);
            }
        }

        paths = tmp.ToArray();

        EditorUtility.SetDirty(this);
    }

#endif

}
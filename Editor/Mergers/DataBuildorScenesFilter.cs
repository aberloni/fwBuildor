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
/// meant to filter a list of path
/// this will be feed to the ScenesMerger
/// </summary>

[CreateAssetMenu(menuName = "buildor/merger/new filter", order = 100)]
public class DataBuildorScenesFilter : ScriptableObject
{
    [Header("params")]

    public bool recordFromBuildSettings;

    [Tooltip("all string pattern that will be recorded")]
    public string[] includesFilter; // filter a string that the scene NEEDs to have, CONTAINS
    public bool includesAND;

    [Tooltip("all string pattern that will be ignored")]
    public string[] excludesFilter; // filter things that are excluded, CONTAINS
    public bool excludesAND;

    [Header("result")]
    public string[] paths; // can't use Scene type :/ (not serializable)

    public string stringify()
    {
        string ret = name;
        foreach(var p in paths)
        {
            ret += "\n  " + p;
        }
        return ret;
    }

#if UNITY_EDITOR

    [ContextMenu("add to build settings")]
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

    [ContextMenu("record from project scenes")]
    public void record()
    {
        List<string> tmp = new List<string>();

        List<string> scenesPaths = new List<string>();
        if(recordFromBuildSettings)
        {
            var edScenes = EditorBuildSettings.scenes;
            foreach (var sc in edScenes)
            {
                scenesPaths.Add(sc.path);
            }
        }
        else
        {
            scenesPaths.AddRange(getProjectAssetScenesPaths());
        }

        foreach (string path in scenesPaths)
        {
            bool toAdd = includesAND;

            // must include ALL
            if(includesFilter != null)
            {
                foreach(string filter in includesFilter)
                {
                    if (filter.Length <= 0) continue;
                    
                    if(includesAND && !path.Contains(filter)) toAdd = false;
                    else if(!includesAND && path.Contains(filter)) toAdd = true;
                }
            }

            bool toExclude = excludesAND;

            if(excludesFilter != null)
            {
                foreach (string exclude in excludesFilter)
                {
                    if (exclude.Length <= 0) continue;

                    if (excludesAND && !path.Contains(exclude)) toExclude = false;
                    else if (!excludesAND && path.Contains(exclude)) toExclude = true;
                }
            }

            if (toAdd && !toExclude)
            {
                tmp.Add(path);
            }
        }

        paths = tmp.ToArray();

        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// fetch all scene present in database
    /// this should return all scene in projet
    /// </summary>
    static public string[] getProjectAssetScenesPaths()
    {
        string[] paths = AssetDatabase.FindAssets("t:Scene");

        if (paths.Length <= 0)
        {
            Debug.LogWarning("asking for scene but none ?");
        }

        //replace GUID by full path
        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
        }

        return paths;
    }
#endif

}

#if UNITY_EDITOR
[CustomEditor(typeof(DataBuildorScenesFilter))]
public class DataBuildorScenesFilterCustom : Editor
{
    public override void OnInspectorGUI()
    {
        var t = target as DataBuildorScenesFilter;
        
        if(GUILayout.Button("add to build settings"))
        {
            t.add();
        }

        if (GUILayout.Button("record from project"))
        {
            t.record();
        }

        GUILayout.Space(10f);

        base.OnInspectorGUI();
    }
}
#endif
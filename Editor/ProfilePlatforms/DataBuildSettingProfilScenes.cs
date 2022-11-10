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
  public string filter = ""; // filter a string that the scene NEEDs to have
  public string[] excludes; // filter things that are excluded

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
        if(tmp[j].path == paths[i])
        {
          found = true;
        }
      }

      if(!found)
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
    foreach(EditorBuildSettingsScene sc in edScenes)
    {
      bool add = true;

      if(filter.Length > 0)
      {
        if(!sc.path.Contains(filter))
        {
          add = false;
        }
      }

      if(add)
      {
        foreach (string exclude in excludes)
        {
          if (sc.path.Contains(exclude)) add = false;
        }
      }
      
      if(add)
      {
        Debug.Log(sc.path);
        //tmp.Add(EditorSceneManager.GetSceneByPath(sc.path));
        tmp.Add(sc.path);
      }
    }

    paths = tmp.ToArray();

    EditorUtility.SetDirty(this);
  }

#endif
  
}
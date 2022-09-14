using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace buildor
{
  /*
  [CustomEditor(typeof(DataBuildSettingsBridge))]
  public class DataBuildSettingsBridgeEditor : Editor
  {
    override public void OnInspectorGUI()
    {
      DrawDefaultInspector();

      EditorGUILayout.Separator();

      DataBuildSettingsBridge handle = (DataBuildSettingsBridge)target;

      if (GUILayout.Button("apply all settings"))
      {
        Debug.LogWarning("not implem");
        //handle.activeScenes.apply();
        //BuildHelperBase.applySettings();
      }

      EditorGUILayout.Separator();

      if(handle.availableScenesListing != null)
      {
        for (int i = 0; i < handle.availableScenesListing.Length; i++)
        {
          if (GUILayout.Button("apply " + handle.availableScenesListing[i].name))
          {
            handle.availableScenesListing[i].add();
          }
        }
      }
      
    }

  }
  
  */

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.buildor.editor
{
    static public class BuildorHelpers
    {
        public const string _menuItem_basepath = "buildor/";
        public const string _path_merger = _menuItem_basepath + "merger/";

        static public ScriptableObject[] getScriptableObjectsInEditor(System.Type scriptableType)
        {
            string[] all = AssetDatabase.FindAssets("t:" + scriptableType.Name);

            List<ScriptableObject> output = new List<ScriptableObject>();
            for (int i = 0; i < all.Length; i++)
            {
                Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), scriptableType);
                ScriptableObject so = obj as ScriptableObject;
                if (so == null) continue;
                output.Add(so);
            }

            //Debug.Log(scriptableType + " x"+output.Count+" / x" + all.Length);

            return output.ToArray();
        }

        static public T getScriptableObjectInEditor<T>(string nameContains = "", bool pingFailure = false) where T : ScriptableObject
        {
            string[] all = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            for (int i = 0; i < all.Length; i++)
            {
                Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(T));
                T data = obj as T;

                if (data == null) continue;
                if (nameContains.Length > 0)
                {
                    if (!data.name.Contains(nameContains)) continue;
                }

                return data;
            }

            if(pingFailure)
            {
                Debug.LogWarning("can't locate scriptable of type " + typeof(T).Name + " (filter name ? " + nameContains + ")");
            }

            return null;
        }

    }

}

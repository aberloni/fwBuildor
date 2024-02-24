using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.buildor.editor
{
    static public class BuildorHelpers
    {

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


        static public ScriptableObject[] getScriptableObjectsInEditor<T>() where T : ScriptableObject
            => getScriptableObjectsInEditor(typeof(T));

        static public T getScriptableObjectInEditor<T>(string nameContains = "") where T : ScriptableObject
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
            Debug.LogWarning("can't locate scriptable of type " + typeof(T).Name + " (filter name ? " + nameContains + ")");
            return null;
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class BuildorWinEdHelper
{
#if UNITY_EDITOR
    static public T drawEnum<T>(string label, string pprefUid, int defaultValue = 0) where T : System.Enum
    {
        int value = UnityEditor.EditorPrefs.GetInt(pprefUid, defaultValue);
        T enumValue = (T)System.Enum.ToObject(typeof(T), value);

        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(200f));
        T enumOutput = (T)UnityEditor.EditorGUILayout.EnumPopup(enumValue);
        GUILayout.EndHorizontal();

        if (enumOutput.CompareTo(enumValue) != 0)
        {
            UnityEditor.EditorPrefs.SetInt(pprefUid, (int)System.Enum.Parse(typeof(T), enumOutput.ToString()));
        }

        return enumOutput;
    }
#endif
}

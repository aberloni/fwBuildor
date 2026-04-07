using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class BuildorWinEdHelper
{
#if UNITY_EDITOR

    readonly static GUILayoutOption labelW = GUILayout.Width(200f);

    /// <summary>
    /// without ppref
    /// </summary>
    static public T drawEnum<T>(string label, int value = 0) where T : System.Enum
    {
        T enumValue = (T)System.Enum.ToObject(typeof(T), value);

        GUILayout.BeginHorizontal();
        GUILayout.Label(label, labelW);
        T enumOutput = (T)UnityEditor.EditorGUILayout.EnumPopup(enumValue);
        GUILayout.EndHorizontal();

        return enumOutput;
    }

    static public T drawEnum<T>(string label, string pprefUid, int defaultValue = 0) where T : System.Enum
    {
        int value = PlayerPrefs.GetInt(pprefUid, defaultValue);
        T enumValue = (T)System.Enum.ToObject(typeof(T), value);

        GUILayout.BeginHorizontal();
        GUILayout.Label(label, labelW);
        T enumOutput = (T)UnityEditor.EditorGUILayout.EnumPopup(enumValue);
        GUILayout.EndHorizontal();

        if (enumOutput.CompareTo(enumValue) != 0)
        {
            PlayerPrefs.SetInt(pprefUid, (int)System.Enum.Parse(typeof(T), enumOutput.ToString()));
        }

        return enumOutput;
    }

#endif
}

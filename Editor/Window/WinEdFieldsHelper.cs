using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.buildor.editor
{
	static public class WinEdFieldsHelper
	{
		static public void drawCopyPastablePath(string label, string path)
		{
			GUILayout.BeginHorizontal();
			drawDisabledText(label, path); // read only fields
			if (GUILayout.Button("copy", GUILayout.Width(70f))) EditorGUIUtility.systemCopyBuffer = path;
			GUILayout.EndHorizontal();
		}

		static public void drawDisabledText(string label, string value)
		{
			GUILayout.BeginHorizontal();
			GUI.enabled = false;
			if (!string.IsNullOrEmpty(label)) GUILayout.Label(label, GUILayout.Width(150f));
			GUILayout.TextField(value);
			GUI.enabled = true;
			GUILayout.EndHorizontal();
		}

		/// <summary>
		/// draw text area
		/// </summary>
		static public string editText(string label, string ppref)
		{
			// if(string.IsNullOrEmpty(fieldLabel)) fieldLabel = ppref;
			GUILayout.BeginHorizontal();
			if (!string.IsNullOrEmpty(label)) GUILayout.Label(label, GUILayout.Width(150f));
			string text = EditorPrefs.GetString(ppref, string.Empty);
			string _text = GUILayout.TextField(text);
			if (text != _text) EditorPrefs.SetString(ppref, _text);
			GUILayout.EndHorizontal();
			return _text;
		}

		/// <summary>
		/// will auto ppref save value using label is not specific key is given
		/// </summary>
		static public bool drawToggle(bool value, string label = null, string ppref = null)
		{
			if (ppref == null && !string.IsNullOrEmpty(label)) ppref = label;
			bool _val = GUILayout.Toggle(value, label, GUILayout.Width(100f));
			if (!string.IsNullOrEmpty(ppref) && _val != value)
			{
				EditorPrefs.SetInt(ppref, _val ? 1 : 0);
				// Debug.Log(ppref+"="+_val);
			}

			return _val;
		}
		static public bool drawToggle(string label, string ppref, bool? forceValue = null)
		{
			if (forceValue == null) forceValue = EditorPrefs.GetInt(ppref, -1) > 0;
			return drawToggle(forceValue.Value, label, ppref);
		}

		static public int drawInt(string label, string ppref)
		{
			int _val = EditorPrefs.GetInt(ppref);

			int val = EditorGUILayout.IntField(label, _val);
			if (val != _val)
			{
				EditorPrefs.SetInt(ppref, val);
			}

			return val;
		}

		static public T drawEnum<T>(string label, string pprefUid, int defaultValue = 0) where T : System.Enum
		{
			int value = EditorPrefs.GetInt(pprefUid, defaultValue);
			T enumValue = (T)System.Enum.ToObject(typeof(T), value);

			GUILayout.BeginHorizontal();
			GUILayout.Label(label, GUILayout.Width(200f));
			T enumOutput = (T)EditorGUILayout.EnumPopup(enumValue);
			GUILayout.EndHorizontal();

			if (enumOutput.CompareTo(enumValue) != 0)
			{
				EditorPrefs.SetInt(pprefUid, (int)System.Enum.Parse(typeof(T), enumOutput.ToString()));
			}

			return enumOutput;
		}


	}

}

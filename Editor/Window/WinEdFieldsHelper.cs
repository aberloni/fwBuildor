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
			WinEdFieldsHelper.drawTextLabel(label, path, true);
			if (GUILayout.Button("copy", GUILayout.Width(70f))) EditorGUIUtility.systemCopyBuffer = path;
			GUILayout.EndHorizontal();
		}

		static public void drawTextLabel(string label, string text, bool disabled = false)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(label, GUILayout.Width(150f));
			if (disabled) GUI.enabled = false;
			GUILayout.TextArea(text);
			if (disabled) GUI.enabled = true;
			GUILayout.EndHorizontal();
		}

		static public bool drawToggle(string label, string ppref, bool? forceValue = null)
		{
			bool _val = EditorPrefs.GetInt(ppref, -1) > 0;
			bool val = _val;

			if (forceValue == null)
			{
				val = GUILayout.Toggle(_val, label);
			}
			else
			{
				val = forceValue.Value;
			}

			if (val != _val) EditorPrefs.SetInt(ppref, val ? 1 : 0);

			return val;
		}

		static public int drawInt(string label, string ppref)
		{
			int _val = EditorPrefs.GetInt(ppref);

			int val = EditorGUILayout.IntField(label, _val);
			if (val != _val) EditorPrefs.SetInt(ppref, val);

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

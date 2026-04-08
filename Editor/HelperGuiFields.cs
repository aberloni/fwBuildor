using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.buildor.editor
{
	static public class HelperGuiFields
	{

		static public void drawField(string context, string value)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(context, HelperGui.bM);
			GUILayout.Label(value, HelperGui.gWrapped, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
		}

		static public void drawWrapLabel(string label)
		{
			if (string.IsNullOrEmpty(label)) return;
			GUILayout.Label(label, HelperGui.gWrapped, GUILayout.ExpandWidth(true));
		}

		static public void drawCopyPastablePath(string label, string path, bool useCopy = true)
		{
			GUILayout.BeginHorizontal();
			if (useCopy && GUILayout.Button("copy", GUILayout.Width(70f))) EditorGUIUtility.systemCopyBuffer = path;
			drawDisabledText(label, path); // read only fields
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
		static public string editText(string ppref, string label = null)
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
		static public bool drawPrefToggle(string ppref, string label = null)
		{
			bool value = EditorPrefs.GetBool(ppref);
			bool _val = GUILayout.Toggle(value, label, GUILayout.Width(150f));
			if (_val != value) EditorPrefs.SetBool(ppref, _val);
			return _val;
		}

		static public bool drawToggle(bool value, string label, System.Action<bool> onDirty = null)
		{
			bool _val = GUILayout.Toggle(value, label, GUILayout.Width(150f));
			if (_val != value) onDirty?.Invoke(_val);
			return _val;
		}

		static public int drawPrefInt(string label, string ppref)
		{
			int value = EditorPrefs.GetInt(ppref);

			int _val = EditorGUILayout.IntField(label, value);
			if (_val != value)
			{
				EditorPrefs.SetInt(ppref, _val);
				// Debug.LogWarning(ppref + "=" + _val);
			}

			return _val;
		}

		static public T drawObject<T>(string label, T target, bool disabled = false) where T : Object
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(label, GUILayout.Width(200f));
			if (disabled) GUI.enabled = false;
			var ret = (T)EditorGUILayout.ObjectField(target, typeof(T), false);
			if (ret != null && GUILayout.Button(">>", GUILayout.Width(40f)))
			{
				UnityEditor.Selection.activeObject = ret;
			}
			if (disabled) GUI.enabled = true;
			GUILayout.EndHorizontal();
			return ret;
		}

		static public void drawObjectDisabled(Object target)
		{
			GUI.enabled = false;
			EditorGUILayout.ObjectField(target, target.GetType(), false);
			GUI.enabled = true;
		}

		/// <summary>
		/// without ppref
		/// </summary>
		static public T drawEnum<T>(string label, T value, System.Action<T> onDirty = null) where T : System.Enum
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(label + "(" + value + ")");
			T enumOutput = (T)EditorGUILayout.EnumPopup(value);
			if (enumOutput.CompareTo(value) != 0) onDirty?.Invoke(enumOutput);
			GUILayout.EndHorizontal();
			return enumOutput;
		}

		static public T drawPrefEnum<T>(string pprefUid, string label = null, System.Action<T> onDirty = null) where T : System.Enum
		{
			T enumValue = (T)System.Enum.ToObject(typeof(T), EditorPrefs.GetInt(pprefUid, 0));

			GUILayout.BeginHorizontal();
			if (!string.IsNullOrEmpty(label)) GUILayout.Label(label);
			T enumOutput = (T)EditorGUILayout.EnumPopup(enumValue);
			GUILayout.EndHorizontal();

			if (enumOutput.CompareTo(enumValue) != 0)
			{
				EditorPrefs.SetInt(pprefUid, (int)System.Enum.Parse(typeof(T), enumOutput.ToString()));
				onDirty?.Invoke(enumOutput);
			}

			return enumOutput;
		}


	}

}

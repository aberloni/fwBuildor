using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.buildor
{
	using fwp.buildor.editor;

	static public class BuildorHelpers
	{
		static public DataBuildSettingProfile Profile
		{
			get
			{
				return BuildHelperBase.getActiveProfile(PublishLevel);
			}
		}

		public const string ppref_publish = "level_publish";
		static public PublishLevel PublishLevel
		{
			get => (PublishLevel)PlayerPrefs.GetInt(ppref_publish, (int)PublishLevel.normal);
			set => PlayerPrefs.SetInt(ppref_publish, (int)value);
		}

		public const string ppref_debug = "ppref_debug";
		static public DebugLevel DebugLevel
		{
			get => (DebugLevel)PlayerPrefs.GetInt(ppref_debug, (int)DebugLevel.normal);
			set => PlayerPrefs.SetInt(ppref_debug, (int)value);
		}

		static public bool IsDebug => DebugLevel == DebugLevel.debug;

		public const string _menuItem_basepath = "buildor/";
		public const string _path_merger = _menuItem_basepath + "merger/";

        static public string formatSymbols(string[] symbols)
        {
            string ret = string.Empty;
            foreach(var s in symbols) ret += s+";";
            return ret;
        }
		
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

		static public T getScriptableObjectInEditor<T>(string filter = "", bool pingFailure = false) where T : ScriptableObject
		{
			string[] all = AssetDatabase.FindAssets("t:" + typeof(T).Name);
			for (int i = 0; i < all.Length; i++)
			{
				Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(T));
				T data = obj as T;

				if (data == null) continue;

				//Debug.Log(data.name + " vs " + filter, data);

				if (!string.IsNullOrEmpty(filter))
				{
					if (!data.name.Contains(filter)) continue;
				}

				return data;
			}

			if (pingFailure)
			{
				Debug.LogWarning("can't locate scriptable of type " + typeof(T).Name + " (filter name ? " + filter + ") x" + all.Length);
			}

			return null;
		}

	}

}

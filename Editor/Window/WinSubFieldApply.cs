using UnityEngine;
using UnityEditor;

namespace fwp.buildor.editor
{

	/// <summary>
	/// wrapper around a scriptable object
	/// fetch, swap
	/// the idea is to be able to override profil value
	/// 
	/// store target scriptable object NAME into editor pprefs
	/// </summary>
	abstract public class WinSubFieldApply<T> where T : ScriptableObject
	{
		protected WinEdBuildor win;

		bool _fold;

		public WinSubFieldApply(WinEdBuildor win)
		{
			this.win = win;

			focus();
		}

		/// <summary>
		/// value injected in buildor window, to override profil
		/// </summary>
		protected T _valuePrefs;

		/// <summary>
		/// value assigned in active profil
		/// </summary>
		protected T _valueProfil;

		/// <summary>
		/// whichever is != null
		/// </summary>
		public T Value
		{
			get
			{
				if (_valuePrefs != null) return _valuePrefs;
				if (_valueProfil != null) return _valueProfil;
				return null;
			}
		}

		string pprefUid()
		{
			string ppref = typeof(T).ToString();
			var profil = BuildorHelpers.Profile;
			if (profil != null) ppref = profil.name + "_" + ppref;
			return ppref;
		}

		public void focus()
		{
			_valuePrefs = fetchPrefsInstance();
			_valueProfil = fetchProfilInstance();
		}

		protected T fetchPrefsInstance()
		{
			string val = EditorPrefs.GetString(pprefUid(), string.Empty);

			T ret = null;

			// if none yet, try to get ppref one
			if (!string.IsNullOrEmpty(val))
			{
				ret = BuildorHelpers.getScriptableObjectInEditor<T>(val, true);
				//Debug.LogWarning("could not locate : " + val + "<" + typeof(T) + ">");
			}

			// nothing for that value ? reset
			if (ret == null)
			{
				EditorPrefs.SetString(pprefUid(), string.Empty);
			}
			return ret;
		}

		abstract protected T fetchProfilInstance();

		virtual protected string getSectionTitle() => GetType().ToString();

		public void draw()
		{
			GUILayout.Label(getSectionTitle(), HelperGui.gCategoryBold);

			T _val;

			_val = drawInstance("override", _valuePrefs, true);
			if (_val != _valuePrefs)
			{
				onValueChanged(_val);
			}

			if (_valuePrefs == null)
			{
				drawInstance("profil", _valueProfil, false);
			}


			if (Value != null) drawHeader(Value);

			if (Value != null)
			{
				_fold = EditorGUILayout.Foldout(_fold, "see details of injection", true);
				if (_fold) drawDetails(Value);
			}


		}

		T drawInstance(string label, T val, bool drawSelector)
		{
			GUILayout.BeginHorizontal();

			if (val != null && GUILayout.Button("?", GUILayout.Width(20f)))
			{
				UnityEditor.Selection.activeObject = val;
			}

			if (val != null && GUILayout.Button("apply", GUILayout.Width(50f)))
			{
				applyEditor(val);
			}

			GUILayout.Label(label, GUILayout.Width(100f));


			if (drawSelector)
			{
				val = (T)EditorGUILayout.ObjectField(val, typeof(T), true);
				if (val != null && GUILayout.Button("clear", GUILayout.Width(50f))) val = null;
			}
			else if (val != null)
			{
				GUILayout.Label(val.ToString());
			}
			else GUILayout.Label("-none-");


			GUILayout.EndHorizontal();

			/*
			if (val != null)
			{
				_fold = EditorGUILayout.Foldout(_fold, "see details", true);
				if (_fold) drawDetails(val);
			}
			*/


			return val;
		}

		/// <summary>
		/// when object field changes value
		/// how to apply to profil
		/// </summary>
		virtual protected void onValueChanged(T value)
		{
			string uid = value != null ? value.name : string.Empty;
			EditorPrefs.SetString(pprefUid(), uid);
			_valuePrefs = fetchPrefsInstance();
		}

		/// <summary>
		/// major info of currently selected item
		/// </summary>
		virtual protected void drawHeader(T value)
		{
			GUILayout.Label("focus: " + value.name);
		}

		/// <summary>
		/// detail dropdown section
		/// if user tick box
		/// </summary>
		abstract protected void drawDetails(T value);

		/// <summary>
		/// pressed apply button
		/// how selected element apply its values to editor
		/// </summary>
		abstract protected void applyEditor(T value);
	}
}
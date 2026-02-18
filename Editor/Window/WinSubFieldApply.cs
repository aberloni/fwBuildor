using UnityEngine;

namespace fwp.buildor.editor
{
	using UnityEditor;

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
		

		public T Value
		{
			get
			{
				if (_valuePrefs != null) return _valuePrefs;
				if (_valueProfil != null) return _valueProfil;
				return null;
			}
		}

		string pprefUid() => win.activeProfil.name + "_" + typeof(T).ToString();

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
			GUILayout.Label(getSectionTitle(), BuildorHelperGuiStyle.getCategoryBold());

			GUILayout.BeginHorizontal();
			var newValue = (T)EditorGUILayout.ObjectField(_valuePrefs, typeof(T), true);
			if (_valuePrefs != newValue)
			{
				onValueChanged(newValue);
			}

			if (_valuePrefs != null)
			{
				if (Value != null && GUILayout.Button("apply", GUILayout.Width(50f)))
				{
					applyEditor(Value);
				}

				if (GUILayout.Button("clear", GUILayout.Width(50f)))
				{
					onValueChanged(null);
				}

				if (GUILayout.Button("?", GUILayout.Width(20f)))
					UnityEditor.Selection.activeObject = _valuePrefs;
			}
			GUILayout.EndHorizontal();

			if (Value != null) drawHeader(Value);

			GUILayout.BeginHorizontal();
			if (_valueProfil == null) GUILayout.Label("profil has :		none");
			else
			{
				GUILayout.Label("profil has :		" + _valueProfil.name);
				if (GUILayout.Button("?", GUILayout.Width(40f)))
					UnityEditor.Selection.activeObject = _valueProfil;
			}
			GUILayout.EndHorizontal();

			if (Value != null)
			{
				_fold = EditorGUILayout.Foldout(_fold, "see details of injection", true);
				if (_fold)
				{
					drawDetails(Value);
				}
			}
		}

		/// <summary>
		/// when object field changes value
		/// how to apply to profil
		/// </summary>
		virtual protected void onValueChanged(T value)
		{
			string uid = value != null ? value.name : string.Empty;
			//Debug.Log(uid);
			EditorPrefs.SetString(pprefUid(), uid);
			_valuePrefs = fetchPrefsInstance();
		}

		/// <summary>
		/// major info of currently selected item
		/// </summary>
		virtual protected void drawHeader(T value)
		{ }

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
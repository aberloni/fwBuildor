using UnityEngine;

namespace fwp.buildor.editor
{
    using UnityEditor;

    abstract public class WinSubFieldApply<T> where T : ScriptableObject
    {
        protected WinEdBuildor win;
        
        bool _fold;
        
        public WinSubFieldApply(WinEdBuildor win)
        {
            this.win = win;

            fetchInstance();
        }

        protected T _value;
        public T value
        {
            get
            {
                fetchInstance();
                return _value;
            }

            set
            {
                if (value == _value) return;

                _value = value;
                string ppref = _value != null ? _value.name : string.Empty;
                EditorPrefs.SetString(pprefUid(), ppref);
                if (BuildorVerbosity.verbose) Debug.Log("sub field ppref value : " + ppref, _value);
            }
        }

        abstract protected string pprefUid();

        virtual public void fetchInstance()
        {
            string val = EditorPrefs.GetString(pprefUid(), string.Empty);

            // save new name if changed
            if (_value != null && !_value.name.Contains(val))
            {
                _value = null;
            }

            // if none yet, try to get ppref one
            if (_value == null && !string.IsNullOrEmpty(val))
            {
                _value = BuildorHelpers.getScriptableObjectInEditor<T>(val);
            }
        }

        virtual protected string getSectionTitle() => GetType().ToString();

        public void draw()
        {
            GUILayout.Label(getSectionTitle(), BuildorHelperGuiStyle.getCategoryBold());

            GUILayout.BeginHorizontal();
            var newValue = (T)EditorGUILayout.ObjectField(value, typeof(T), true);
            if (value != newValue)
            {
                value = newValue;
            }

            if(_value != null)
            {
                if (GUILayout.Button("apply", GUILayout.Width(50f)))
                {
                    apply();
                }
            }
            GUILayout.EndHorizontal();

            if(value != null)
            {
                drawContent();

                _fold = EditorGUILayout.Foldout(_fold, "see details", true);
                if (_fold)
                {
                    drawDetails();
                }
            }

        }

        virtual protected void drawContent()
        { }

        virtual protected void drawDetails()
        { }

        virtual protected void apply()
        { }
    }
}
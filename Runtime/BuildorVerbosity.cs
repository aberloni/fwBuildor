using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.buildor
{
    public class BuildorVerbosity
    {

        public const string _buildor_menuitem_path = "Window/Buildor/";

        const string _bool_verbose = "fwp.buildor.verbosity";

        static bool _verbose;
        static public bool verbose
        {
            get
            {
#if UNITY_EDITOR
                _verbose = UnityEditor.EditorPrefs.GetBool(_bool_verbose, false);
#endif
                return _verbose;
            }
            set
            {
                _verbose = value;
#if UNITY_EDITOR
                UnityEditor.EditorPrefs.SetBool(_bool_verbose, value);
#endif
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Window/Buildor/(toggle) verbosity")]
        static void miScreensVerbose()
        {
            verbose = !verbose;
            Debug.LogWarning(_bool_verbose + " : " + verbose);
        }
#endif

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.buildor
{
    static public class ScriptableSymbolHelper
    {
        static public string getGroupSymbols(BuildTargetGroup group)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        }
    }
}

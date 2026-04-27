namespace fwp.symbols
{
    using System;

    using UnityEditor;
    using System.Collections.Generic;
    using UnityEditor.Build;

    /// <summary>
    /// whatever is localy set (in unity)
    /// </summary>
    static public class ScriptSymbolsView
    {
        /// <summary>
        /// <Enum, Values>
        /// all values for each symbols enum
        /// </summary>
        static Dictionary<System.Type, string[]> buffer_symbols = new Dictionary<System.Type, string[]>();

        /// <summary>
        /// shk
        /// </summary>
        static public string getPlayerSetSymbols(BuildTargetGroup group)
        {
            try
            {
                var namedTarget = NamedBuildTarget.FromBuildTargetGroup(group);
                return PlayerSettings.GetScriptingDefineSymbols(namedTarget);
                // return PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            }
            catch
            {

                return string.Empty;
            }
        }

        static public void setPlayerSymbols(BuildTargetGroup group, string val, bool force = false)
        {
            if (!force)
            {
                string cur = ScriptSymbolsView.getPlayerSetSymbols(group);
                if (cur == val)
                {
                    UnityEngine.Debug.LogWarning("no need to apply, already at value=" + val);
                    return;
                }
            }

            UnityEngine.Debug.LogWarning(group + " = apply => " + val);
            var namedTarget = NamedBuildTarget.FromBuildTargetGroup(group);
            PlayerSettings.SetScriptingDefineSymbols(namedTarget, val);
            // PlayerSettings.SetScriptingDefineSymbolsForGroup(group, val);
        }

        static public Dictionary<System.Type, string[]> getEnumsBuffer(bool force = false)
        {
            if (buffer_symbols == null || force)
            {
                buffer_symbols = new Dictionary<Type, string[]>();
            }

            if (buffer_symbols.Count <= 0)
            {
                var enums = ScriptSymbols.extractEnums();

                foreach (var e in enums)
                {
                    if (!buffer_symbols.ContainsKey(e.GetType()))
                    {
                        buffer_symbols.Add(e.GetType(), null);
                    }

                    var nms = Enum.GetNames(e.GetType());
                    buffer_symbols[e.GetType()] = nms;
                }

            }

            return buffer_symbols;
        }
    }

}
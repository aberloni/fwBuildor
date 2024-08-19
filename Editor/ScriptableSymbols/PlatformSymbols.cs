using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.symbols
{
    using System;
    using UnityEditor;

    /// <summary>
    /// all symbols for a specific platform
    /// </summary>
    [System.Serializable]
    public class GroupSymbols
    {
        public BuildTargetGroup group = BuildTargetGroup.Unknown;
        public List<string> symbols = new List<string>();
        public string unityPlatformSymbols => ScriptSymbolsView.getPlayerSetSymbols(group);

        public bool has(string uid) => symbols.Contains(uid);

        /// <summary>
        /// extract from unity symbols
        /// </summary>
        public GroupSymbols extractDefaultContent()
        {
            // all helper enums
            var enums = ScriptSymbols.extractEnums();

            string[] symbolsParsed = unityPlatformSymbols.Split(';');

            //Debug.Log(group + " => " + unityPlatformSymbols);
            //Debug.Log("total enums x" + enums.Count);

            symbols.Clear();

            foreach (var e in enums)
            {
                var nms = Enum.GetNames(e.GetType());

                foreach (var nm in nms)
                {
                    // check si present dans les player settings
                    bool symbPresent = false;
                    for (int j = 0; j < symbolsParsed.Length; j++)
                    {
                        if (nm == symbolsParsed[j])
                            symbPresent = true;
                    }
                    if (symbPresent && !symbols.Contains(nm))
                    {
                        symbols.Add(nm);
                    }
                }
            }

            return this;
        }

    }

    /// <summary>
    /// all symbols of all platforms
    /// </summary>
    [System.Serializable]
    public class ScriptableSymbolsGroups
    {
        public List<GroupSymbols> groups = new List<GroupSymbols>();

        public ScriptableSymbolsGroups()
        { }

        public GroupSymbols extract(BuildTargetGroup tGroup)
        {
            return fetch(tGroup).extractDefaultContent();
        }

        public GroupSymbols fetch(BuildTargetGroup tGroup)
        {
            var g = getGroup(tGroup);
            if (g == null)
            {
                g = new GroupSymbols();
                g.group = tGroup;
                groups.Add(g);
                g.extractDefaultContent();
            }

            return g;
        }

        public GroupSymbols getGroup(BuildTargetGroup group)
        {
            foreach (var g in groups)
            {
                if (g.group == group) return g;
            }
            return null;
        }

        public string[] getSymbols(BuildTargetGroup group)
        {
            foreach (var g in groups)
            {
                if (g.group == group) return g.symbols.ToArray();
            }
            return null;
        }

        public bool drawToggles(BuildTargetGroup tGroup)
        {
            //int idx = 0;
            var allSymbols = ScriptSymbolsView.getEnumsBuffer();

            if (allSymbols.Count <= 0)
            {
                GUILayout.Label("no buffed symbols");
                return false;
            }

            var group = getGroup(tGroup);
            if (group == null)
            {
                if (GUILayout.Button("add group " + tGroup))
                {
                    group = fetch(tGroup);
                    Debug.Log(group.group);
                }
            }

            if (group == null)
            {
                GUILayout.Label("no group " + tGroup);
                return false;
            }

            bool changes = false;

            foreach (var kp in allSymbols)
            {
                // symbols.ScriptSymbolsGenerics => Generics
                string nm = kp.Key.ToString();
                string[] nms = nm.Split(".");
                if (nms.Length > 1) nm = nms[nms.Length - 1];
                nm = nm.Replace("ScriptSymbols", string.Empty);

                GUILayout.BeginHorizontal();

                GUILayout.Label(nm, GUILayout.Width(100f));

                for (int i = 0; i < kp.Value.Length; i++)
                {
                    var symb = kp.Value[i];

                    //symb.state = GUILayout.Toggle(symb.state, symb.label);
                    //symb.state = EditorGUILayout.ToggleLeft(symb.label, symb.state);
                    bool _has = group.symbols.Contains(symb);
                    bool _check = drawToggle(symb, _has);
                    if (_check != _has)
                    {
                        group.symbols.Remove(symb);
                        if (_check) group.symbols.Add(symb);

                        changes = true;
                    }
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("untick all"))
            {
                group.symbols.Clear();
                changes = true;
            }
            if (GUILayout.Button("Apply"))
            {
                mergeScriptableSymbols(group);
                changes = true;
                //EditorUtility.SetDirty(this);
            }
            GUILayout.EndHorizontal();

            return changes;
        }

        /// <summary>
        /// some symbols to add every time
        /// ";" separator
        /// </summary>
        string solvePredefSymbols()
        {
            return "UNITY_POST_PROCESSING_STACK_V2;";
        }

        private void mergeScriptableSymbols(GroupSymbols group)
        {
            string newScriptableSymbol = solvePredefSymbols();

            foreach (var key in group.symbols)
            {
                newScriptableSymbol += key + ";";
            }

            string cur = group.unityPlatformSymbols;
            if (cur != newScriptableSymbol)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group.group, newScriptableSymbol);
                Debug.Log(group.group + " => " + cur);
            }

        }

        public void drawRawStringSymbols(BuildTargetGroup tGroup)
        {
            var _group = getGroup(tGroup);
            if (_group == null)
            {
                GUILayout.Label("no group " + tGroup);
                return;
            }

            string value = _group.unityPlatformSymbols;

            if (string.IsNullOrEmpty(value))
                return;

            string[] vals = value.Split(";");

            string output = string.Empty;
            for (int i = 0; i < vals.Length; i++)
            {
                if (i > 0) output += " ; ";
                output += vals[i];

                if (i > 0 && i % 4 == 0)
                {
                    output += "\n";
                }

            }

            GUILayout.Label($"{output}");
        }


        bool drawToggle(string label, bool val)
        {
            //GUILayout.BeginHorizontal();

            //bool _val = EditorGUILayout.Toggle(label, val, GUILayout.Width(15f));
            bool _val = EditorGUILayout.ToggleLeft(label, val, GUILayout.Width(125f));
            //GUILayout.Label(label);
            //GUILayout.EndHorizontal();

            return _val;
        }

    }

}

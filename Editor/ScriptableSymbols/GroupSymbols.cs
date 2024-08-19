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

}

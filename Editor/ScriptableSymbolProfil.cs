using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using fwp.buildor.editor;
using fwp.buildor;

namespace fwp.symbols.editor
{
    [CreateAssetMenu(
        menuName = BuildorHelpers._menuItem_basepath + "(profil) scriptables symbols", order = 100)]
    public class ScriptableSymbolProfil : ScriptableObject
    {
        public ScriptableSymbolsGroups release;
        public ScriptableSymbolsGroups debug;

        public BuildTargetGroup ActiveGroup => WinEdBuildor.Profile.getBuildTargetGroup();
        public DebugLevel ActiveDebugLevel => WinEdBuildor.Profile.debugLevel;

        public int Count
        {
            get
            {
                int cnt = 0;
                foreach (var g in release.groups)
                {
                    cnt += g.symbols.Count;
                }
                if (ActiveDebugLevel == DebugLevel.debug)
                {
                    foreach (var g in debug.groups)
                    {
                        cnt += g.symbols.Count;
                    }
                }
                return cnt;
            }
        }

        /// <summary>
        /// concat of release+debug
        /// </summary>
        public string getStringifiedSymbols()
        {
            var _group = WinEdBuildor.Profile.getBuildTargetGroup();
            string output = release.extractSymbolsOfGroup(_group);
            if (ActiveDebugLevel == DebugLevel.debug) output += debug.extractSymbolsOfGroup(_group);
            return output;
        }

        [ContextMenu("apply to editor")]
        public void apply()
        {
            ScriptableSymbolsGroups.applyEditor(ActiveGroup, getStringifiedSymbols());
        }

        [ContextMenu("record    : defined platforms")]
        protected void cmExtracts()
        {
            foreach (var d in release.groups)
            {
                d.recordDefaultContent();
            }

            EditorUtility.SetDirty(this);
        }

        [ContextMenu("record    : current platform")]
        protected void cmRecord()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    release.extract(BuildTargetGroup.Standalone);
                    break;
                case RuntimePlatform.Android:
                    release.extract(BuildTargetGroup.Android);
                    break;
                case RuntimePlatform.Switch:
                    release.extract(BuildTargetGroup.Switch);
                    break;
                default:
                    throw new System.NotImplementedException(Application.platform + " not implem");
            }

            EditorUtility.SetDirty(this);
        }
    }

}


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
        public ScriptableSymbolsGroups groups;
        
        /// <summary>
        /// #if watermark
        /// </summary>
        public bool watermark;

        /// <summary>
        /// lang_en, lang_fr, ...
        /// </summary>
        public string[] langs;

        public int Count
        {
            get
            {
                int cnt = 0;
                foreach (var g in groups.groups)
                {
                    cnt += g.symbols.Count;
                }
                return cnt;
            }
        }

        /// <summary>
        /// concat of release+debug
        /// </summary>
        public string getStringifiedSymbols()
        {
            var _group = BuildorHelpers.Profile.getBuildTargetGroup();
            string output = groups.extractSymbolsOfGroup(_group);
            return output;
        }

        [ContextMenu("apply to editor")]
        public void apply()
        {
            ScriptSymbolsView.applyEditor(
                BuildorHelpers.Profile.getBuildTargetGroup(), getStringifiedSymbols());
        }

        [ContextMenu("record    : defined platforms")]
        protected void cmExtracts()
        {
            foreach (var d in groups.groups) d.recordDefaultContent();
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("record    : current platform")]
        protected void cmRecord()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    groups.extract(BuildTargetGroup.Standalone);
                    break;
                case RuntimePlatform.Android:
                    groups.extract(BuildTargetGroup.Android);
                    break;
                case RuntimePlatform.Switch:
                    groups.extract(BuildTargetGroup.Switch);
                    break;
                default:
                    throw new System.NotImplementedException(Application.platform + " not implem");
            }

            EditorUtility.SetDirty(this);
        }
    }

}


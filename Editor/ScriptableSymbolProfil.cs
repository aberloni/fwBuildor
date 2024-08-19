using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace fwp.symbols
{
    [CreateAssetMenu(
        menuName = fwp.buildor.editor.BuildorHelpers._menuItem_basepath + "(profil) scriptables symbols", order = 100)]
    public class ScriptableSymbolProfil : ScriptableObject
    {
        public ScriptableSymbolsGroups data;

        [ContextMenu("record    : defined platforms")]
        protected void cmExtracts()
        {
            foreach(var d in data.groups)
            {
                d.extractDefaultContent();
            }

            EditorUtility.SetDirty(this);
        }

        [ContextMenu("record    : current platform")]
        protected void cmRecord()
        {
            switch(Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    data.extract(BuildTargetGroup.Standalone);
                    break;
                case RuntimePlatform.Android:
                    data.extract(BuildTargetGroup.Android);
                    break;
                case RuntimePlatform.Switch:
                    data.extract(BuildTargetGroup.Switch);
                    break;
                default:
                    throw new System.NotImplementedException(Application.platform + " not implem");
            }

            EditorUtility.SetDirty(this);
        }
    }

}


using fwp.buildor;
using fwp.buildor.editor;
using UnityEngine;

namespace fwp.symbols.editor
{
    [CreateAssetMenu(
        menuName = fwp.buildor.BuildorHelpers._menuItem_basepath + "(profil) scriptables symbols", order = 100)]
    public class ScriptableSymbolProfil : ScriptableObject
    {
        /// <summary>
        /// #if watermark
        /// </summary>
        public bool watermark;

        /// <summary>
        /// lang_en, lang_fr, ...
        /// </summary>
        public string[] langs;

        /// <summary>
        /// concat of release+debug
        /// </summary>
        public string getStringifiedSymbols()
        {
            string ret = string.Empty;

            if (watermark) ret += "watermark;";

            if (langs != null) ret += BuildorHelpers.formatSymbols(langs);

            return ret;
        }
    }

}


using UnityEngine;

namespace fwp.buildor
{

    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+symbols", fileName = "symbols_")]
    public class BoduleSymbols : BuildModule
    {
        [SerializeField] string[] symbols = new string[0];

        public bool Has => symbols != null && symbols.Length > 0;

        public string Symbols => Has ? formatSymbols(symbols) : string.Empty;

        static public string formatSymbols(string[] symbols)
        {
            if (symbols == null || symbols.Length <= 0) return string.Empty;

            string ret = string.Empty;
            foreach (var s in symbols) ret += s + ";";
            return ret;
        }
    }

}
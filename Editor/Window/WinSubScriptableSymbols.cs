using UnityEditor;

namespace fwp.buildor.editor
{
    using fwp.symbols.editor;
    using UnityEngine;

    public class WinSubScriptableSymbols : WinSubFieldApply<ScriptableSymbolProfil>
    {
        public WinSubScriptableSymbols(WinEdBuildor win) : base(win)
        {
        }

        protected override string getSectionTitle() => "symbols (#if)";

        protected override ScriptableSymbolProfil fetchProfilInstance()
        {
            // var profil = BuildorHelpers.Profile;
            // if (profil == null) return null;
            // return profil.build.symbolsCustom;
            return null;
        }

        protected override void applyEditor(ScriptableSymbolProfil value)
        {
        }

        protected override void drawDetails(ScriptableSymbolProfil value)
        {
        }

        protected override void drawHeader(ScriptableSymbolProfil value)
        {
        }

    }

}
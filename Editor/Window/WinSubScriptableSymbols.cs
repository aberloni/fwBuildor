using UnityEditor;

namespace fwp.buildor.editor
{
    using fwp.symbols;
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
            var profil = WinEdBuildor.Profile;
            if (profil == null) return null;
            return profil.build.scriptSymbols;
        }

        protected override void applyEditor(ScriptableSymbolProfil value)
        {
            var profil = WinEdBuildor.Profile;
            if (profil == null) return;

            value.apply();
        }

        protected override void drawDetails(ScriptableSymbolProfil value)
        {
            var profil = WinEdBuildor.Profile;
            if (profil == null) return;

            // var group = value.ActiveGroup;
            string deets = value.getStringifiedSymbols();
            if (string.IsNullOrEmpty(deets)) deets = "-none-";
            GUILayout.Label(deets);
        }

        protected override void drawHeader(ScriptableSymbolProfil value)
        {
            var profil = WinEdBuildor.Profile;
            if (profil == null) return;

            GUILayout.Label("editor: " + ScriptSymbolsView.getPlayerSetSymbols(value.ActiveGroup));
        }

    }

}
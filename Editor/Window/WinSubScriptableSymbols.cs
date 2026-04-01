using UnityEditor;

namespace fwp.buildor.editor
{
    using fwp.symbols;

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

            value.data.apply(profil.getPlatformTargetGroup());
        }

        protected override void drawDetails(ScriptableSymbolProfil value)
        {
            var profil = WinEdBuildor.Profile;
            if (profil == null) return;
            bool changes = value.data.drawToggles(profil.getPlatformTargetGroup());
            if (changes)
            {
                EditorUtility.SetDirty(value);
            }
        }

        protected override void drawHeader(ScriptableSymbolProfil value)
        {
            var profil = WinEdBuildor.Profile;
            if (profil == null) return;
            value.data.drawRawStringSymbols(profil.getPlatformTargetGroup());
        }

    }

}
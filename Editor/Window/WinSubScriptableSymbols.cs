using UnityEngine;
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
            if (win == null) return null;
            if (win.ActiveProfil == null) return null;
            
            return win.ActiveProfil.scriptSymbols;
        }

        protected override void applyEditor(ScriptableSymbolProfil value)
        {
            if (win == null) return;
            if (win.ActiveProfil == null) return;

            value.data.apply(win.ActiveProfil.getPlatformTargetGroup());
        }

        protected override void drawDetails(ScriptableSymbolProfil value)
        {
            bool changes = value.data.drawToggles(win.ActiveProfil.getPlatformTargetGroup());
            if (changes)
            {
                EditorUtility.SetDirty(value);
            }
        }

        protected override void drawHeader(ScriptableSymbolProfil value)
        {
            value.data.drawRawStringSymbols(win.ActiveProfil.getPlatformTargetGroup());
        }

    }

}
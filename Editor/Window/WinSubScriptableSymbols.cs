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
            return win.activeProfil.scriptSymbols;
		}

		protected override void applyEditor(ScriptableSymbolProfil value)
		{
            value.data.apply(win.activeProfil.getPlatformTargetGroup());
		}

        protected override void drawDetails(ScriptableSymbolProfil value)
        {
            bool changes = value.data.drawToggles(win.activeProfil.getPlatformTargetGroup());
            if (changes)
            {
                EditorUtility.SetDirty(value);
            }
        }

        protected override void drawHeader(ScriptableSymbolProfil value)
        {
            value.data.drawRawStringSymbols(win.activeProfil.getPlatformTargetGroup());
        }

    }

}
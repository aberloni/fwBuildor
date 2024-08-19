using UnityEngine;
using UnityEditor;

namespace fwp.buildor.editor
{
    public class WinSubScriptableSymbols : WinSubFieldApply<fwp.symbols.ScriptableSymbolProfil>
    {
        public WinSubScriptableSymbols(WinEdBuildor win) : base(win)
        {
        }

        protected override string getSectionTitle() => "symbols (#if)";
        protected override string pprefUid() => "buildor.ssymbols";

        public override void fetchInstance()
        {
            base.fetchInstance();

            if(_value == null && win.activeProfil.scriptSymbols != null)
            {
                _value = win.activeProfil.scriptSymbols;
            }
        }

        protected override void drawDetails()
        {
            base.drawDetails();

            if(value != null)
            {
                bool changes = value.data.drawToggles(win.activeProfil.getPlatformTargetGroup());
                if (changes)
                {
                    EditorUtility.SetDirty(value);
                }
            }
            
        }

        protected override void drawContent()
        {
            base.drawContent();

            value?.data.drawRawStringSymbols(win.activeProfil.getPlatformTargetGroup());
        }

        protected override void apply()
        {
            base.apply();
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace fwp.symbols
{

    /// <summary>
    /// 
    /// Editor Window to display toggles to manage scriptable symbols per platforms
    /// 
    /// https://docs.unity3d.com/ScriptReference/EditorGUILayout.DropdownButton.html
    /// https://forum.unity.com/threads/looking-for-a-simple-editorgui-dropdownbutton-example.768530/
    /// 
    /// /// NN_PLUGIN_ENABLE
    /// https://developer.nintendo.com/html/online-docs/g1kr9vj6-en/document.html?doc=Packages/middleware/UnityForNintendoSwitch/UnityForNintendoSwitchDevManual-en.html?docname=Unity%20for%20Nintendo%20Switch%20Development%20Manual
    /// 
    /// </summary>

    public class WinEdScriptSymbols : UnityEditor.EditorWindow
    {

        [UnityEditor.MenuItem("Tools/(window) scriptable symbols editor")]
        static public void init() => UnityEditor.EditorWindow.GetWindow(typeof(WinEdScriptSymbols));

        Vector2 scroll;

        BuildTargetGroup curTarget;

        ScriptableSymbolsGroups sGroups;

        private void OnEnable()
        {
            titleContent = new GUIContent("Scriptable Symbols Helper");

            curTarget = ScriptSymbols.evaluateCurrentTargetPlatform();
        }

        private void OnFocus()
        {
            ScriptSymbolsView.getEnumsBuffer(true);
            refresh();
        }

        public void refresh(bool force = false)
        {
            if (force)
            {
                curTarget = ScriptSymbols.evaluateCurrentTargetPlatform();
            }

            if(sGroups == null || force)
            {
                sGroups = new ScriptableSymbolsGroups();
                sGroups.fetch(BuildTargetGroup.Standalone);
                sGroups.fetch(BuildTargetGroup.Switch);
            }
        }

        //bool ppGetScriptable(string label) => PlayerPrefs.GetInt("scritpable_" + label, 0) == 1;
        //void ppSetScriptable(string label, bool val) => PlayerPrefs.GetInt("scritpable_" + label, val ? 1 : 0);

        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("not at runtime");
                return;
            }

            EditorGUILayout.LabelField("platform detected : " + curTarget);

            BuildTargetGroup nxt = (BuildTargetGroup)EditorGUILayout.EnumPopup("target platform", curTarget);
            if (nxt != curTarget)
            {
                // assign will also solve() values
                curTarget = nxt;
                sGroups.fetch(curTarget);
            }

            sGroups.drawToggles(curTarget);
            sGroups.drawRawStringSymbols(curTarget);

            //drawStringSymbols(tar.getPlayerSetSymbols());

            GUILayout.Space(20f);

            drawPlatformsSymbols();
        }

        void drawPlatformsSymbols()
        {
            GUILayout.Label("Unity Scripting Define Symbols (raw) : ");

            scroll = GUILayout.BeginScrollView(scroll);
            string[] nms = System.Enum.GetNames(typeof(BuildTargetGroup));
            for (int i = 0; i < nms.Length; i++)
            {
                GUILayout.BeginHorizontal();
                //BuildTargetGroup curGroup = ScriptSymbols.platformToSysTarGroup((BuildTargetGroup)i);

                var curGroup = (BuildTargetGroup)i;
                GUILayout.Label(curGroup.ToString(), GUILayout.Width(250f));
                
                string symbols = ScriptSymbolsView.getPlayerSetSymbols(curGroup);
                if(!string.IsNullOrEmpty(symbols)) GUILayout.Label(symbols);

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }

}

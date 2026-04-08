using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.buildor
{
    static public class HelperGui
    {

        const float refSize = 1280f;
        const float refTextSize = 16f;

        static public int getPropSizedTextSize(float addRatio = 1f)
        {
            float ratio = (Screen.width * 1f) / refSize; // 0.6f pour 1280
            return (int)((refTextSize * ratio) * addRatio);
        }

        public static readonly GUIStyle gWinTitle;
        public static readonly GUIStyle gWhiteBold;
        public static readonly GUIStyle gButtonBig;
        public static readonly GUIStyle gColoredButtonRed;
        public static readonly GUIStyle gCategoryBold;
        public static readonly GUIStyle gBold;
        public static readonly GUIStyle gLabelBoldGizmos;

        public static readonly GUILayoutOption labelW = GUILayout.Width(200f);

        public static readonly GUILayoutOption bXS = GUILayout.Width(50f);
        public static readonly GUILayoutOption bS = GUILayout.Width(75f);
        public static readonly GUILayoutOption bM = GUILayout.Width(100f);
        public static readonly GUILayoutOption bL = GUILayout.Width(150f);

        public static readonly GUIStyle gWrapped = new GUIStyle(GUI.skin.label)
        {
            // normal.textColor = Color.white;
            wordWrap = true
        };

        static HelperGui()
        {
            gWinTitle = new GUIStyle();
            gWinTitle.richText = true;
            gWinTitle.alignment = TextAnchor.MiddleCenter;
            gWinTitle.normal.textColor = Color.white;
            gWinTitle.fontSize = 20;
            gWinTitle.fontStyle = FontStyle.Bold;
            gWinTitle.margin = new RectOffset(10, 10, 10, 10);

            gWhiteBold = new GUIStyle();
            gWhiteBold.richText = true;
            gWhiteBold.fontStyle = FontStyle.Bold;
            gWhiteBold.padding = new RectOffset(10, 5, 2, 2);
            gWhiteBold.normal.textColor = Color.white;

            gButtonBig = new GUIStyle(GUI.skin.button);
            gButtonBig.fontSize = 25;
            gButtonBig.fontStyle = FontStyle.Bold;
            gButtonBig.fixedHeight = 50f;

            gColoredButtonRed = new GUIStyle(GUI.skin.button);
            gColoredButtonRed.normal.textColor = Color.red;

            gCategoryBold = new();
            gCategoryBold.padding.left = 10;
            gCategoryBold.padding.top = 5;
            gCategoryBold.fontStyle = FontStyle.Bold;
            gCategoryBold.normal.textColor = new Color(1f, 0.2f, 0.20f);

            gBold = new();
            gBold.padding.left = 10;
            gBold.padding.top = 5;
            gBold.fontStyle = FontStyle.Bold;
            gBold.normal.textColor = Color.white;

            gLabelBoldGizmos = new GUIStyle();
            gLabelBoldGizmos.richText = true;
            gLabelBoldGizmos.fontStyle = FontStyle.Bold;
            //gLabelBoldGizmos.padding = new RectOffset(10, 50, 20, 20);
            gLabelBoldGizmos.normal.textColor = Color.blue;
            gLabelBoldGizmos.fontSize = 20;

        }

    }

}

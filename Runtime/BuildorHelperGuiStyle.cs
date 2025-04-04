﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.buildor
{
    /// <summary>
    /// extracted : 2022-11-08
    /// </summary>
    static public class BuildorHelperGuiStyle
    {


        const float refSize = 1280f;
        const float refTextSize = 16f;

        static public int getPropSizedTextSize(float addRatio = 1f)
        {
            float ratio = (Screen.width * 1f) / refSize; // 0.6f pour 1280
            return (int)((refTextSize * ratio) * addRatio);
        }

        static private GUIStyle gWinTitle;
        static public GUIStyle getWinTitle()
        {
            if (gWinTitle == null)
            {
                gWinTitle = new GUIStyle();

                gWinTitle.richText = true;
                gWinTitle.alignment = TextAnchor.MiddleCenter;
                gWinTitle.normal.textColor = Color.white;
                gWinTitle.fontSize = 20;
                gWinTitle.fontStyle = FontStyle.Bold;
                gWinTitle.margin = new RectOffset(10, 10, 10, 10);
                //gWinTitle.padding = new RectOffset(30, 30, 30, 30);

            }

            return gWinTitle;
        }

        static private GUIStyle gWhiteBold;
        static public GUIStyle getWhiteBold()
        {
            if (gWhiteBold == null)
            {
                gWhiteBold = new GUIStyle();
                gWhiteBold.richText = true;
                gWhiteBold.fontStyle = FontStyle.Bold;
                gWhiteBold.padding = new RectOffset(10, 5, 2, 2);
                gWhiteBold.normal.textColor = Color.white;
            }

            return gWhiteBold;
        }

        static private GUIStyle gButtonBig;
        static public GUIStyle getButtonBig(float height)
        {
            if (gButtonBig == null) gButtonBig = new GUIStyle(GUI.skin.button);
            gButtonBig.fixedHeight = height;
            return gButtonBig;
        }

        static private GUIStyle gButtonColored;
        static public GUIStyle getButtonColored(Color color)
        {
            if (gButtonColored == null) gButtonColored = new GUIStyle(GUI.skin.button);
            gButtonColored.normal.textColor = color;
            return gButtonColored;
        }

        static private GUIStyle gCategoryBold;
        static public GUIStyle getCategoryBold()
        {
            if (gCategoryBold != null) return gCategoryBold;
            return getCategoryBold(new Color(1f, 0.2f, 0.20f));
        }
        static public GUIStyle getCategoryBold(Color color)
        {
            if (gCategoryBold == null)
            {
                gCategoryBold = new GUIStyle();
                gCategoryBold.padding.left = 10;
                gCategoryBold.padding.top = 5;
                gCategoryBold.fontStyle = FontStyle.Bold;
            }

            gCategoryBold.normal.textColor = color;

            return gCategoryBold;
        }

        static private GUIStyle gLabelBoldGizmos;
        static public GUIStyle getLabelBoldGizmos()
        {
            if (gLabelBoldGizmos == null)
            {
                gLabelBoldGizmos = new GUIStyle();
                gLabelBoldGizmos.richText = true;
                gLabelBoldGizmos.fontStyle = FontStyle.Bold;
                //gLabelBoldGizmos.padding = new RectOffset(10, 50, 20, 20);
                gLabelBoldGizmos.normal.textColor = Color.blue;
                gLabelBoldGizmos.fontSize = 20;
            }

            return gLabelBoldGizmos;
        }

    }

}

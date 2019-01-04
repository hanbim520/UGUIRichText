﻿//盛大游戏 张海军 710605420@qq.com


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

namespace SDGame.UI.RichText
{
    public class CreateRichTextMenu
    {
        [MenuItem("GameObject/UI/Rich Text", false, 1901)]
        private static void _AddRichText (MenuCommand menuCommand)
        {
            GameObject go = CreateRichText();
            MenuOptions.PlaceUIElementRoot(go, menuCommand);
        }

        public static GameObject CreateRichText ()
        {
            GameObject go = DefaultControls.CreateUIElementRoot("RichText", s_ThickElementSize);

            var lbl = go.AddComponent<RichText>();
            lbl.text = "New RichText";
            lbl.raycastTarget = false;

            SetDefaultTextValues(lbl);

            return go;
        }

        private static void SetDefaultTextValues (Text lbl)
        {
            lbl.color = s_TextColor;
            lbl.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private const float  kWidth       = 160f;
        private const float  kThickHeight = 30f;
        private static Vector2 s_ThickElementSize       = new Vector2(kWidth, kThickHeight);
        private static Color   s_TextColor              = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);
    }
}
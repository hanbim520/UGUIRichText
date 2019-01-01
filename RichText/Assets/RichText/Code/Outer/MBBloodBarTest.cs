using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SDGame.UI.RichText
{
    public class MBBloodBarTest : MonoBehaviour
    {
        public Button btn;

        private void Awake ()
        {
            AssetLoader.Instance.LoadAll();

            btn.onClick.AddListener(() => {
                UnityEngine.SceneManagement.SceneManager.LoadScene("test");
            });
        }

        private void Update ()
        {
            if (null == foregroundRichText)
            {
                return;
            }

            var tags = foregroundRichText.GetSpriteTags();
            var count = tags.Count;
            for (int i= 0; i< count; ++i)
            {
                var tag = tags[i];
                var amount = Time.time - (int) Time.time;
                tag.SetFillMethod(SpriteTag.FillMethod.Horizontal);
                tag.SetFillAmount(amount);
            }
            foregroundRichText.SetNativeSize();
        }

        private void OnDestroy ()
        {
            UIAtlasManager.Instance.Clear();
        }

        public RichText foregroundRichText;
    }
}
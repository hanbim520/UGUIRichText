using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

namespace SDGame.UI.RichText
{
    public class MBDrawCallTest : MonoBehaviour
    {
        public Button btn;
        protected void Awake ()
        {
            AssetLoader.Instance.LoadAll();
            btn.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("test");
            });

//             var canvasArray = FindObjectsOfType<Canvas>();
//             var length = canvasArray.Length;
//             if (length > 0)
//             {
//                 var currentCanvas = canvasArray[0];
//                 var spriteAssetPaths = AssetLoader.Instance.GetSpriteAssetPaths();
//                 for (int i= 0; i< spriteAssetPaths.Length -4; ++i)
//                 {
//                     var assetPath = spriteAssetPaths[i];
//                     var goParent = new GameObject(assetPath);
//                     goParent.transform.SetParent(currentCanvas.transform, false);
// 
//                     var parent = goParent.transform;
//                     _CreateRichText(parent, assetPath);
//                 }
// 
                 StartCoroutine(_CoUpdateSprite()); 
//             }
//             string url = "<a href=http://news.sina.com.cn?u=2&ts=14321&page=1 target=\"_blank\">测试</a>";
//             Regex reg = new Regex(@"(?is)<a(?:(?!href=).)*href=(['""]?)(?<url>[^""\s>]*)\1[^>]*>(?<text>(?:(?!</?a\b).)*)</a>");
//             MatchCollection mc = reg.Matches(url);
//             foreach (Match m in mc)
//             {
//                 Debug.Log(m.Groups["url"].Value);
//                 Debug.Log(m.Groups["text"].Value);
//             }

        }
        public static bool IsURL(string str)
        {
            string pattern = @"^(https?|ftp|file|ws)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
            return IsMatch(pattern, str);
        }

        public static bool IsMatch(string expression, string str)
        {
            Regex reg = new Regex(expression);
            if (string.IsNullOrEmpty(str))
                return false;
            return reg.IsMatch(str);
        }
        public static string BuildUrl(string url, string ParamText, string ParamValue)
        {
            Regex reg = new Regex(string.Format("{0}=[^&]*", ParamText), RegexOptions.IgnoreCase);
            Regex reg1 = new Regex("[&]{2,}", RegexOptions.IgnoreCase);
            string _url = reg.Replace(url, "");
            if (_url.IndexOf("?") == -1)
                _url += string.Format("?{0}={1}", ParamText, ParamValue);//?
            else
                _url += string.Format("&{0}={1}", ParamText, ParamValue);//&
            _url = reg1.Replace(_url, "&");
            _url = _url.Replace("?&", "?");
            return _url;
        }
        private void _CreateRichText (Transform parent, string spriteDataPath)
        {
            var count = 1;
            for (int i= 0; i< count; ++i)
            {
                var go = new GameObject();
                var trans = go.AddComponent<RectTransform>();
                go.transform.SetParent(parent, false);
                trans.position = new Vector2(500, 1 * 100);
                trans.sizeDelta = new Vector2(500, 100);

                var richText = go.AddComponent<RichText>();
                richText.font =  Resources.GetBuiltinResource<Font>("Arial.ttf");
                richText.fontSize = 30;
                string url = string.Format("测试用的文字<quad name=meat_1 src={0} width=35 height=30/>尾巴", spriteDataPath);
               // string url = "啊啊啊啊啊啊啊啊啊<a href=mujian>[测试][测试][测试][测试][测试][测试][测试][测试][测试][测试][测试][测试][测试]</a>";
                richText.text = url;
                // richText.text = "测试用的文字尾巴";

                Debug.Log(richText.preferredWidth);

                richText.SetNativeSize();
                
                go.name = "RichText_" + i.ToString();

                _richTextObjects.Add(go);
            }
        }

        protected void OnDestroy()
        {
            foreach (var go in _richTextObjects)
            {
                GameObject.DestroyImmediate(go);    
            }

            _richTextObjects.Clear();
            UIAtlasManager.Instance.Clear();
        }

        private IEnumerator _CoUpdateSprite ()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);

                var richTexts = FindObjectsOfType<RichText>();
                foreach (var richText in richTexts)
                {
                    var tags = richText.GetSpriteTags();
                    var count = tags.Count;
                    for (int i= 0; i< count; ++i)
                    {
                        var tag = tags[i];

                        var atlas = tag.GetAtlas();
                        var sprites = atlas.GetSprites();
                        var sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
                        tag.SetName(sprite.name);

                        richText.SetVerticesDirty();
                    }
                }
            }
        }

        private readonly List<GameObject> _richTextObjects = new List<GameObject>();
    }
}
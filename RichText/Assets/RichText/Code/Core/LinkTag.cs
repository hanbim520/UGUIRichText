using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace SDGame.UI.RichText
{
    [System.Serializable]
    public class LinkTag
    {
        public enum FillMethod
        {
            None,
            Horizontal,
//            Vertical,
        }
        private Dictionary<string, string> paramDic = new Dictionary<string, string>();
        public List<Rect> Boxes = new List<Rect>();
        private bool isvalid = false;
        public LinkTag(RichText richText)
        {
            if (null == richText)
            {
                throw new ArgumentNullException("richText is null.");
            }

            _richText = richText;
        }
        public bool IsValid()
        {
            return isvalid == true;
        }
        public void SetStartIndex(int index)
        {
            _startIndex = index;
        }
        public int GetStartIndex()
        {
            return _startIndex;
        }
        public int GetEndIndex()
        {
            return _endIndex;
        }
        public void SetEndIndex(int index)
        {
            _endIndex = index;
        }
        public static MatchCollection GetMatches (string strText)
        {
            return _linkTagRegex.Matches(strText);
        }

        public bool SetValue (Match match)
        {
            if (null == match)
            {
                return false;
            }

            var keyCaptures = match.Groups[1].Captures;
            var valCaptures = match.Groups[2].Captures;

            var count = keyCaptures.Count;
            if (count != valCaptures.Count)
            {
                return false;
            }
            paramDic.Clear();
            for (int i = 0; i < keyCaptures.Count; ++i)
            {
                var key = keyCaptures[i].Value;
                var val = valCaptures[i].Value;
                _CheckSetValue(match, key, val);
            }

            isvalid = true;

            return true;
        }

        public Dictionary<string,string> GetParams()
        {
            return paramDic;
        }
        private void _CheckSetValue (Match match, string key, string val)
        {
            paramDic.Add(key, val);
//             else if (key == "width")
//             {
//                 float width;
//                 float.TryParse(val, out width);
// //                 if (_richText.fontSize < width)
// //                     width = _richText.fontSize;
//                 _size.x = width;
//             }
//             else if (key == "height")
//             {
//                 float height;
//                 float.TryParse(val, out height);
// //                 if (_richText.fontSize < height)
// //                     height = _richText.fontSize;
//                 _size.y = height;
//             }

        }

        public void Reset ()
        {
            _startIndex = 0;
            _endIndex = 0;
            isvalid = false;
        }

        public int GetVertexIndex ()
        {
            return _startIndex;
        }
//         public void SetLinkParams(string param)
//         {
//             _linkParams = param;
//        //         public void SetName(string name)
//         {
//             isvalid = true;
//             _name = name;
//         }
//        public string GetName()
//         {
//             return _name;
//              
//         public string GetLinkParams()
//         {
//             return _linkParams;
//        

       public float GetOffset ()
        {
            return _offset;
        }

        public void SetFillMethod (FillMethod fillMethod)
        {
            _fillMethod = fillMethod;
        }

        public FillMethod GetFillMethod ()
        {
            return _fillMethod;
        }

        public void SetFillAmount (float amount)
        {
            amount = Mathf.Clamp01(amount);

            float eps = 0.001f;
            var delta = _fillAmount - amount;
            if (delta > eps || delta < -eps)
            {
                _fillAmount = amount;
                _richText.SetVerticesDirty();
            }
        }

        public float GetFillAmount ()
        {
            return _fillAmount;
        }

        private void _SetAtlas (UIAtlas atlas)
        {
            _atlas = atlas;

            if (null == atlas)
            {
                return;
            }

            var richText = _richText;
            var mat = richText.material;
            var manager = MaterialManager.Instance;
            var lastSpriteTexture = manager.GetSpriteAtlas(mat);
            var spriteTexture = atlas.GetTexture();

            var isTextureChanged = lastSpriteTexture != spriteTexture;
            if (isTextureChanged)
            {
                manager.DetachTexture(richText, lastSpriteTexture);
                manager.AttachTexture(richText, spriteTexture);
            }
        }

        public UIAtlas GetAtlas ()
        {
            return _atlas;
        }

        private RichText _richText;
        private UIAtlas  _atlas;

        private int _endIndex = 0;
       // private string _linkParams;
        private int _startIndex;
        //private string _name;

        private float _offset = 0;

        private float _fillAmount = 1.0f;
        private FillMethod _fillMethod = FillMethod.None;


        // private static readonly string mHrefRegex = @"(?is)<a(?:(?!href=).)*href=(['""]?)(?<url>[^""\s>]*)\1[^>]*>(?<text>(?:(?!</?a\b).)*)</a>";
        // private static readonly string mHrefRegex = @"<a param=([^>\n\s]+)>(.*?)(</a>)";//可用
        //  private static readonly string mHrefRegex = @"<a(?:\s+(\w+)\s*=\s*(?<quota>['""]?)([\w\/]+)\k<quota>)+\s*\>(.*?)(</a>)";
        // private static readonly string mHrefRegex = @"<url id=([^>\n\s]+)\s(?:\s+(\w+)>(.*?)(</url>)";\
        private static readonly string mHrefRegex = @"<a(?:\s+(\w+)\s*=\s*(?<quota>['""]?)([\w\/]+)\k<quota>)+\s*\>(?<text>(?:(?!</?a\b).)*)</a>";
        private static readonly Regex _linkTagRegex = new Regex(mHrefRegex, RegexOptions.Singleline);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SDGame.UI.RichText
{
    partial class RichText
    {
        private static readonly StringBuilder mTextBuilder = new StringBuilder();
        private readonly List<LinkTag> mUnderlineTagInfos = new List<LinkTag>();

        private IList<UIVertex> verts = null;
       

        [Serializable]
        public class RichTextClickEvent : UnityEvent<Dictionary<string, string>> { }

        [SerializeField]
        private RichTextClickEvent m_OnClick = new RichTextClickEvent();

        public RichTextClickEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            foreach (var hrefInfo in mUnderlineTagInfos)
            {
                var boxes = hrefInfo.Boxes;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(lp))
                    {
                        m_OnClick.Invoke(hrefInfo.GetParams());
                        return;
                    }
                }
            }
        }
        private void _ParseLinkTags(ref string strText)
        {
            if (string.IsNullOrEmpty(strText) )
            {
                _ResetLinkTags(0);
                return;
            }
            mUnderlineTagInfos.Clear();
            int index = 0;
            mTextBuilder.Length = 0;
            var indexText = 0;
            foreach (Match match in LinkTag.GetMatches(strText))
            {
                mTextBuilder.Append(strText.Substring(indexText, match.Index - indexText));

                var group = match.Groups[1];
                LinkTag tag = _GetLinkTag(index);
                tag.SetStartIndex(mTextBuilder.Length * 4);
                tag.SetEndIndex((mTextBuilder.Length + match.Groups["text"].Length - 1) * 4 + 3);
                mUnderlineTagInfos.Add(tag);
                ++index;
                mTextBuilder.Append(match.Groups["text"].Value);
                indexText = match.Index + match.Length;
                tag.SetValue(match);
            }
            mTextBuilder.Append(strText.Substring(indexText, strText.Length - indexText));
          
            strText = mTextBuilder.ToString();
            _ResetLinkTags(index);
        }
        private void _HandleLinkTag(VertexHelper toFill,IList<UIVertex> verts, TextGenerationSettings setting)
        {
            this.verts = verts;
            GetBounds(toFill, mUnderlineTagInfos);
            //绘制underline  实现有点bug，先不开启
//             TextGenerator textGenerator = new TextGenerator();
//             textGenerator.Populate("_", setting);
//             IList<UIVertex> underlineVerts = textGenerator.verts;
//             for (int m = 0; m < mUnderlineTagInfos.Count; ++m)
//             {
//                 var underlineInfo = mUnderlineTagInfos[m];
//                 if (!underlineInfo.IsValid())
//                 {
//                     continue;
//                 }
//                 if (underlineInfo.GetStartIndex() >= mVertexHelperRef.currentVertCount)
//                 {
//                     continue;
//                 }
//                
//                 for (int i = 0; i < underlineInfo.Boxes.Count; i++)
//                 {
//                     Vector3 startBoxPos = new Vector3(underlineInfo.Boxes[i].x, underlineInfo.Boxes[i].y, 0.0f);
//                     Vector3 endBoxPos = startBoxPos + new Vector3(underlineInfo.Boxes[i].width, 0.0f, 0.0f);
//                     float height = underlineInfo.Boxes[i].height;
//                     float fontSizeHeight = GetComponent<Text>().preferredHeight;
//                     AddUnderlineQuad(underlineVerts, startBoxPos, endBoxPos);
//                 } 
//             }

        }
        #region 添加下划线  
        private void AddUnderlineQuad(IList<UIVertex> underlineVerts, Vector3 startBoxPos, Vector3 endBoxPos)
        {
            Vector3[] underlinePos = new Vector3[4];
            //             underlinePos[0] = startBoxPos + new Vector3(0, fontSize * -0.1f, 0);
            //             underlinePos[1] = endBoxPos + new Vector3(0, fontSize * -0.1f, 0); ;
            //             underlinePos[2] = endBoxPos + new Vector3(0, fontSize * 0f, 0);
            //             underlinePos[3] = startBoxPos + new Vector3(0, fontSize * 0f, 0);

            underlinePos[0] = startBoxPos + new Vector3(0, fontSize * -0.1f, 0);
            underlinePos[1] = endBoxPos + new Vector3(0, fontSize * -0.1f, 0); ;
            underlinePos[2] = endBoxPos + new Vector3(0, fontSize * 0f, 0);
            underlinePos[3] = startBoxPos + new Vector3(0, fontSize * 0f, 0);
            for (int i = 0; i < 4; ++i)
            {
                int tempVertsIndex = i & 3;
                _tempVerts[tempVertsIndex] = underlineVerts[i % 4];
                _tempVerts[tempVertsIndex].color = Color.blue;
                _tempVerts[tempVertsIndex].position = underlinePos[i];
                if (tempVertsIndex == 3)
                {
                    mVertexHelperRef.AddUIVertexQuad(_tempVerts);
                }
            }
        }


        #endregion
        private void GetBounds(VertexHelper toFill,List<LinkTag> m_HrefInfos)
        {
            SetNativeSize();
            UIVertex vert0 = new UIVertex();
            UIVertex vert1 = new UIVertex();
            UIVertex vert2 = new UIVertex();
            UIVertex vert3 = new UIVertex();
            //float fontSz = fontSize * 0.5f;
            float unitsPerPixel = 1 / pixelsPerUnit;
            UIVertex vertPre = new UIVertex();

            // 处理超链接包围框
            foreach (var hrefInfo in m_HrefInfos)
            {
                hrefInfo.Boxes.Clear();
                if (hrefInfo.GetStartIndex() >= toFill.currentVertCount)
                {
                    continue;
                }
                int lineCount = 0;
                var pos = verts[hrefInfo.GetStartIndex()].position;
                var bounds = new Bounds(pos, Vector3.zero);
//                 Rect rect = new Rect();
//                 float maxY = 0;
//                 float minX = 0;
//                 float maxHeight = 0;
               toFill.PopulateUIVertex(ref vertPre, hrefInfo.GetStartIndex() + 3);
                for (int i = hrefInfo.GetStartIndex(), m = hrefInfo.GetEndIndex(); i < m; i+=4)
                {
                    if (i >= toFill.currentVertCount)
                    {
                        break;
                    }

                    toFill.PopulateUIVertex(ref vert0, i);
                    toFill.PopulateUIVertex(ref vert1, i+1);
                    toFill.PopulateUIVertex(ref vert2, i+2);
                    toFill.PopulateUIVertex(ref vert3, i+3);
                    Rect rect = new Rect();
                    rect.Set(vert3.position.x, vert3.position.y,Mathf.Abs(vert2.position.x - vert3.position.x), Mathf.Abs( vert2.position.y - vert1.position.y));
                     hrefInfo.Boxes.Add(rect);
                    //                     vert0 = verts[i];
                    //                     vert1 = verts[i+1];
                    //                     vert2 = verts[i+2];
                    //                     vert3 = verts[i+3];

                    //                     bool newline = Mathf.Abs(vert2.position.y - vertPre.position.y) > fontSize;
                    //                     if (newline)
                    //                     {
                    //                         maxHeight = 0;
                    //                         maxY = 0;
                    //                         minX = 0;
                    // 
                    //                         hrefInfo.Boxes.Add(rect);
                    //                         rect = new Rect();
                    //                         lineCount++;
                    //                     }
                    //                     else
                    //                     {
                    //                         if (maxY < vert3.position.y)
                    //                         {
                    //                             maxY = vert3.position.y;
                    //                         }
                    //                         if(minX > vert3.position.x)
                    //                         {
                    //                             minX = vert3.position.x;
                    //                         }
                    //                         float height = Mathf.Abs(vert2.position.y - vert1.position.y);
                    //                         if (maxHeight > height)
                    //                         {
                    //                             maxHeight = height;
                    //                         }
                    //                         rect.Set(minX * unitsPerPixel, maxY * unitsPerPixel, Mathf.Abs(vert2.position.x - minX) , height);
                    // 
                    //                     }
                    //                     vertPre = verts[i +3];
                }

            //    hrefInfo.Boxes.Add(rect);
            //    Debug.Log("lineCount： " + lineCount);
            }
        }

//         //添加下划线
//         private void AddUnderlineQuad(IList<UIVertex> underlineVerts, Vector3 startBoxPos, Vector3 endBoxPos)
//         {
//             Vector3[] underlinePos = new Vector3[4];
//             underlinePos[0] = startBoxPos + new Vector3(0, fontSize * -0.1f, 0);
//             underlinePos[1] = endBoxPos + new Vector3(0, fontSize * -0.1f, 0); ;
//             underlinePos[2] = endBoxPos + new Vector3(0, fontSize * 0f, 0);
//             underlinePos[3] = startBoxPos + new Vector3(0, fontSize * 0f, 0);
// 
//             for (int i = 0; i < 4; ++i)
//             {
//                 int tempVertsIndex = i & 3;
//                 m_TempVerts[tempVertsIndex] = underlineVerts[i % 4];
//                 m_TempVerts[tempVertsIndex].color = Color.blue;
//                 m_TempVerts[tempVertsIndex].position = underlinePos[i];
//                 if (tempVertsIndex == 3)
//                 {
//                     mVertexHelperRef.AddUIVertexQuad(m_TempVerts);
//                 }
//             }
//         }
    }

}

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
    public struct BoxStruct
    {
        public int line;
        public int start;
        public int end;
    }
    partial class RichText
    {
        public float LineOffset = 1f;
        public float LineThickness = 2;
        public Color UnderlineColor = Color.black;

        private static readonly UIVertex[] data = new UIVertex[4];
        private static readonly StringBuilder mTextBuilder = new StringBuilder();
        private readonly List<LinkTag> mUnderlineTagInfos = new List<LinkTag>();
        List<UILineInfo> m_Lines = new List<UILineInfo>(20);
        private IList<UIVertex> verts = null;

        private List<Rect> draw = new List<Rect>();

        [Serializable]
        public class RichTextClickEvent : UnityEvent<Dictionary<string, string>> { }

        [SerializeField]
        private RichTextClickEvent m_OnClick = new RichTextClickEvent();

        public RichTextClickEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }
        public void GetLines(List<UILineInfo> lines)
        {
            cachedTextGenerator.GetLines(lines);
        }

        private Vector2 GetUnderlineCharUV()
        {
            var ch = '*';
            CharacterInfo info;
            if (font.GetCharacterInfo(ch, out info, fontSize, fontStyle))
            {
                return (info.uvBottomLeft + info.uvBottomRight + info.uvTopLeft + info.uvTopRight) * 0.25f;
            }
            Debug.LogWarning("GetCharacterInfo failed");
            return Vector2.zero;
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
        void OnGUI()
        {
            GUIStyle style = new GUIStyle
            {
                border = new RectOffset(10, 10, 10, 10),
                fontSize = 50,
                fontStyle = FontStyle.BoldAndItalic,
            };
            // normal:Rendering settings for when the component is displayed normally.
            style.normal.textColor = new Color(200 / 255f, 180 / 255f, 150 / 255f);    // 需要除以255，因为范围是0-1

            if (draw == null) return;
//             for(int i = 0;i < draw.Count;++i)
//             {
// 
//                 Gizmos.DrawGUITexture(draw[i], new Texture());
//             }
            
        }

        private void _HandleLinkTag(VertexHelper toFill,IList<UIVertex> verts, TextGenerationSettings setting)
        {
            this.verts = verts;
            GetLines(m_Lines);
            GetBounds(toFill, mUnderlineTagInfos);
            //绘制underline  实现有点bug，先不开启
            TextGenerator textGenerator = new TextGenerator();
            textGenerator.Populate("_", setting);
            IList<UIVertex> underlineVerts = textGenerator.verts;
            for (int m = 0; m < mUnderlineTagInfos.Count; ++m)
            {
                var underlineInfo = mUnderlineTagInfos[m];
                if (!underlineInfo.IsValid())
                {
                    continue;
                }
                if (underlineInfo.GetStartIndex() >= mVertexHelperRef.currentVertCount)
                {
                    continue;
                }
               
                for (int i = 0; i < underlineInfo.Boxes.Count; i++)
                {
                    Vector3 startBoxPos = new Vector3(underlineInfo.Boxes[i].x, underlineInfo.Boxes[i].y - 1, 0.0f);
                    Vector3 endBoxPos = startBoxPos + new Vector3(underlineInfo.Boxes[i].width, 0.0f, 0.0f);
                    AddUnderlineQuad(underlineVerts, startBoxPos, endBoxPos);
                } 
            }

        }
        #region 添加下划线  
        private void AddUnderlineQuad(IList<UIVertex> underlineVerts, Vector3 startBoxPos, Vector3 endBoxPos)
        {
            Vector3[] underlinePos = new Vector3[4];
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

        private int GetCharLine(int charIndex)
        {
            int line = 0;
            for(int i = 0;i <m_Lines.Count;++i)
            {
                if(m_Lines[i].startCharIdx> charIndex)
                {
                    return line;
                }
                line = i;
            }
            return line;
        }

        private List<BoxStruct> ConstructBox(int startIndex,int endIndex)
        {
            startIndex = startIndex / 4;
            endIndex = (endIndex - 3) / 4;
            List<BoxStruct> box = new List<BoxStruct>();
            BoxStruct boxStruct;
            int preLine = 0;
            boxStruct = new BoxStruct();
            int line = GetCharLine(startIndex );
            preLine = line;
            boxStruct.line = line;
            boxStruct.start = startIndex;
            for (int i=startIndex;i<= endIndex;++i)
            {
                line = GetCharLine(i);
                if(preLine != line)
                {
                    box.Add(boxStruct);
                    preLine = line;
                    boxStruct.line = line;
                    boxStruct.start = i;
                }
                else
                {
                    boxStruct.end = i;
                }
            }
            box.Add(boxStruct);
            return box;
        }
        #endregion
        private void GetBounds(VertexHelper toFill,List<LinkTag> m_HrefInfos)
        {
            SetNativeSize();
            UIVertex vertStart3 = new UIVertex();
            UIVertex vertEnd1 = new UIVertex();
          
            //float fontSz = fontSize * 0.5f;
            float unitsPerPixel = 1 / pixelsPerUnit;
            UIVertex vertPre = new UIVertex();
            draw.Clear();
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

                List<BoxStruct> box = ConstructBox(hrefInfo.GetStartIndex(), hrefInfo.GetEndIndex());

                UIVertex vert = UIVertex.simpleVert;

                for (int i = 0;i <box.Count;++i)
                {
                    BoxStruct boxStruct = box[i];
                    Rect rect = new Rect();
                   
                    int startVert = boxStruct.start * 4 + 3;
                    int endVert = boxStruct.end * 4 + 1;
                    if (startVert >= toFill.currentVertCount || endVert >= toFill.currentVertCount)
                    {
                        break;
                    }
                    toFill.PopulateUIVertex(ref vertStart3, startVert);
                    toFill.PopulateUIVertex(ref vertEnd1, endVert);

                    rect.Set(vertStart3.position.x, vertStart3.position.y, Mathf.Abs(vertEnd1.position.x - vertStart3.position.x), m_Lines[boxStruct.line].height);
                   // rect.Set(vertStart3.position.x, vertStart3.position.y, Mathf.Abs(vertEnd1.position.x - vertStart3.position.x), Mathf.Abs(vertEnd1.position.y - vertStart3.position.y));
                    hrefInfo.Boxes.Add(rect);
                    draw.Add(rect);


//                     toFill.PopulateUIVertex(ref vert, startVert);
//                     var pos_a = vertStart3.position;
//                     toFill.PopulateUIVertex(ref vert, boxStruct.end * 4 - 2);
//                     var pos_b = vert.position;
//                     var y = (pos_a.y + pos_b.y) * 0.5f;
// 
//                     var pos0 = new Vector3(pos_a.x, y - LineOffset);
//                     var pos1 = new Vector3(pos_b.x, y - LineOffset);
//                     var pos2 = new Vector3(pos_b.x, y - LineOffset - LineThickness);
//                     var pos3 = new Vector3(pos_a.x, y - LineOffset - LineThickness);
// 
//                     vert.color = UnderlineColor;
//                     vert.uv0 = GetUnderlineCharUV();
// 
//                     vert.position = pos0;
//                     data[0] = vert;
//                     vert.position = pos1;
//                     data[1] = vert;
//                     vert.position = pos2;
//                     data[2] = vert;
//                     vert.position = pos3;
//                     data[3] = vert;
// 
//                     toFill.AddUIVertexQuad(data);
                }
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

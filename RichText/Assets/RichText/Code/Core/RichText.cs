using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace SDGame.UI.RichText
{
    [ExecuteInEditMode]
    public partial class RichText : Text, IPointerClickHandler
    {
        [SerializeField]
        private SpriteAtlas[] m_Atlas;

        private VertexHelper mVertexHelperRef;
        private Font mFont;

        private WaitForSeconds waitFrame = new WaitForSeconds(0.33f);

        protected override void Awake()
        {
            base.Awake();
            if(m_Atlas != null )
            {
                for(int i =0;i < m_Atlas.Length;++i)
                {
                    UIAtlasManager.Instance.Add(m_Atlas[i].name, m_Atlas[i]);
                }
            }
        }

        protected override void OnEnable()
        {
            this.supportRichText = true;

            mFont = font;
            _ParseText();
            SetVerticesDirty();

            StartCoroutine(_CoUpdateSprite());
            base.OnEnable();
        }

        private void InitImage()
        {
            var tags = GetSpriteTags();
            var count = tags.Count;
            for (int i = 0; i < count; ++i)
            {
                var tag = tags[i];
                if (tag.GetStartFrame() == tag.GetStartFrame() && tag.GetStartFrame() == 0)
                {
                    tag.SetAniName(tag.GetName());
                    
                }
            }
            SetVerticesDirty();
        }

        private IEnumerator _CoUpdateSprite()
        {                       
            while (true)
            {
                yield return waitFrame;

                var tags = GetSpriteTags();
                var count = tags.Count;
                for (int i = 0; i < count; ++i)
                {
                    var tag = tags[i];
                    if (tag.GetStartFrame() == tag.GetStartFrame() && tag.GetStartFrame() != 0)
                    {
                        int frameLength = tag.GetEndFrame() - tag.GetStartFrame();
                        tag.PlayerCurrentFrame = (tag.PlayerCurrentFrame++) % (frameLength) + tag.GetEndFrame();
                        //                     var atlas = tag.GetAtlas();
                        //                     var sprites = atlas.GetSprites();
                        //                     var sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
                        //待优化
                        string spriteName = string.Format("{0}_{1}", tag.GetName(), tag.PlayerCurrentFrame);
                        tag.SetAniName(spriteName);
                        
                    }
                }
                SetVerticesDirty();
            }
        }

        private void Update()
        {
            
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            var manager = MaterialManager.Instance;
            var lastSpriteTexture = manager.GetSpriteAtlas(material);
            manager.DetachTexture(this, lastSpriteTexture);

            if (m_Atlas != null)
            {
                for (int i = 0; i < m_Atlas.Length; ++i)
                {
                    UIAtlasManager.Instance.Remove(m_Atlas[i].name);
                }
            }

            base.OnDestroy();
        }

        public override string text
        {
            get
            {
                return m_Text;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(m_Text))
                    {
                        return;
                    }

                    m_Text = string.Empty;

                    _ParseText();
                    SetVerticesDirty();
                }
                else if (m_Text != value)
                {
                    m_Text = value;
                    _ParseText();

                    SetVerticesDirty();
                    SetLayoutDirty();
                }
            }
        }

        private void _ParseText()
        {
            _parseOutputText = text;
            _ParseLinkTags(ref _parseOutputText);
            _ParseSpriteTags(_parseOutputText);
          
        }

        private void LateUpdate()
        {
            if (rectTransform.hasChanged)
            {
                rectTransform.hasChanged = false;
                SetVerticesDirty();
            }
        }
      
        protected override void OnPopulateMesh (VertexHelper toFill)
        {
            if (null == font)
            {
                return;
            }
            //来自官方Text源码
            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.Populate(_parseOutputText, settings);

            Rect inputRect = rectTransform.rect;

            // get the text alignment anchor point for the text in local space
            Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
            Vector2 refPoint = Vector2.zero;
            refPoint.x = (textAnchorPivot.x == 1 ? inputRect.xMax : inputRect.xMin);
            refPoint.y = (textAnchorPivot.y == 0 ? inputRect.yMin : inputRect.yMax);

            // Determine fraction of pixel to offset text mesh.
            Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            //Last 4 verts are always a new line...
            int vertCount = verts.Count - 4;

            toFill.Clear();

            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    _tempVerts[tempVertsIndex] = verts[i];
                    _tempVerts[tempVertsIndex].position *= unitsPerPixel;
                    _tempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    _tempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    _tempVerts[tempVertsIndex].uv1 = new Vector2(1.0f, 0);

                    if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(_tempVerts);
                    }
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    _tempVerts[tempVertsIndex] = verts[i];
                    _tempVerts[tempVertsIndex].position *= unitsPerPixel;
                    _tempVerts[tempVertsIndex].uv1 = new Vector2(1.0f, 0);

                    if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(_tempVerts);
                    }
                }
            }
            mVertexHelperRef = toFill;
            _HandleSpriteTag(toFill);
            _HandleLinkTag(toFill, verts, settings);
            m_DisableFontTextureRebuiltCallback = false;
        }


        private readonly UIVertex[] _tempVerts = new UIVertex[4];
        private string _parseOutputText;
    }
}
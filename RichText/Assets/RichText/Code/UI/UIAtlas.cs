
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

namespace SDGame.UI
{
    public class UIAtlas : IDisposable
    {
        public int Reference = 0;
        public UIAtlas (SpriteAtlas spriteAtlas)
        {
            if (null == spriteAtlas)
            {
                throw new ArgumentNullException("spriteAtlas is null.");
            }
            Reference++;
            _spriteAtlas = spriteAtlas;
        }

        ~UIAtlas ()
        {
        }

        public void Dispose ()
        {
            if (_isDisposed)
            {
                return;
            }

            GC.SuppressFinalize(this);
            _isDisposed = true;
        }


        public Sprite GetSprite (string name)
        {
            name = name ?? string.Empty;
            Sprite[] allSpts = GetSprites();
            for(int i = 0;i < allSpts.Length;++i)
            {
                if(allSpts[i].name == name)
                {
                    return allSpts[i];
                }
            }
           // var sprite = _spriteAtlas.GetSprite(name);
            return null;
        }

        public Sprite[] GetSprites ()
        {
            if (_sprites == null)
            {
                _sprites = new Sprite[_spriteAtlas.spriteCount];
                _spriteAtlas.GetSprites(_sprites);
                for (int i = 0; i < _sprites.Length; ++i)
                {
                    _sprites[i].name = _sprites[i].name.Replace("(Clone)", "");
                }
            }

            return _sprites;
        }

        public Texture GetTexture ()
        {
            _texture = GetSprites()[0].texture;
            return _texture;
        }

        private SpriteAtlas _spriteAtlas;
        private bool _isDisposed;

        private Sprite[]    _sprites;
        private Texture _texture;
    }
}
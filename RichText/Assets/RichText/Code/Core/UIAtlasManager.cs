//盛大游戏 张海军 710605420@qq.com

using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

namespace SDGame.UI.RichText
{
    public class UIAtlasManager
    {
        public void Add (string key, SpriteAtlas spriteAtlas)
        {
            if (null == key || null == spriteAtlas)
            {
                return;
            }
            if(!_atlasMap.ContainsKey(key))
            {
                _atlasMap[key] = new UIAtlas(spriteAtlas);
            }else
            {
                (_atlasMap[key] as UIAtlas).Reference++;
            }
            
        }

        public UIAtlas Get (string key)
        {
            key = key ?? string.Empty;
            var sprite = _atlasMap[key] as UIAtlas;
            return sprite;
        }

        public Sprite GetSprite(string atlas,string name)
        {
            atlas = atlas ?? string.Empty;
            name = name ?? string.Empty;
            var spriteatlas = _atlasMap[atlas] as UIAtlas;
            return spriteatlas.GetSprite(name);
        }
        public void Remove (string key)
        {
            key = key ?? string.Empty;
            if(_atlasMap.ContainsKey(key))
            {
                UIAtlas atlas = _atlasMap[key] as UIAtlas;
                atlas.Reference--;
                if (atlas.Reference == 0)
                {
                    atlas.Dispose();
                    _atlasMap.Remove(key);
                }
            }
           
        }

        public void Clear ()
        {
            if (_atlasMap.Count > 0)
            {
                var iter = _atlasMap.GetEnumerator();
                while (iter.MoveNext())
                {
                    var atlas = iter.Value as UIAtlas;
                    if (null != atlas)
                    {
                        atlas.Dispose();
                    }
                }

                _atlasMap.Clear();
            }
        }

        private readonly Hashtable _atlasMap = new Hashtable();
        public static readonly UIAtlasManager Instance = new UIAtlasManager();
    }
}
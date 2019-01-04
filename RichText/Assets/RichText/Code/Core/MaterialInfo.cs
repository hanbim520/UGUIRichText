﻿//盛大游戏 张海军 710605420@qq.com

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SDGame.UI.RichText
{
    internal class MaterialInfo
    {
        public MaterialInfo (Material mat)
        {
            if (null == mat)
            {
                throw new ArgumentNullException();
            }

            _material = mat;
        }

        public void Attach (Graphic target)
        {
            if (null == target)
            {
                return;
            }

            target.material = _material;
            _observers.Add(target, this);
        }

        public void Detach (Graphic target)
        {
            if (null == target)
            {
                return;
            }

            _observers.Remove(target);
        }

        public Material GetMaterial ()
        {
            return _material;
        }

        public int GetCount ()
        {
            return _observers.Count;
        }

        private Material _material;
        private readonly Hashtable _observers = new Hashtable();
    }
}
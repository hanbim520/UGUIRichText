using System;
using System.Collections;
//盛大游戏 张海军 710605420@qq.com

using System.Collections.Generic;

namespace SDGame
{
    internal static class ExtendedIList
    {
        public static void ForEachNotNullEx<T> (this IList<T> list, Action<T> action) where T : class
        {
            if (null == list || null == action)
            {
                return;
            }

            var count = list.Count;
            for (int i= 0; i< count; ++i)
            {
                var item = list[i];
                if (null != item)
                {
                    action(item);
                }
            }
        }

        public static void ForEachNotNullEx<T, Arg1> (this IList<T> list, Action<T, Arg1> action, Arg1 arg1) where T : class
        {
            if (null == list || null == action)
            {
                return;
            }

            var count = list.Count;
            for (int i= 0; i< count; ++i)
            {
                var item = list[i];
                if (null != item)
                {
                    action(item, arg1);
                }
            }
        }

        public static void EnsureSizeEx<T> (this IList<T> list, int size)
        {
            if (null != list)
            {
                var count = list.Count;
                for (int i= count; i< size; ++i)
                {
                    list.Add(default(T));
                }
            }
        }
    }
}
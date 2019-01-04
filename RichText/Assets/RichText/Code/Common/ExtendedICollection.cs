//盛大游戏 张海军 710605420@qq.com

using System.Collections;
using System.Collections.Generic;

namespace SDGame
{
    internal static class ExtendedICollection
    {
        public static bool IsNullOrEmptyEx<T> (this ICollection<T> collection)
        {
            return null == collection || collection.Count == 0;
        }
    }
}
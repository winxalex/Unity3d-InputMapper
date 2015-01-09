using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ws.winx.utils
{
    public static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

   


		public static void Resize<T>(this List<T> list, int sz, T c)
		{
			int cur = list.Count;
			if(sz < cur)
				list.RemoveRange(sz, cur - sz);
			else if(sz > cur)
			{
				if(sz > list.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
					list.Capacity = sz;
				list.AddRange(Enumerable.Repeat(c, sz - cur));
			}
		}

		public static void Resize<T>(this List<T> list, int sz)
		{
			Resize(list, sz, default(T));
		}
    }
}

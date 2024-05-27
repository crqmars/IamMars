using System;

namespace System
{
    public static class Int16Extensions
    {
        /// <summary>
        /// 返回int值是否介于两者（含）之间
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsBetween(this short source, short start, short end)
        {
            return start <= source && source <= end;
        }
    }

    public static class Int32Extensions
    {
        /// <summary>
        /// 返回int值是否介于两者（含）之间
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsBetween(this int source, int start, int end)
        {
            return start <= source && source <= end;
        }
    }

    public static class Int64Extensions
    {
        /// <summary>
        /// 返回int值是否介于两者（含）之间
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsBetween(this long source, long start, long end)
        {
            return start <= source && source <= end;
        }
    }
}

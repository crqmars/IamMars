using System;

namespace System
{
    /// <summary>
    /// DateTime 扩展类
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// 返回ComUse.TimeHelper.GetTime时间戳
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ToPyTime(this DateTime source, ComUse.Enumerate.TimeType dtType = ComUse.Enumerate.TimeType.秒)
        {
            if (source == DateTime.MinValue || source == DateTime.MaxValue)
                return 0;
            return ComUse.TimeHelper.GetTime(source, dtType);
        }

        /// <summary>
        /// 获取精确到秒的时间
        /// </summary>
        /// <returns></returns>
        public static DateTime ToSecondTime(this DateTime source)
        {
            return source.AddMilliseconds(-source.Millisecond);
        }

        /// <summary>
        /// 获取DateTimeOffset对象的Date部分Offset表示
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTimeOffset GetOffsetDate(this DateTimeOffset source)
        {
            return new DateTimeOffset(source.Date, source.Offset);
        }

        /// <summary>
        /// 获取日期所在周一日期
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime GetMonday(this DateTime source)
        {
            int diff = source.DayOfWeek - DayOfWeek.Monday;
            if (diff == -1) diff = 6;
            return source.Date.AddDays(-diff);
        }

        /// <summary>
        /// 返回值是否介于开始（含）至结束（不含）之间
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime source, DateTime start, DateTime end)
        {
            return start <= source && source < end;
        }
    }
}

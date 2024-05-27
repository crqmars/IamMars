using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// List 扩展类
    /// </summary>
    public static class ListExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// 是否是null或空
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count == 0;
        }

        /// <summary>
        /// 串联集合成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string StringJoin<T>(this IEnumerable<T> source, string separator = ",")
        {
            if (source == null)
                return string.Empty;
            return string.Join(separator, source);
        }

        /// <summary>
        /// 转换成下发格式字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToResString<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return "" ;
            return $"[{string.Join(",", source)}]";
        }

        /// <summary>
        /// 转换成下发格式字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="func">末尾要带,</param>
        /// <returns></returns>
        public static string ToResString<T>(this IEnumerable<T> source, Func<T, string> func)
        {
            if (source.IsNullOrEmpty())
                return "";
            var builder = new Text.StringBuilder("[");
            foreach (var item in source)
            {
                var itemStr = func(item);
                if (string.IsNullOrEmpty(itemStr))
                    continue;
                builder.Append(itemStr);
            }
            if (builder.Length > 1) builder.Remove(builder.Length - 1, 1);//去除最后一个逗号，更符合json格式
            builder.Append("]");
            return builder.ToString();
        }

        /// <summary>
        /// 打乱数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(this List<T> source)
        {
            if (source == null || source.Count <= 1)
                return source;
            for (int i = source.Count - 1; i > 0; i--)
            {
                int target = ComUse.RandomHelper.NextSafe(i + 1);
                var temp = source[i];
                source[i] = source[target];
                source[target] = temp;
            }
            return source;
        }

        /// <summary>
        /// 打乱数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static void Shuffle<T>(this List<T> source, int beginIndex, int endIndex)
        {
            if (source == null || source.Count <= 1)
                return;
            if (beginIndex < 0)
                beginIndex = 0;
            if (endIndex >= source.Count)
                endIndex = source.Count - 1;
            for (int i = endIndex; i > beginIndex; i--)
            {
                int target = ComUse.RandomHelper.NextSafe(beginIndex, i + 1);
                (source[target], source[i]) = (source[i], source[target]);
            }
        }

        /// <summary>
        /// 打乱数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static void Shuffle<T>(this List<T> source, Random random)
        {
            if (source == null || source.Count <= 1)
                return;
            for (int i = source.Count - 1; i > 0; i--)
            {
                int target = random.Next(i + 1);
                var temp = source[i];
                source[i] = source[target];
                source[target] = temp;
            }
        }

        /// <summary>
        /// 乱序遍历
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static void ShuffleEach<T>(this List<T> source, Action<T> action)
        {
            if (source == null || source.Count <= 1)
                return;
            for (int i = source.Count - 1; i > 0; i--)
            {
                int target = ComUse.RandomHelper.NextSafe(i + 1);
                var targetItem = source[target];
                var tempItem = source[i];
                source[i] = targetItem;
                source[target] = tempItem;
                action(targetItem);
            }
            action(source[0]);
        }

        /// <summary>
        /// 随机从列表取N个不重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="num"></param>
        /// <param name="canUpset">允许打乱原list</param>
        /// <returns></returns>
        public static IEnumerable<T> GetRandomItems<T>(this List<T> source, int num, bool canUpset = true)
        {
            if (source == null || source.Count <= num)
                return source;
            if (!canUpset)
            {
                source = source.ToList();
            }
            for (int i = 0; i < num; i++)
            {
                int target = ComUse.RandomHelper.NextSafe(i, source.Count);
                var temp = source[i];
                source[i] = source[target];
                source[target] = temp;
            }
            return source.Take(num);
        }

        /// <summary>
        /// 随机从列表取一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetRandomItem<T>(this List<T> source)
        {
            if (source == null || source.Count <= 0)
                return default(T);

            int rIndex = ComUse.RandomHelper.NextSafe(0, source.Count);
            return source[rIndex];
        }

        /// <summary>
        /// 从列表里随机一个Item
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetRandomItem<T>(this IEnumerable<T> source)
        {
            var count = source.Count();
            if (source == null || count <= 0)
                return default;
            if (count == 1)
                return source.First();
            var randomIndex = ComUse.RandomHelper.NextSafe(0, count);
            foreach (var item in source)
            {
                if (randomIndex-- == 0) return item;
            }
            return default;
        }

        public static void SetMaxID<T>(this IEnumerable<T> source, Func<T, int> maxFunc, ref int curMaxID, int defaultMaxID = 1)
        {
            if (source.Count() == 0)
            {
                curMaxID = defaultMaxID;
                return;
            }
            int newMax = source.Max(maxFunc);
            if (newMax > curMaxID)
            {
                curMaxID = newMax;
            }
        }

        /// <summary>
        /// 转换成和源长度相等的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<T> ToListTrim<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentException("source");
            }
            if (source is ICollection<T> collection)
            {
                return new List<T>(collection);
            }
            var list = new List<T>(source.Count());
            foreach (var item in source)
            {
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 转换成和源长度相等的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="capacity">初始容量</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<T> ToListTrim<T>(this IEnumerable<T> source, int capacity)
        {
            if (source == null)
            {
                throw new ArgumentException("source");
            }
            if (source is ICollection<T> collection)
            {
                return new List<T>(collection);
            }
            var list = new List<T>(capacity);
            foreach (var item in source)
            {
                list.Add(item);
            }
            return list;
        }
    }
}

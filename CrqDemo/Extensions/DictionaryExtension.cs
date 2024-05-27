using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// Dictionary 扩展类
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 是否是null或空
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            return source == null || source.Count == 0;
        }

        /// <summary>
        /// 转换成和源长度相等的Dictionary
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TSource> ToDictionaryTrim<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ToDictionaryTrim(source, keySelector, item => item);
        }

        /// <summary>
        /// 转换成和源长度相等的Dictionary
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Dictionary<TKey, TElement> ToDictionaryTrim<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            if (source == null)
            {
                throw new ArgumentException("source");
            }
            var dictionary = new Dictionary<TKey, TElement>(source.Count());
            foreach (var item in source)
            {
                dictionary.Add(keySelector(item), elementSelector(item));
            }
            return dictionary;
        }
    }
}

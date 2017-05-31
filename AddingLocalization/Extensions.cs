using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace AddingLocalization
{
    public static class Extensions
    {
        public static bool IsSubsetOf<T>(this IEnumerable<T> setA, IEnumerable<T> setB, IEqualityComparer<T> comparer)
        {
            return (setA.Intersect(setB, comparer).Count() == setA.Count());
        }

        public static int CountOf(this string s, char c)
        {
            var t = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c)
                    t++;
            }

            return t;
        }

        public static int IndexOf(this string s, char[] c, int startIndex)
        {
            for(int i = startIndex; i < s.Length; i++)
            {
                if (c.Contains(s[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static Dictionary<string, string> ToDictionary(this ResXResourceReader reader, bool throwExceptions = false, IEqualityComparer<string> comparer = null)
        {
            Dictionary<string, string> dic;

            if (comparer != null)
                dic = new Dictionary<string, string>(comparer);
            else
                dic = new Dictionary<string, string>();

            foreach (DictionaryEntry i in reader)
            {
                try
                {
                    dic.Add((string)i.Key, (string)i.Value);
                }
                catch (Exception e)
                {
                    if (throwExceptions)
                        throw e;
                }
            }

            return dic;
        }

        public static Dictionary<T, int> ToCountDictionary<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> comparer = null)
        {
            Dictionary<T, int> dic = null;

            if (comparer != null)
                dic = new Dictionary<T, int>(comparer);
            else
                dic = new Dictionary<T, int>();

            foreach (var item in enumerable)
            {
                if (dic.ContainsKey(item))
                    dic[item] = dic[item] + 1;
                else
                    dic.Add(item, 1);
            }

            return dic;
        }

        public static Dictionary<T, K> ToDictionary<T, K>(this ResXResourceReader reader, bool throwExceptions = false, IEqualityComparer<T> comparer = null)
        {
            Dictionary<T, K> dic;

            if (comparer != null)
                dic = new Dictionary<T, K>(comparer);
            else
                dic = new Dictionary<T, K>();

            foreach (DictionaryEntry i in reader)
            {
                try
                {
                    dic.Add((T)i.Key, (K)i.Value);
                }
                catch (Exception e)
                {
                    if (throwExceptions)
                        throw e;
                }
            }

            return dic;
        }

        public static Dictionary<K, T> ReverseDictionary<T, K>(this Dictionary<T, K> d, IEqualityComparer<K> comparer = null)
        {
            Dictionary<K, T> r;

            if (comparer != null)
                r = new Dictionary<K, T>(comparer);
            else
                r = new Dictionary<K, T>();

            foreach (var e in d)
            {
                r.Add(e.Value, e.Key);
            }

            return r;
        }

        public static void AddRange<T>(this HashSet<T> s, IEnumerable<T> r)
        {
            foreach(var i in r)
            {
                s.Add(i);
            }
        }

        public static void ConvertUTF8ToBOM(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
                ConvertUTF8ToBOM(filePath);
        }

        public static void ConvertUTF8ToBOM(string filePath)
        {
            var text = File.ReadAllText(filePath);

            text = text.Take(3) + text;

            File.WriteAllText(filePath, text, Encoding.UTF8);
        }
    }
}

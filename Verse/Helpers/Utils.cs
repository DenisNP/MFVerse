using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Verse.Helpers
{
    public static class Utils
    {
        public static readonly JsonSerializerSettings ConverterSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };
        
        public static string SafeSubstring(this string s, int len)
        {
            return s.Length <= len ? s : s.Substring(0, len);
        }

        public static T PickRandom<T>(this IList<T> list)
        {
            var rng = new Random();
            return list[rng.Next(list.Count)];
        }
        
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
        
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
        
        public static string Join(this IEnumerable<string> s, string separator)
        {
            return string.Join(separator, s);
        }
        
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list, int? seed = null)  
        {
            Random rng = seed == null ? new Random() : new Random(seed.Value);
            var buffer = list.ToList();
            for (var i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];
                buffer[j] = buffer[i];
            }
        }

        public static string GetNumericPhrase(int num, string one, string few, string many)
        {
            num = num < 0 ? 0 : num;
            string postfix;

            if (num < 10)
            {
                if (num == 1) postfix = one;
                else if (num > 1 && num < 5) postfix = few;
                else postfix = many;
            }
            else if (num <= 20)
            {
                postfix = many;
            }
            else if (num <= 99)
            {
                int lastOne = num - (int)Math.Floor((double)num / 10) * 10;
                postfix = GetNumericPhrase(lastOne, one, few, many);
            }
            else
            {
                int lastTwo = num - (int)Math.Floor((double)num / 100) * 100;
                postfix = GetNumericPhrase(lastTwo, one, few, many);
            }
            return postfix;
        }

        public static string ToPhrase(this int num, string one, string few, string many)
        {
            return num + " " + GetNumericPhrase(num, one, few, many);
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            var comparer = Comparer<TKey>.Default;

            using IEnumerator<TSource> sourceIterator = source.GetEnumerator();
            if (!sourceIterator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");
            
            TSource min = sourceIterator.Current;
            TKey minKey = selector(min);
            while (sourceIterator.MoveNext())
            {
                TSource candidate = sourceIterator.Current;
                TKey candidateProjected = selector(candidate);
                if (comparer.Compare(candidateProjected, minKey) < 0)
                {
                    min = candidate;
                    minKey = candidateProjected;
                }
            }
            return min;
        }
        
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            var comparer = Comparer<TKey>.Default;

            using IEnumerator<TSource> sourceIterator = source.GetEnumerator();
            if (!sourceIterator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");
            
            TSource max = sourceIterator.Current;
            TKey maxKey = selector(max);
            while (sourceIterator.MoveNext())
            {
                TSource candidate = sourceIterator.Current;
                TKey candidateProjected = selector(candidate);
                if (comparer.Compare(candidateProjected, maxKey) > 0)
                {
                    max = candidate;
                    maxKey = candidateProjected;
                }
            }
            return max;
        }

        public static int WordsBeginningsMatch(string w1, string w2)
        {
            var count = 0;
            int minLen = Math.Min(w1.Length, w2.Length);
            
            for (var i = 0; i < minLen; i++)
            {
                if (w1[i] == w2[i])
                    count++;
                else
                    break;
            }

            return count;
        }
        
        public static double WordsBeginningsMatchRatio(string w1, string w2, double threshold = 0.0)
        {
            int matches = WordsBeginningsMatch(w1, w2);
            double avgLen = (w1.Length + w2.Length) / 2.0;
            if (avgLen == 0.0) return 0.0;
            double match = 1.0 * matches / avgLen;
            return match >= threshold ? match : 0.0;
        }

        public static string ToUpperFirst(this string s)
        {
            if (s.IsNullOrEmpty()) return s;
            if (s.Length == 1) return s.ToUpper();

            return s[0].ToString().ToUpper() + s.Substring(1);
        }
        
        public static int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
                return 0;
            if (string.IsNullOrEmpty(a))
                return b.Length;
            if (string.IsNullOrEmpty(b))
                return a.Length;
            
            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (var i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (var j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (var i = 1; i <= lengthA; i++)
            for (var j = 1; j <= lengthB; j++)
            {
                int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                distances[i, j] = Math.Min
                (
                    Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                    distances[i - 1, j - 1] + cost
                );
            }
            return distances[lengthA, lengthB];
        }
        
        public static int LevenshteinDistance(string[] a, string[] b)
        {
            if (a.Length == 0 && b.Length == 0)
                return 0;
            if (a.Length == 0)
                return b.Length;
            if (b.Length == 0)
                return a.Length;
            
            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (var i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (var j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (var i = 1; i <= lengthA; i++)
            for (var j = 1; j <= lengthB; j++)
            {
                int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                distances[i, j] = Math.Min
                (
                    Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                    distances[i - 1, j - 1] + cost
                );
            }
            return distances[lengthA, lengthB];
        }

        public static double LevenshteinMatchRatio(string a, string b)
        {
            int maxLen = Math.Max(a.Length, b.Length);
            if (maxLen == 0)
                return 0.0;

            int levDist = LevenshteinDistance(a, b);
            return 1.0 - (double) levDist / maxLen;
        }
        
        public static double LevenshteinMatchRatio(string[] a, string[] b)
        {
            int maxLen = Math.Max(a.Length, b.Length);
            if (maxLen == 0)
                return 0.0;

            int levDist = LevenshteinDistance(a, b);
            return 1.0 - (double) levDist / maxLen;
        }
        
        public static bool IsSimilarTokens(string[] expected, string[] tokens, int maxExcessAllowed)
        {
            return tokens.Length <= expected.Length + maxExcessAllowed && expected.All(tokens.Contains);
        }

        public static string GetStepsName(int stepsCount)
        {
            string prefix = stepsCount switch
            {
                0 => "",
                1 => "одно",
                2 => "двух",
                3 => "трёх",
                4 => "четырёх",
                5 => "пяти",
                6 => "шести",
                7 => "семи",
                8 => "восьми",
                9 => "девяти",
                10 => "десяти",
                _ => "много"
            };

            return $"{prefix}стопный";
        }
    }
}
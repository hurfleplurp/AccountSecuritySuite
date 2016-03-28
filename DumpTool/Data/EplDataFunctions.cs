using System;
using System.Collections.Generic;
using System.Linq;

namespace DumpTool.Data
{
    public class EplDataFunctions
    {
        public static Dictionary<string, List<Tuple<string, string>>> SortedTldBucketDictionary(
            List<Tuple<string, string>> sourceList)
        {
            Dictionary<string, List<Tuple<string, string>>> approxCountryBuckets = new Dictionary<string, List<Tuple<string, string>>>();

            foreach (var item in sourceList)
            {
                string[] afterAt = item.Item1.Split('@');
                if (afterAt.Length > 2)
                    continue;
                string[] tlds = afterAt[afterAt.Length - 1].Split('.');
                string tldKey = "";
                if (tlds.Length == 2)
                    tldKey = $".{tlds[1]}";
                if (tlds.Length == 3)
                    tldKey = $".{tlds[1]}.{tlds[2]}";

                if ((tlds.Length < 2) || (tlds.Length > 3))
                    continue;
                if ((tldKey == "") || string.IsNullOrWhiteSpace(item.Item2))
                    continue;

                if (approxCountryBuckets.ContainsKey(tldKey))
                    approxCountryBuckets[tldKey].Add(item);
                else
                {
                    approxCountryBuckets.Add(tldKey, new List<Tuple<string, string>>());
                    approxCountryBuckets[tldKey].Add(item);
                }
            }

            List<string> tldKeys = approxCountryBuckets.Keys.ToList();
            tldKeys.Sort();

            var approxCountryBucketsSorted = new Dictionary<string, List<Tuple<string, string>>>();

            foreach (var item in tldKeys)
                approxCountryBucketsSorted.Add(item, approxCountryBuckets[item]);

            return approxCountryBucketsSorted;

        }
    }
}
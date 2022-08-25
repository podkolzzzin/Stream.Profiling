using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stream.Profiling;

class Sorter2Tuned
{
    public void Sort(string fileName)
    {
        Dictionary<string, List<int>> strDictionary = new Dictionary<string, List<int>>();
        using (var fs = File.OpenText(fileName))
        {
            for (;;)
            {
                var str = fs.ReadLine();
                if (string.IsNullOrWhiteSpace(str)) break;
                int idx = str.IndexOf('.');
                var key = str[(idx+2)..];
                var n = int.Parse(str.AsSpan(0, idx));
                if (strDictionary.TryGetValue(key, out var list))
                {
                    list.Add(n);
                }
                else
                {
                    strDictionary.Add(key, new List<int>() { n });
                }
            }
        }

        using (var res2 = File.CreateText("result-2.txt"))
        {
            foreach (var pair in strDictionary.OrderBy(e => e.Key))
            {
                pair.Value.Sort();
                foreach (var i in pair.Value)
                {
                    res2.Write(i);
                    res2.Write(". ");
                    res2.WriteLine(pair.Key);
                }
            }
        }
    }
}
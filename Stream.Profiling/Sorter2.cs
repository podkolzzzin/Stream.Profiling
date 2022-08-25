using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stream.Profiling;

class Sorter2
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
                var arr = str.Split('.', 2);
                List<int> list;
                if (strDictionary.TryGetValue(arr[1], out list))
                {
                    list.Add(int.Parse(arr[0]));
                }
                else
                {
                    strDictionary.Add(arr[1], new List<int>() { int.Parse(arr[0]) });
                }
            }
        }

        using (var res2 = File.CreateText("result-2.txt"))
        {
            foreach (var pair in strDictionary.OrderBy(e => e.Key, StringComparer.Ordinal))
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
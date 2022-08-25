using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Stream.Profiling;

class Sorter3
{
    public void Sort(string fileName)
    {
        var strDictionary = new Dictionary<string, Dictionary<short, int>>();
        using (var fs = File.OpenText(fileName))
        {
            for (;;)
            {
                var str = fs.ReadLine();
                if (string.IsNullOrWhiteSpace(str)) break;
                var arr = str.Split('.', 2);
                var n = short.Parse(arr[0]);
                if (!strDictionary.TryGetValue(arr[1], out var list))
                {
                    strDictionary.Add(arr[1], new () { [n] = 1 });
                }
                else
                {
                    if (!list.ContainsKey(n))
                        list.Add(n, 0);
                    list[n]++;
                }
            }
        }

        using (var res2 = File.CreateText("result-3.txt"))
        {
            foreach (var pair in strDictionary.OrderBy(x => x.Key))
            {
                foreach (var nVals in pair.Value.OrderBy(x => x.Key))
                {
                    for (int i = 0; i < nVals.Value; i++)
                    {
                        res2.Write(nVals.Value);
                        res2.Write(". ");
                        res2.WriteLine(pair.Key);
                    }
                }
            }
        }
    }
}
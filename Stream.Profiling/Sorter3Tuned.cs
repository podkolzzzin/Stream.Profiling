using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stream.Profiling;

class Sorter3Tuned
{
    public void Sort(string fileName)
    {
        var strDictionary = new Dictionary<ReadOnlyMemory<char>, Dictionary<short, int>>();
        using (var fs = File.OpenText(fileName))
        {
            for (;;)
            {
                var str = fs.ReadLine();
                if (string.IsNullOrWhiteSpace(str)) break;
                var pos = str.IndexOf('.');
                var n = short.Parse(str.AsSpan(0, pos));
                var mem = str.AsMemory(pos + 2);
                if (!strDictionary.TryGetValue(mem, out var list))
                {
                    strDictionary.Add(mem, new () { [n] = 1 });
                }
                else
                {
                    if (!list.TryAdd(n, 1))
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
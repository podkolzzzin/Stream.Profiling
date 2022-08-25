using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stream.Profiling;

class Sorter
{
    public async Task Sort(string fileName, int partLinesCount)
    {
        var files = await SplitFile(fileName, partLinesCount);
        //SortParts(files, partLinesCount);
        await SortResult(files);
    }

    private class LineState
    {
        public StreamReader Reader { get; set; }
        public Line Line { get; set; }
    }

    private async Task SortResult(string[] files)
    {
        var readers = files.Select(x => new StreamReader(x)).ToArray();
        try
        {
            var lines = readers.Select(x => new LineState
            {
                Line = new Line(x.ReadLine()),
                Reader = x
            }).OrderBy(x => x.Line).ToList();

            using var writer = new StreamWriter("result.txt");
            while (lines.Count > 0)
            {
                var current = lines[0];
                WriteLine(writer, current.Line);

                if (current.Reader.EndOfStream)
                {
                    lines.Remove(current);
                    continue;
                }
                current.Line = new Line(current.Reader.ReadLine());
                Reorder(lines);
            }
        }
        finally
        {
            foreach (var r in readers)
                r.Dispose();
        }
    }

    private void Reorder(List<LineState> lines)
    {
        if (lines.Count == 1)
            return;

        int i = 0;
        while (lines[i].Line.CompareTo(lines[i + 1].Line) > 0)
        {
            var t = lines[i];
            lines[i] = lines[i + 1];
            lines[i + 1] = t;
            i++;
            if (i + 1 == lines.Count)
                return;
        }
    }
        
    private async Task<string[]> SplitFile(string fileName, int partLinesCount)
    {
        var list = new List<string>();
        int partNumber = 0;
        Line[] lines = new Line[partLinesCount];
        int i = 0;

        using var reader = new StreamReader(fileName);
        for (string line = reader.ReadLine();;line = reader.ReadLine())
        {
            lines[i] = new Line(line);
            i++;
            if (i == partLinesCount)
            {
                partNumber++;
                var partFileName = partNumber + ".txt";
                list.Add(partFileName);
                Array.Sort(lines);
                WriteAllLines(partFileName, lines);
                i = 0;
            }
            if (reader.EndOfStream)
                break;
        }

        if (i != 0)
        {
            Array.Resize(ref lines, i + 1);
            partNumber++;
            var partFileName = partNumber + ".txt";
            list.Add(partFileName);
            WriteAllLines(partFileName, lines);
        }

        return list.ToArray();
    }

    private void WriteAllLines(string fileName, Line[] lines)
    {
        using var writer = new StreamWriter(fileName);
        foreach (var line in lines)
        {
            WriteLine(writer, line);
        }
    }

    private static void WriteLine(StreamWriter writer, Line line)
    {
        writer.Write(line.Number);
        writer.Write(".");
        writer.WriteLine(line.Word);
    }
}
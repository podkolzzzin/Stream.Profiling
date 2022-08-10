using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stream.Profiling
{
    struct Line : IComparable<Line>
    {
        private int pos;
        private string line;
        public Line(string line)
        {
            pos = line.IndexOf(".");
            Number = int.Parse(line.AsSpan(0, pos));
            this.line = line;
        }

        public string Build() => line;

        public int Number { get; set; }
        public ReadOnlySpan<char> Word => line.AsSpan(pos + 2);

        public int CompareTo(Line other)
        {
            int result = Word.CompareTo(other.Word, StringComparison.Ordinal);
            if (result != 0)
                return result;
            return Number.CompareTo(other.Number);
        }
    }


    class Sorter
    {
        public async Task Sort(string fileName, int partLinesCount)
        {
            var files = await SplitFile(fileName, partLinesCount);
            //SortParts(files, partLinesCount);
            await SortResult(files);
        }

        private void SortParts(string[] files, int partLinesCount)
        {
            Line[] lines = new Line[partLinesCount];
            foreach(var file in files)
            {
                var strings = File.ReadAllLines(file);
                for (int i = 0; i < strings.Length; i++)
                {
                    lines[i] = new Line(strings[i]);
                }

                Array.Sort(lines, 0, strings.Length);
                File.WriteAllLines(file, lines.Select(x => x.Build()));
            }
        }

        private class LineState
        {
            public StreamReader Reader { get; set; }
            public Line Line { get; set; }
        }

        private async Task SortResult(string[] files)
        {
            var readers = files.Select(x => new StreamReader(x));
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
                    writer.WriteLine(current.Line.Build());

                    if (current.Reader.EndOfStream)
                    {
                        lines.Remove(current);
                        continue;
                    }
                    current.Line = new Line(await current.Reader.ReadLineAsync());
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

        private IEnumerable<Line[]> Batch(IEnumerable<string> lines, int partLinesCount)
        {
            Line[] l = new Line[partLinesCount];
            int i = 0;
            foreach(var line in lines)
            {
                l[i] = new Line(line);
                i++;
                if (i == partLinesCount)
                {
                    yield return l;
                    i = 0;
                }
            }

            if (i != 0)
            {
                Array.Resize(ref l, i);
                yield return l;
            }
        }

        private async Task<string[]> SplitFile(string fileName, int partLinesCount)
        {
            var list = new List<string>();
            int partNumber = 0;
            Line[] lines = new Line[partLinesCount];
            int i = 0;

            using var reader = new StreamReader(fileName);
            for (string line = await reader.ReadLineAsync();
                !reader.EndOfStream; 
                line = await reader.ReadLineAsync())
            {
                lines[i] = new Line(line);
                i++;
                if (i == partLinesCount)
                {
                    partNumber++;
                    var partFileName = partNumber + ".txt";
                    list.Add(partFileName);
                    Array.Sort(lines);
                    await File.WriteAllLinesAsync(partFileName, lines.Select(x => x.Build()));
                    i = 0;
                }
            }

            if (i == 0)
            {
                Array.Resize(ref lines, i);
                var partFileName = partNumber + ".txt";
                list.Add(partFileName);
                await File.WriteAllLinesAsync(partFileName, lines.Select(x => x.Build()));
            }

            return list.ToArray();
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            var fileName = new Generator().Generate(2_000_000);
            await new Sorter().Sort(fileName, 200_000);
            sw.Stop();
            Console.WriteLine($"Execution took: {sw.Elapsed}");
        }
    }
}

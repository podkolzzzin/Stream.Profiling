using System;
using System.IO;
using System.Linq;

namespace Stream.Profiling
{
    internal sealed class Generator
    {
        private readonly Random random = new Random();
        private readonly string[] words;

        public Generator()
        {
            words = Enumerable.Range(0, 10000)
                .Select(
                    x =>
                    {
                        var range = Enumerable.Range(0, random.Next(20, 100));
                        var chars = range.Select(x => (char)random.Next('A', 'Z')).ToArray();
                        var str = new string(chars);
                        return str;
                    }).ToArray();
        }

        public string Generate(int linesCount)
        {
            var fileName = "L" + linesCount + ".txt";
            using (var writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < linesCount; i++)
                {
                    writer.WriteLine(GenerateNumber() + ". " + GenerateString());
                }
            }
            return fileName;
        }

        private string GenerateString() => words[random.Next(0, words.Length)];

        private string GenerateNumber() => random.Next(0, 10000).ToString();
    }
}

using System;

namespace Stream.Profiling;

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
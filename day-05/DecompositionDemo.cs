public record Range(long Start, long Length)
{
    public Range? IntersectWith(Range range)
    {        
        var startIntersect = Math.Max(Start, range.Start);
        var endIntersect = Math.Min(End, range.End);
        return startIntersect < endIntersect 
            ? new Range(startIntersect, endIntersect - startIntersect)
            : null;    
    }
    
    /// <summary>Exclusive right end of the range</summary>
    public long End => Start + Length;

    public Range AddOffset(long offset) => this with { Start = Start + offset };

    public IEnumerable<Range> ExcludeAll(IEnumerable<Range> rangesToExclude)
    {
        var current = this;
        foreach (var rangeToExclude in rangesToExclude.OrderBy(r => r.Start))
        {
            var (prefix, suffix) = current.Exclude(rangeToExclude);
            if (prefix is not null) yield return prefix;
            if (suffix is null) yield break;
            current = suffix;
        }
        yield return current;
    }
    
    private (Range? left, Range? right) Exclude(Range rangeToExclude)
    {
        Range? left = null;
        Range? right = null;
        if (Start < rangeToExclude.Start)
            left = FromStartEnd(Start, Math.Min(rangeToExclude.Start, End));
        if (End > rangeToExclude.End)
            right = FromStartEnd(Math.Max(rangeToExclude.End, Start), End);
        return (left, right);
    }
    
    private static Range FromStartEnd(long start, long endExclusive) => new(start, endExclusive - start); 
}

public record Mapping(Range Source, long Offset);

public record Transform(Mapping[] Mappings)
{
    public Range[] ApplyTo(Range range)
    {
        // Отобразить все пересечения Mappings с range
        var mapped = Mappings
            .Select(m => m.Source.IntersectWith(range)?.AddOffset(m.Offset))
            .OfType<Range>(); 
        
        // Все части range, не пересекающиеся с Mappings оставить как есть.
        var mappingSourceRanges = Mappings.Select(m => m.Source);
        var unchanged = range.ExcludeAll(mappingSourceRanges);
        return mapped.Concat(unchanged).ToArray();
    }
    
    public Range[] ApplyTo(Range[] ranges)
    {
        return ranges.SelectMany(ApplyTo).ToArray(); 
    }
}

public class Day5Part2
{
    public static long GetLowestTransformedNumber(Range[] ranges, Transform[] transforms)
    {
        // Последовательно применить все трансформации к каждому диапазону
        // Найти минимальное число во всех полученных диапазонах
        return transforms.Aggregate(
                ranges, 
                (currentRanges, transform) => transform.ApplyTo(currentRanges))
            .Min(range => range.Start);
    }
    
    #region Parsing 
    public static void Main()
    {
        var lines = File.ReadAllLines("05.txt");
        var ranges = ParseInputRanges(lines[0]);
        var transforms = ParseTransforms(lines.Skip(2));
        var answer = GetLowestTransformedNumber(ranges, transforms);
        Console.WriteLine($"Min output number: {answer}");
    }

    private static Range[] ParseInputRanges(string line) =>
        line.Split(" ").Skip(1).Select(long.Parse)
            .Chunk(2)
            .Select(pair => new Range(pair[0], pair[1]))
            .ToArray();

    private static Transform[] ParseTransforms(IEnumerable<string> lines) =>
        lines.SplitBy(string.Empty)
            .Select(ls => new Transform(ls.Skip(1).Select(ParseMapping).ToArray()))
            .ToArray();

    private static Mapping ParseMapping(string line)
    {
        var parts = line.Split(" ").Select(long.Parse).ToArray();
        var offset = parts[0] - parts[1];
        return new Mapping(new Range(parts[1], parts[2]), offset);
    }
    #endregion
}
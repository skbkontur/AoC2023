public static class ParsingExtensions
{
    public static IEnumerable<List<string>> SplitBy(this IEnumerable<string> lines, string delimiter)
    {
        var block = new List<string>();
        foreach (var line in lines)
        {
            if (line == delimiter)
            {
                yield return block;
                block = new List<string>();
            }
            else
                block.Add(line);
        }
        if (block.Any()) yield return block;
    }    
}

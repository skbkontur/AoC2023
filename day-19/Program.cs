using System.Diagnostics;

namespace aoc;

public static class Program
{
    public static long SolvePart1(IPartChecker partChecker, long[][] parts)
    {
        return parts
            .Where(partChecker.IsAccepted)
            .Sum(part => part.Sum());
    }

    public static void Main()
    {
        var (rules, parts) = LoadFromFile("day19.txt");

        var partChecker = new PartChecker(rules);
        var partChecker_ILGenerated = new PartChecker_ILGenerated(rules);
        
        if (SolvePart1(partChecker, parts) != 575412)
             throw new Exception("Bad code");
        
        if (SolvePart1(partChecker_ILGenerated, parts) != 575412)
             throw new Exception("Bad code");
        
        var t = Stopwatch.GetTimestamp();
        SolvePart1(partChecker, parts);
        Console.WriteLine(Stopwatch.GetElapsedTime(t));
        
        var t2 = Stopwatch.GetTimestamp();
        SolvePart1(partChecker_ILGenerated, parts);
        Console.WriteLine(Stopwatch.GetElapsedTime(t2));
    }

    # region Parsing
    
    private static (Rule[] rules, long[][] parts) LoadFromFile(string fileName)
    {
        var input = File.ReadAllText(fileName).Split("\n\n");
        var rulesInput = input[0].Split("\n", StringSplitOptions.RemoveEmptyEntries);
        var partsInput = input[1].Split("\n", StringSplitOptions.RemoveEmptyEntries);

        var rules = rulesInput
            .Select(r => r.Split("{},".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            .Select(r => (name: r[0], branches: r.Skip(1).SkipLast(1).Select(b => b.Split(':')).ToArray(), otherwise: r[^1]))
            .Select(r => new Rule(
                r.name,
                r.branches.Select(b => new Branch("xmas".IndexOf(b[0][0]), b[0][1], long.Parse(b[0][2..]), b[1])).ToArray(), r.otherwise))
            .ToArray();

        var parts = partsInput
            .Select(p => p
                .Split("{}=xmas,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse)
                .ToArray()
            )
            .ToArray();

        return (rules, parts);
    }
    
    # endregion
}
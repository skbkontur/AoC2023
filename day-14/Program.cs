using System.Diagnostics;

namespace aoc;

public static class Program
{
    public static void Main()
    {
        // var input = File.ReadAllLines("day14-sample.txt");
        var input = File.ReadAllLines("day14.txt");
    
        if (Day14.SolvePart1(input) != 110821)
            throw new Exception("Bad code");

        if (Day14.SolvePart2(input) != 83516)
            throw new Exception("Bad code");
    
        // Optimizations history:
        // 00:00:00.2255773
        // 00:00:00.1857285
        // 00:00:00.1672642
        // 00:00:00.1648000
        var t = Stopwatch.GetTimestamp();
        Day14.SolvePart2(input);
        Console.WriteLine(Stopwatch.GetElapsedTime(t));
    }
}
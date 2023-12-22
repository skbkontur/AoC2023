namespace aoc;

public static class Day14
{
    public static long SolvePart1(string[] input)
    {
        input = FallN(input);
        return Load(input);           
    }

    public static long SolvePart2(string[] input)
    {
        const long count = 1000000000;
        var hashToCycle = new Dictionary<int, int>();
        var cycleToLoad = new Dictionary<int, long>();
        for (int i = 0; i < count; i++)
        {
            var hash = Hash(input);
            if (hashToCycle.TryGetValue(hash, out var prev))
            {
                var patternLength = i - prev;
                var left = (count - i) % patternLength;
                var resultAt = prev + left;
                return cycleToLoad[(int)resultAt];
            }
            hashToCycle.Add(hash, i);
            cycleToLoad.Add(i, Load(input));
            input = Cycle(input);
        }

        return Load(input);
    }

    static long Load(string[] input)
    {
        return input.Select((s, i) => s.Count(c => c == 'O') * (input.Length - i)).Sum();
    }

    static int Hash(string[] input)
    {
        return input.Aggregate(0, HashCode.Combine);
    }

    static string[] Cycle(string[] input)
    {
        input = RotateCW(input);
        input = FallRight(input);
        input = RotateCW(input);
        input = FallRight(input);
        input = RotateCW(input);
        input = FallRight(input);
        input = RotateCW(input);
        input = FallRight(input);
        return input;
    }

    static string[] FallN(string[] input)
    {
        input = RotateCCW(input);
        input = FallLeft(input);
        input = RotateCW(input);
        return input;
    }

    static string[] FallS(string[] input)
    {
        input = RotateCW(input);
        input = FallLeft(input);
        input = RotateCCW(input);
        return input;
    }

    static string[] FallE(string[] input)
    {
        input = FallRight(input);
        return input;
    }

    static string[] FallW(string[] input)
    {
        input = FallLeft(input);
        return input;
    }

    static string[] FallLeft(string[] input)
    {
        return input.Select(FallLineLeft).ToArray();
    }

    static string FallLineLeft(string line)
    {
        return string.Join('#', line.Split('#').Select(FallPartLeft));
    }
    
    static string FallPartLeft(string line)
    {
        return new string('O', line.Count(c => c == 'O')) + new string('.', line.Count(c => c == '.'));
    }
    
    static string[] FallRight(string[] input)
    {
        return input.Select(FallLineRight).ToArray();
    }

    static string FallLineRight(string line)
    {
        return string.Join('#', line.Split('#').Select(FallPartRight));
    }

    static string FallPartRight(string line)
    {
        return new string('.', line.Count(c => c == '.')) + new string('O', line.Count(c => c == 'O'));
    }

    static string[] RotateCCW(string[] input)
    {
        /*
         * +-------S-+
         * E<------+ |
         * |         |
         * |         |
         * +---------+
         */
        return Transpose(input).Reverse().ToArray();
    }

    static string[] RotateCW(string[] input)
    {
        /*
         * +-------E-+
         * S-------^ |
         * |         |
         * |         |
         * +---------+
         */
        return Transpose(input.Reverse().ToArray());
    }

    static string[] Transpose(string[] input)
    {
        return Enumerable
            .Range(0, input[0].Length)
            .Select(col => new string(input.Select(line => line[col]).ToArray()))
            .ToArray();
    }

    static void Dump(string[] input)
    {
        foreach (var line in input)
            Console.WriteLine(line);
    }
}
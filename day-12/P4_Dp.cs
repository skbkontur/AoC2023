using System.Diagnostics;
using System.Numerics;

public class Day12Part1_DpDemo
{
    public record PatternLine(string Pattern, int[] Blocks)
    {
        public BigInteger GetArrangementsCount()
        {
            var maxBlockLen = new int[Pattern.Length+1];
            for (var i = 0; i < Pattern.Length; i++)
                if (Pattern[i] is not '.')
                    maxBlockLen[i+1] = maxBlockLen[i] + 1;

            var counts = new BigInteger[Pattern.Length+1, Blocks.Length+1];
            // counts[nChars, nBlocks] - количество способов расставить первые nBlocks блоков на левых nChars клетках
            counts[0, 0] = BigInteger.One;
            // counts[0, 1..] = 0
            for (int nChars = 1; nChars <= Pattern.Length; nChars++)
            for (int nBlocks = 0; nBlocks <= Blocks.Length; nBlocks++)
            {
                var count = BigInteger.Zero;
                // Оставляем клетку пустой:
                if (Pattern[nChars-1] is not '#') 
                    count += counts[nChars-1, nBlocks];
                if (nBlocks > 0)
                {
                    var len = Blocks[nBlocks - 1];
                    if (FitBlockAt(nChars - len, len)) // На клетке можем закончить блок
                    {
                        var gap = nChars == len ? 0 : 1; 
                        count += counts[nChars - len - gap, nBlocks - 1];
                    }
                }
                counts[nChars, nBlocks] = count;
            }
            return counts[Pattern.Length, Blocks.Length];

            bool FitBlockAt(int start, int len)
            {
                if (start < 0) return false;
                if (start+len > Pattern.Length) return false;
                return 
                    //Pattern[start..(start+len)].All(c => c is not '.')  // no holes in block
                    maxBlockLen[start+len] >= len
                    &&  (start == 0 || Pattern[start - 1] is not '#'); // no # just before
            }
        }

        public PatternLine Unfold(int n)
        {
            var unfoldedPattern = string.Join("?", Enumerable.Repeat(Pattern, n));
            var unfoldedBlocks = Enumerable.Repeat(Blocks, n).SelectMany(x => x).ToArray();
            return new PatternLine(unfoldedPattern, unfoldedBlocks);
        }
    }

    public static void Solve()
    {
        Console.WriteLine(nameof(Day12Part1_DpDemo));
        var lines = File.ReadAllLines("12.txt")
            .Select(line => line.Split(' '))
            .Select(line => new PatternLine(line[0], line[1].Split(',').Select(int.Parse).ToArray()))
            .Select(line => line.Unfold(10))
            .ToList();
        var sw = Stopwatch.StartNew();
        var sum = lines.Aggregate(BigInteger.Zero, (current, line) => current + line.GetArrangementsCount());
        Console.WriteLine(sum);
        Console.WriteLine(sw.Elapsed);
    }
}
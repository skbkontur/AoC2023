using System.Diagnostics;
using System.Numerics;

public class Day12Part1_MemoizationDemo
{
    public record PatternLine(string Pattern, int[] Blocks)
    {
        public BigInteger GetArrangementsCount()
        {
            var cache = new Dictionary<(int nChars, int nBlocks), BigInteger>();
            return ArrangementsCount(Pattern.Length, Blocks.Length);

            BigInteger ArrangementsCount(int nChars, int nBlocks)
            {
                if (cache.TryGetValue((nChars, nBlocks), out var cached)) return cached;
                if (nChars <= 0) return nBlocks == 0 ? 1 : 0;
                var count = BigInteger.Zero;

                // Оставляем клетку пустой:
                if (Pattern[nChars-1] is not '#') 
                    count += ArrangementsCount(nChars-1, nBlocks);
                if (nBlocks > 0)
                {
                    // На клетке можем закончить блок: 
                    var len = Blocks[nBlocks - 1];
                    if (FitBlockAt(nChars - len, len))
                    {
                        var gap = nChars == len ? 0 : 1;   
                        count += ArrangementsCount(nChars - len - gap, nBlocks - 1);
                    }
                }
                cache[(nChars, nBlocks)] = count;
                return count;
            }

            bool FitBlockAt(int start, int len)
            {
                if (start < 0) return false;
                if (start+len > Pattern.Length) return false;
                return 
                    Pattern[start..(start+len)].All(c => c is not '.')  // no holes in block
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
        Console.WriteLine("Memoization:");
        
        var lines = File.ReadAllLines("12.txt")
            .Select(line => line.Split(' '))
            .Select(line => new PatternLine(line[0], line[1].Split(',').Select(int.Parse).ToArray()))
            .Select(line => line.Unfold(10))
            .ToList();
        var sw = Stopwatch.StartNew();
        var ans = lines.Aggregate(BigInteger.Zero, (sum, line) => sum + line.GetArrangementsCount());
        Console.WriteLine(ans);
        Console.WriteLine(sw.Elapsed);
    }
}
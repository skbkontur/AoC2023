using System.Diagnostics;
using System.Numerics;
using Memoizer;

public class Day12Part1_RecursionDemo
{
    public record PatternLine(string Pattern, int[] Blocks)
    {
        public BigInteger GetArrangementsCount()
        {
            return ArrangementsCount(Pattern.Length, Blocks.Length);

            // Количество способов расставить первые nBlocks блоков на левых nChars клетках 
            [Cache]
            BigInteger ArrangementsCount(int nChars, int nBlocks)
            {
                // Если паттерн пустой
                if (nChars == 0)
                    return nBlocks == 0 ? 1 : 0;
                
                var count = BigInteger.Zero;
                // Оставляем последнюю клетку пустой, если это возможно:
                if (Pattern[nChars-1] is not '#') 
                    count += ArrangementsCount(nChars-1, nBlocks);
                if (nBlocks > 0)
                {
                    // На последней клетке можем закончить блок
                    //
                    //  0                                           nChars-1
                    //  ↓                                              ↓
                    // [(предыдущие nBlocks-1 блоков) . (последний блок)]
                    // или
                    // [(последний блок занял весь паттерн)]
                    var len = Blocks[nBlocks - 1];
                    if (FitBlockAt(nChars - len, len))
                    {
                        var gap = nChars == len ? 0 : 1;
                        count += ArrangementsCount(nChars - len - gap, nBlocks - 1);
                    }
                }
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
        var lines = File.ReadAllLines("12.txt")
            .Select(line => line.Split(' '))
            .Select(line => new PatternLine(line[0], line[1].Split(',').Select(int.Parse).ToArray()))
            .Select(line => line.Unfold(10))
            .ToList();
        var sw = Stopwatch.StartNew();
        var ans = lines.Aggregate(BigInteger.Zero, (sum, line) => sum + line.GetArrangementsCount());
        Console.WriteLine($"{ans} in {sw.Elapsed}");
    }
}
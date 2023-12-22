namespace aoc;

public record Branch(int Field, char Op, long Value, string Next)
{
    public string? Try(long[] part)
    {
        if (Op == '<' && part[Field] < Value)
            return Next;
        if (Op == '>' && part[Field] > Value)
            return Next;
        return null;
    }
}

public record Rule(string Name, Branch[] Branches, string Otherwise)
{
    public string Next(long[] part)
    {
        return Branches.Select(b => b.Try(part)).FirstOrDefault(r => r != null) ?? Otherwise;
    }
}

public interface IPartChecker
{
    bool IsAccepted(long[] part);
}

public class PartChecker: IPartChecker
{
    private readonly Dictionary<string, Rule> rules;

    public PartChecker(Rule[] rules)
    {
        this.rules = rules.ToDictionary(x => x.Name);
    }

    public bool IsAccepted(long[] part) => IsAccepted(part, "in");
        
    private bool IsAccepted(long[] part, string rule)
    {
        return rule switch
        {
            "A" => true,
            "R" => false,
            _ => IsAccepted(
                part,
                rules[rule].Next(part)
            )
        };
    }
}
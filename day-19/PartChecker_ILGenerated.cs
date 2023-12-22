using System.Reflection.Emit;

namespace aoc;

public class PartChecker_ILGenerated : IPartChecker
{
    private readonly Func<long[], bool> isAccepted;
    
    public PartChecker_ILGenerated(Rule[] rules)
    {
        var method = new DynamicMethod(
            Guid.NewGuid().ToString(),
            typeof(bool),
            [typeof(long[])],
            typeof(PartChecker_ILGenerated).Module,
            true);

        var il = method.GetILGenerator();
        var labels = rules.ToDictionary(x => x.Name, _ => il.DefineLabel());
        labels.Add("A", il.DefineLabel());
        labels.Add("R", il.DefineLabel());
        
        il.Emit(OpCodes.Br, labels["in"]);

        foreach (var (name, branches, otherwise) in rules)
        {
            il.MarkLabel(labels[name]);
            foreach (var (field, op, value, next) in branches)
            {
                // stack: []
                il.Emit(OpCodes.Ldarg_0); // [part]
                il.Emit(OpCodes.Ldc_I4, field); // [part, field]
                il.Emit(OpCodes.Ldelem_I8); // [part[field]]
                il.Emit(OpCodes.Ldc_I8, value); // [part[field], value]
                if (op == '<')
                    il.Emit(OpCodes.Blt, labels[next]); // if (part[field] < value) goto next; []
                else // if (op == '>')
                    il.Emit(OpCodes.Bgt, labels[next]); // if (part[field] > value) goto next; []
            }
            il.Emit(OpCodes.Br, labels[otherwise]);
        }
        
        // stack: []
        il.MarkLabel(labels["A"]);
        il.Emit(OpCodes.Ldc_I4_1); // [1 === true]
        il.Emit(OpCodes.Ret);
        
        // stack: []
        il.MarkLabel(labels["R"]);
        il.Emit(OpCodes.Ldc_I4_0); // [0 === false]
        il.Emit(OpCodes.Ret);

        isAccepted = method.CreateDelegate<Func<long[], bool>>();
    }

    public bool IsAccepted(long[] part) => isAccepted(part);
}
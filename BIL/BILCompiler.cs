namespace Batch2OS.BIL;

public static class BILCompiler
{
    public static List<BILInstruction> Compile(IEnumerable<string> lines)
    {
        var list = new List<BILInstruction> { new(BILOpCodes.Initialize) };

        foreach (var line in lines)
        {
            var args = line.Split(new[] { ' ' }, 2);

            switch (args[0])
            {
                case "echo":
                    list.Add(new BILInstruction(BILOpCodes.Write32, args[1].ToCharArray().Select(x => (byte)x).ToArray()));
                    break;
            }
        }

        list.Add(new BILInstruction(BILOpCodes.Jump));
        return list;
    }
}
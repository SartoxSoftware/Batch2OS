namespace Batch2OS.BIL;

public static class BILCompiler
{
    public static List<BILInstruction> Compile(IEnumerable<string> lines)
    {
        var list = new List<BILInstruction> { new(BILOpCodes.ClearScreen), new(BILOpCodes.Interrupt, 0x10) };

        foreach (var line in lines)
        {
            var args = line.Split(new[] { ' ' }, 2);
            var cmd = args[0];

            switch (cmd)
            {
                case "help":
                    list.Add(new BILInstruction(BILOpCodes.PrintScreen, "WIP!".ToCharArray().Select(x => (byte)x).ToArray()));
                    list.Add(new BILInstruction(BILOpCodes.Interrupt, 0x10));
                    break;
                
                case "cls":
                    list.Add(new BILInstruction(BILOpCodes.ClearScreen));
                    list.Add(new BILInstruction(BILOpCodes.Interrupt, 0x10));
                    break;

                case "echo":
                    list.Add(new BILInstruction(BILOpCodes.PrintScreen, args[1].ToCharArray().Select(x => (byte)x).ToArray()));
                    list.Add(new BILInstruction(BILOpCodes.Interrupt, 0x10));
                    break;

                case "pause":
                    list.Add(new BILInstruction(BILOpCodes.PrintScreen, "Press any key to continue...".ToCharArray().Select(x => (byte)x).ToArray()));
                    list.Add(new BILInstruction(BILOpCodes.SetAH, 0x00));
                    list.Add(new BILInstruction(BILOpCodes.Interrupt, 0x16));
                    break;

                case "ver":
                    list.Add(new BILInstruction(BILOpCodes.PrintScreen, $"Batch2OS v{Utils.Version}".ToCharArray().Select(x => (byte)x).ToArray()));
                    list.Add(new BILInstruction(BILOpCodes.Interrupt, 0x10));
                    break;

                case "REM":
                case "::":
                case "title":
                    break;

                default:
                    Console.WriteLine($"Skipping unsupported command: {cmd}");
                    break;
            }
        }

        list.Add(new BILInstruction(BILOpCodes.Jump));
        return list;
    }
}
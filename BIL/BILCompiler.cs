using System.Globalization;

namespace Batch2OS.BIL;

public static class BILCompiler
{
    public static List<BILInstruction> Compile(IEnumerable<string> lines)
    {
        var list = new List<BILInstruction> { new(BILOpCode.ClearScreen), new(BILOpCode.Interrupt, 0x10) };
        var echo = true;

        foreach (var line in lines)
        {
            var args = line.Split(new[] { ' ' }, 2);
            var cmd = args[0];

            if (echo && !cmd.StartsWith('@'))
            {
                list.Add(new BILInstruction(BILOpCode.PrintScreen, line.Select(x => (byte)x)));
                list.Add(new BILInstruction(BILOpCode.Interrupt, 0x10));
            }

            switch (cmd)
            {
                case "@echo":
                    echo = args[1] == "on";
                    break;
                
                case "help":
                    list.Add(new BILInstruction(BILOpCode.PrintScreen, "help - Shows all commands with a brief description.\ncls - Clears the screen.\necho - Echoes a message, or turns on echo for commands.\npause - Pauses the OS until a key is pressed.\nver - Shows the version of Batch2OS.".Select(x => (byte)x)));
                    list.Add(new BILInstruction(BILOpCode.Interrupt, 0x10));
                    break;

                case "cls":
                    list.Add(new BILInstruction(BILOpCode.ClearScreen));
                    list.Add(new BILInstruction(BILOpCode.Interrupt, 0x10));
                    break;

                case "echo":
                    list.Add(new BILInstruction(BILOpCode.PrintScreen, args[1].Select(x => (byte)x)));
                    list.Add(new BILInstruction(BILOpCode.Interrupt, 0x10));
                    break;

                case "color":
                    list.Add(new BILInstruction(BILOpCode.SetColor, byte.Parse(args[1], NumberStyles.HexNumber)));
                    break;

                case "pause":
                    list.Add(new BILInstruction(BILOpCode.PrintScreen, "Press any key to continue...".Select(x => (byte)x)));
                    list.Add(new BILInstruction(BILOpCode.SetAH, 0x00));
                    list.Add(new BILInstruction(BILOpCode.Interrupt, 0x16));
                    break;

                case "ver":
                    list.Add(new BILInstruction(BILOpCode.PrintScreen, $"Batch2OS v{Utils.Version}".Select(x => (byte)x)));
                    list.Add(new BILInstruction(BILOpCode.Interrupt, 0x10));
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

        list.Add(new BILInstruction(BILOpCode.Jump));
        return list;
    }
}
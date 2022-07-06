using System.Globalization;

namespace Batch2OS;

public class Settings
{
    public bool Verbose;
    public string InputFile, OutputFile;
    public ushort BaseAddress, LoadAddress;

    public Settings(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (!arg.StartsWith('-'))
            {
                InputFile = arg;
                continue;
            }

            var option = arg[1];
            switch (option)
            {
                case 'h': Console.WriteLine("-h: Shows all commands.\n-o: Name of the output binary.\n-b: Base address when loading the binary.\n-l: Base address when loading the sectors."); continue;
                case 'v': Verbose = true; continue;
                case 'o': OutputFile = args[++i]; continue;
                case 'b': BaseAddress = GetNumber(args[++i]); continue;
                case 'l': LoadAddress = GetNumber(args[++i]); continue;
                default: Console.WriteLine("Unrecognized option: " + option); continue;
            }
        }
    }

    private static ushort GetNumber(string arg)
    {
        return arg.StartsWith("0x") ? ushort.Parse(arg[2..], NumberStyles.HexNumber) : ushort.Parse(arg);
    }
}
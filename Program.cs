using Batch2OS;
using Batch2OS.BIL;
using Batch2OS.X86;

if (args.Length == 0)
{
    Console.WriteLine("Usage: Batch2OS <input> -o <output> -b <address> -l <address>\nPlease view help (-h) to know what these arguments mean.");
    Environment.Exit(1);
}

var settings = new Settings(args);
var lines = File.ReadAllLines(settings.InputFile);
var bil = BILCompiler.Compile(lines);

if (settings.Verbose)
    foreach (var inst in bil)
    {
        Console.Write(Enum.GetName(typeof(BILOpCode), inst.OpCode));
        foreach (var arg in inst.Operands)
            Console.Write(" " + arg);
        Console.WriteLine();
    }

// 0x7c00 0x1000
File.WriteAllBytes(settings.OutputFile, X86Emitter.Emit(bil, settings.BaseAddress, settings.LoadAddress));
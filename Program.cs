using Batch2OS.BIL;
using Batch2OS.X86;

var path = args[0];
var lines = File.ReadAllLines(path);

Console.WriteLine("Compiling into BIL...");
var bil = BILCompiler.Compile(lines);

foreach (var inst in bil)
{
    Console.Write(Enum.GetName(typeof(BILOpCode), inst.OpCode));
    foreach (var arg in inst.Operands)
        Console.Write(" " + arg);
    Console.WriteLine();
}

Console.WriteLine("Compiling into x86 native code...");
File.WriteAllBytes(Path.ChangeExtension(path, "img"), X86Emitter.Emit(bil, 0x7c00, 0x1000));
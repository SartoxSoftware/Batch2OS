using System.Diagnostics;
using Batch2OS.BIL;
using Batch2OS.X86;

var lines = File.ReadAllLines("os.bat");

Console.WriteLine("Compiling into BIL...");
var bil = BILCompiler.Compile(lines);

foreach (var inst in bil)
{
    Console.Write(Enum.GetName(typeof(BILOpCodes), inst.OpCode));
    foreach (var arg in inst.Operands)
        Console.Write($" {arg}");
    Console.WriteLine();
}

Console.WriteLine("Compiling into x86 native code...");
var code = X86Assembler.Assemble(bil);

foreach (var b in code)
    Console.Write($"{b.ToString("X2")} ");
Console.WriteLine();

File.WriteAllBytes("kernel.bin", code);

Process.Start("qemu-system-x86_64", "kernel.bin");
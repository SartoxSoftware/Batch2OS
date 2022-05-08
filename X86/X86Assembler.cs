using Batch2OS.BIL;

namespace Batch2OS.X86;

public static class X86Assembler
{
    private static readonly List<byte> _bytes = new();

    public static byte[] Assemble(List<BILInstruction> list)
    {
        foreach (var inst in list)
            switch (inst.OpCode)
            {
                case BILOpCodes.Initialize:
                    // Clear screen
                    AddInstruction(X86OpCode.MOVMRAH, 0x00);
                    AddInstruction(X86OpCode.MOVMRAL, 0x13);
                    AddInstruction(X86OpCode.INT, 0x10);
 
                    // Set cursor position
                    AddInstruction(X86OpCode.MOVMRAH, 0x02); // Function code
                    AddInstruction(X86OpCode.MOVMRBH, 0x00); // Page number
                    AddInstruction(X86OpCode.MOVMRDH, 0x00); // Row
                    AddInstruction(X86OpCode.MOVMRDL, 0x00);
                    AddInstruction(X86OpCode.INT, 0x10);
                    break;

                case BILOpCodes.Write32:
                    PrintString(inst.Operands);
                    break;

                case BILOpCodes.Jump:
                    // 0xFE is the last instruction
                    AddInstruction(X86OpCode.JMP, inst.Operands.Length == 0 ? (byte)0xFE : inst.Operands[0]);
                    break;

                default:
                    Console.WriteLine($"Skipping unknown BIL instruction: {Enum.GetName(typeof(BILOpCodes), inst.OpCode)}");
                    break;
            }

        // Pad out with zeroes
        for (var i = _bytes.Count; i < 510; i++)
            _bytes.Add(0x00);

        // Make the binary bootable under real mode
        _bytes.Add(0x55);
        _bytes.Add(0xAA);

        return _bytes.ToArray();
    }

    private static void PrintString(IEnumerable<byte> operands)
    {
        // Print string
        foreach (var b in operands)
        {
            AddInstruction(X86OpCode.MOVMRAL, b);
            AddInstruction(X86OpCode.MOVMRAH, 0x0E); // Tell BIOS that we need to print one character on screen
            AddInstruction(X86OpCode.MOVMRBH, 0x00); // Page number
            AddInstruction(X86OpCode.MOVMRBL, 0x07); // Text attribute 0x07 is light gray font on black background
            AddInstruction(X86OpCode.INT, 0x10);
        }

        AddInstruction(X86OpCode.MOVMRAH, 0x02); // Function code
        AddInstruction(X86OpCode.MOVMRBH, 0x00); // Page number

        // New line (increase DH)
        _bytes.Add(0xFE);
        _bytes.Add(0xC6);

        AddInstruction(X86OpCode.MOVMRDL, 0x00); // Column
        AddInstruction(X86OpCode.INT, 0x10);
    }

    private static void AddInstruction(X86OpCode opcode, params byte[] operands)
    {
        _bytes.Add((byte)opcode);
        _bytes.AddRange(operands);
    }
}
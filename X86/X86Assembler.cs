using Batch2OS.BIL;

namespace Batch2OS.X86;

public static class X86Assembler
{
    private static readonly List<byte> Bytes = new();

    public static byte[] Assemble(List<BILInstruction> list)
    {
        foreach (var inst in list)
            switch (inst.OpCode)
            {
                case BILOpCodes.ClearScreen:
                    // Clear screen
                    AddInstruction(X86OpCode.MOVMRAH, 0x00);
                    AddInstruction(X86OpCode.MOVMRAL, 0x13);
                    AddInstruction(X86OpCode.INT, 0x10);
 
                    // Set cursor position
                    AddInstruction(X86OpCode.MOVMRAH, 0x02); // Function code
                    AddInstruction(X86OpCode.MOVMRBH, 0x00); // Page number
                    AddInstruction(X86OpCode.MOVMRDH, 0x00); // Row
                    AddInstruction(X86OpCode.MOVMRDL, 0x00);
                    break;

                case BILOpCodes.PrintScreen:
                    // Print string
                    foreach (var b in inst.Operands)
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
                    Bytes.Add(0xFE);
                    Bytes.Add(0xC6);

                    AddInstruction(X86OpCode.MOVMRDL, 0x00); // Column
                    break;

                case BILOpCodes.SetAH:
                    AddInstruction(X86OpCode.MOVMRAH, inst.Operands[0]);
                    break;

                case BILOpCodes.Interrupt:
                    AddInstruction(X86OpCode.INT, inst.Operands[0]);
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
        for (var i = Bytes.Count; i < 510; i++)
            Bytes.Add(0x00);

        // Make the binary bootable under real mode
        Bytes.Add(0x55);
        Bytes.Add(0xAA);

        return Bytes.ToArray();
    }

    private static void AddInstruction(X86OpCode opcode, params byte[] operands)
    {
        Bytes.Add((byte)opcode);
        Bytes.AddRange(operands);
    }
}
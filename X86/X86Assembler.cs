using Batch2OS.BIL;

namespace Batch2OS.X86;

public static class X86Assembler
{
    private static readonly List<byte> Bytes = new();
    private static readonly uint Address = 0x1000;
    private static readonly ushort OrgAddr = 0x7c00;
    private static readonly byte[] ZeroAddr = BitConverter.GetBytes((ushort)0x00);

    public static byte[] Assemble(List<BILInstruction> list)
    {
        // Initialization
        AddInstruction(X86OpCode.JMPF, BitConverter.GetBytes((ushort)(OrgAddr + 5)));
        Bytes.AddRange(ZeroAddr);
        
        // Reset segment registers
        AddInstruction(X86OpCode.XOR, (byte)X86OpCode.AXES);
        AddInstruction(X86OpCode.MOVSEG, (byte)X86OpCode.AXES);
        AddInstruction(X86OpCode.MOVSEG, (byte)X86OpCode.DS);
        AddInstruction(X86OpCode.MOVSEG, (byte)X86OpCode.SS);

        // Read sectors
        AddInstruction(X86OpCode.MOVMREBX, BitConverter.GetBytes((ushort)Address));
        AddInstruction(X86OpCode.MOVMRAH, 0x02);
        AddInstruction(X86OpCode.MOVMRAL, 0);
        var index = Bytes.Count - 1;
        AddInstruction(X86OpCode.MOVMRDL, 0x80);
        AddInstruction(X86OpCode.MOVMRCH, 0x00);
        AddInstruction(X86OpCode.MOVMRCL, 0x02);
        AddInstruction(X86OpCode.MOVMRDH, 0x00);
        AddInstruction(X86OpCode.INT, 0x13);
        AddInstruction(X86OpCode.ORAH, (byte)X86OpCode.AH);
        AddInstruction(X86OpCode.JNE, (byte)X86OpCode.LAST);

        // Jump to the next sector
        AddInstruction(X86OpCode.JMPF, BitConverter.GetBytes(Address));
        Bytes.AddRange(ZeroAddr);

        // Pad out with zeroes
        for (var i = Bytes.Count; i < 510; i++)
            Bytes.Add(0x00);

        // Make the binary bootable under real mode
        Bytes.Add(0x55);
        Bytes.Add(0xAA);
        
        foreach (var inst in list)
            switch (inst.OpCode)
            {
                case BILOpCodes.ClearScreen:
                    // Clear screen
                    AddInstruction(X86OpCode.MOVMRAH, 0x00);
                    AddInstruction(X86OpCode.MOVMRAL, 0x03);
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
                    AddInstruction(X86OpCode.JMP, inst.Operands.Length == 0 ? (byte)X86OpCode.LAST : inst.Operands[0]);
                    break;

                default:
                    Console.WriteLine($"Skipping unknown BIL instruction: {Enum.GetName(typeof(BILOpCodes), inst.OpCode)}");
                    break;
            }

        // Pad out with zeroes
        var sectors = GetSectors();
        Bytes[index] = (byte)((sectors - 1) / 512);
        for (var i = Bytes.Count; i < sectors; i++)
            Bytes.Add(0x00);

        return Bytes.ToArray();
    }

    private static void AddInstruction(X86OpCode opcode, params byte[] operands)
    {
        Bytes.Add((byte)opcode);
        Bytes.AddRange(operands);
    }

    private static int GetSectors()
    {
        var sectors = Bytes.Count;

        while (sectors % 512 != 0)
            sectors++;

        return sectors;
    }
}
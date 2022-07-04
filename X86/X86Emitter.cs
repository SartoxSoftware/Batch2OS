using Batch2OS.BIL;

namespace Batch2OS.X86;

public static class X86Emitter
{
    private static readonly List<byte> Bytes = new();
    private static readonly uint Address = 0x1000;
    private static readonly ushort OrgAddr = 0x7c00;

    public static byte[] Emit(List<BILInstruction> list)
    {
        byte color = 0x07; // Light gray on black by default
        var labels = new Dictionary<int, uint>();
        
        // Initialization
        AddInstruction(X86OpCode.JMPF, BitConverter.GetBytes((ushort)(OrgAddr + 5)));
        Bytes.Add(0x00); Bytes.Add(0x00);

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
        Bytes.Add(0x00); Bytes.Add(0x00);

        // Pad out with zeroes
        for (var i = Bytes.Count; i < 510; i++)
            Bytes.Add(0x00);

        // Make the binary bootable under real mode
        Bytes.Add(0x55);
        Bytes.Add(0xAA);

        foreach (var inst in list)
            switch (inst.OpCode)
            {
                case BILOpCode.ClearScreen:
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

                case BILOpCode.PrintScreen:
                    // Print string
                    foreach (var b in inst.Operands)
                    {
                        if (b == '\n')
                        {
                            AddInstruction(X86OpCode.INC, (byte)X86OpCode.DH); // New line
                            AddInstruction(X86OpCode.MOVMRAH, 0x02); // Function code
                            AddInstruction(X86OpCode.MOVMRDL, 0x00);
                            AddInstruction(X86OpCode.INT, 0x10);

                            continue;
                        }

                        AddInstruction(X86OpCode.MOVMRAL, b);
                        AddInstruction(X86OpCode.MOVMRAH, 0x09); // Tell BIOS that we need to print one character on screen
                        AddInstruction(X86OpCode.MOVMRBL, color); // Color attribute
                        AddInstruction(X86OpCode.MOVMRCX, 0x01, 0x00); // Print the character only one time
                        AddInstruction(X86OpCode.INT, 0x10);
                        
                        // Update cursor
                        AddInstruction(X86OpCode.MOVMRAH, 0x02); // Function code
                        AddInstruction(X86OpCode.INC, (byte)X86OpCode.DL); // Update cursor position
                        AddInstruction(X86OpCode.INT, 0x10);
                    }

                    AddInstruction(X86OpCode.MOVMRAH, 0x02); // Function code
                    AddInstruction(X86OpCode.INC, (byte)X86OpCode.DH); // New line
                    AddInstruction(X86OpCode.MOVMRDL, 0x00); // Column
                    break;

                case BILOpCode.SetColor:
                    color = inst.Operands.ElementAt(0);
                    break;

                case BILOpCode.Define:
                    labels.Add(0, (uint)Bytes.Count);
                    break;

                case BILOpCode.SetAH:
                    AddInstruction(X86OpCode.MOVMRAH, inst.Operands.ElementAt(0));
                    break;

                case BILOpCode.Interrupt:
                    AddInstruction(X86OpCode.INT, inst.Operands.ElementAt(0));
                    break;

                case BILOpCode.Jump:
                    if (!inst.Operands.Any())
                    {
                        AddInstruction(X86OpCode.JMP, (byte)X86OpCode.LAST);
                        break;
                    }

                    // TODO: Fix jump
                    // TODO: Jump to labels defined later

                    var exists = labels.TryGetValue(0, out var addr);
                    if (!exists)
                        throw new Exception($"Label '{new string(inst.Operands.Select(x => (char)x).ToArray())}' does not exist!");

                    // Perform a far, absolute indirect jump to a 32-bit address
                    Bytes.Add(0xFF); Bytes.Add(0x25);
                    Bytes.AddRange(BitConverter.GetBytes(addr));
                    break;

                default:
                    Console.WriteLine($"Skipping unknown BIL instruction: {Enum.GetName(typeof(BILOpCode), inst.OpCode)}");
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
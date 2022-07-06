using System.Text;
using Batch2OS.BIL;

namespace Batch2OS.X86;

public static class X86Emitter
{
    private static readonly List<byte> Bytes = new();

    // TODO: Get rid of this
    private const byte SPECIAL_OPERAND_LAST = 0xFE;

    public static byte[] Emit(List<BILInstruction> list, ushort baseAddr, ushort loadAddr)
    {
        byte color = 0x07; // Light gray on black by default
        var axes = EncodeModRM(X86Register.AL_EAX_ES, X86Register.AL_EAX_ES);
        var dh = EncodeModRM(X86Register.DH_ESI, X86Register.AL_EAX_ES);
        var dl = EncodeModRM(X86Register.DL_EDX_SS, X86Register.AL_EAX_ES);
        var load = BitConverter.GetBytes(loadAddr);

        // Initialization
        AddInstruction(X86OpCode.JMP_FAR, BitConverter.GetBytes((ushort)(baseAddr + 5)));
        Bytes.Add(0x00); Bytes.Add(0x00);

        // Reset segment registers
        AddInstruction(X86OpCode.XOR_RM32_R32, axes);
        AddInstruction(X86OpCode.MOV_RM16_SEG, axes);
        AddInstruction(X86OpCode.MOV_RM16_SEG, EncodeModRM(X86Register.BL_EBX_DS, X86Register.AL_EAX_ES));
        AddInstruction(X86OpCode.MOV_RM16_SEG, EncodeModRM(X86Register.DL_EDX_SS, X86Register.AL_EAX_ES));

        // Read sectors
        AddInstruction(X86OpCode.MOV_IMM32_EBX, load);
        AddInstruction(X86OpCode.MOV_IMM8_AH, 0x02);
        AddInstruction(X86OpCode.MOV_IMM8_AL, 0);
        var index = Bytes.Count - 1;
        AddInstruction(X86OpCode.MOV_IMM8_DL, 0x80);
        AddInstruction(X86OpCode.MOV_IMM8_CH, 0x00);
        AddInstruction(X86OpCode.MOV_IMM8_CL, 0x02);
        AddInstruction(X86OpCode.MOV_IMM8_DH, 0x00);
        AddInstruction(X86OpCode.INT, 0x13);

        // Check for errors
        AddInstruction(X86OpCode.OR_RM8_R8, EncodeModRM(X86Register.AH_ESP_FS, X86Register.AH_ESP_FS));
        AddInstruction(X86OpCode.JNZ, SPECIAL_OPERAND_LAST);

        // If none, jump to the next sector
        AddInstruction(X86OpCode.JMP_FAR, load);
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
                    AddInstruction(X86OpCode.MOV_IMM8_AH, 0x00);
                    AddInstruction(X86OpCode.MOV_IMM8_AL, 0x03);
                    AddInstruction(X86OpCode.INT, 0x10);
 
                    // Set cursor position
                    AddInstruction(X86OpCode.MOV_IMM8_AH, 0x02); // Function code
                    AddInstruction(X86OpCode.MOV_IMM8_BH, 0x00); // Page number
                    AddInstruction(X86OpCode.MOV_IMM8_DH, 0x00); // Row
                    AddInstruction(X86OpCode.MOV_IMM8_DL, 0x00);
                    break;

                case BILOpCode.PrintScreen:
                    // Print string
                    foreach (var b in inst.Operands)
                    {
                        if (b == '\n')
                        {
                            AddInstruction(X86OpCode.INC_RM8, dh); // New line
                            AddInstruction(X86OpCode.MOV_IMM8_AH, 0x02); // Function code
                            AddInstruction(X86OpCode.MOV_IMM8_DL, 0x00);
                            AddInstruction(X86OpCode.INT, 0x10);

                            continue;
                        }

                        AddInstruction(X86OpCode.MOV_IMM8_AL, b);
                        AddInstruction(X86OpCode.MOV_IMM8_AH, 0x09); // Tell BIOS that we need to print one character on screen
                        AddInstruction(X86OpCode.MOV_IMM8_BL, color); // Color attribute
                        AddInstruction(X86OpCode.MOV_IMM32_ECX, 0x01, 0x00); // Print the character only one time
                        AddInstruction(X86OpCode.INT, 0x10);
                        
                        // Update cursor
                        AddInstruction(X86OpCode.MOV_IMM8_AH, 0x02); // Function code
                        AddInstruction(X86OpCode.INC_RM8, dl); // Update cursor position
                        AddInstruction(X86OpCode.INT, 0x10);
                    }

                    AddInstruction(X86OpCode.MOV_IMM8_AH, 0x02); // Function code
                    AddInstruction(X86OpCode.INC_RM8, dh); // New line
                    AddInstruction(X86OpCode.MOV_IMM8_DL, 0x00); // Column
                    break;

                case BILOpCode.SetColor:
                    color = inst.Operands.ElementAt(0);
                    break;

                case BILOpCode.SetAH:
                    AddInstruction(X86OpCode.MOV_IMM8_AH, inst.Operands.ElementAt(0));
                    break;

                case BILOpCode.Interrupt:
                    AddInstruction(X86OpCode.INT, inst.Operands.ElementAt(0));
                    break;
                
                case BILOpCode.Jump:
                    AddInstruction(X86OpCode.JMP_SHORT, SPECIAL_OPERAND_LAST);
                    break;

                default:
                    Console.WriteLine("Skipping unknown BIL instruction: " + Enum.GetName(typeof(BILOpCode), inst.OpCode));
                    break;
            }

        // Add the number of sectors to read
        Bytes[index] = (byte)((GetSectors() - 1) / 512);
        return Bytes.ToArray();
    }

    private static byte EncodeModRM(X86Register dest, X86Register source)
    {
        var sb = new StringBuilder();
        var dstBin = Convert.ToString((byte)dest, 2).PadLeft(3, '0');
        var srcBin = Convert.ToString((byte)source, 2).PadLeft(3, '0');
        return Convert.ToByte(sb.Append("11").Append(srcBin).Append(dstBin).ToString(), 2);
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
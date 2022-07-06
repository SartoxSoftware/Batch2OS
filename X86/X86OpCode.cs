namespace Batch2OS.X86;

//https://www.felixcloutier.com/x86 <3333
public enum X86OpCode : byte
{
    MOV_OFF_AL = 0xA0,
    MOV_OFF_AX = 0xA1,
    MOV_AL_OFF = 0xA2,
    MOV_AX_OFF = 0xA3,
    MOV_IMM8_AL = 0xB0,
    MOV_IMM8_CL = 0xB1,
    MOV_IMM8_DL = 0xB2,
    MOV_IMM8_BL = 0xB3,
    MOV_IMM8_AH = 0xB4,
    MOV_IMM8_CH = 0xB5,
    MOV_IMM8_DH = 0xB6,
    MOV_IMM8_BH = 0xB7,
    MOV_IMM32_EAX = 0xB8,
    MOV_IMM32_ECX = 0xB9,
    MOV_IMM32_EDX = 0xBA,
    MOV_IMM32_EBX = 0xBB,
    MOV_IMM32_ESP = 0xBC,
    MOV_IMM32_EBP = 0xBD,
    MOV_IMM32_ESI = 0xBE,
    MOV_IMM32_EDI = 0xBF,
    MOV_R8_RM8 = 0x88,
    MOV_R32_RM32 = 0x89,
    MOV_RM8_R8 = 0x8A,
    MOV_RM32_R32 = 0x8B,
    MOV_SEG_RM16 = 0x8C,
    MOV_RM16_SEG = 0x8E,
    MOV_IMM8_RM8 = 0xC6,
    MOV_IMM32_RM32 = 0xC7,
    
    OR_AL_IMM8 = 0x0C,
    OR_EAX_IMM32 = 0x0D,
    OR_RM8_R8 = 0x08,
    
    JMP_SHORT = 0xEB,
    JMP_FAR = 0xEA,
    JNZ = 0x75,
    
    INT = 0xCD,

    INC_RM8 = 0xFE,
    INC_RM32 = 0xFF,
    
    XOR_AL_IMM8 = 0x34,
    XOR_EAX_IMM32 = 0x35,
    XOR_RM8_IMM8 = 0x80,
    XOR_RM32_IMM32 = 0x81,
    XOR_RM32_IMM8_SE = 0x83,
    XOR_RM8_R8 = 0x30,
    XOR_RM32_R32 = 0x31,
    XOR_R8_RM8 = 0x32,
    XOR_R32_RM32 = 0x33
}
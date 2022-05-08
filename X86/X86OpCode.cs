namespace Batch2OS.X86;

public enum X86OpCode : byte
{
    ADD,
    CLI = 0xFA,
    CLD = 0xFC,
    STI = 0xFB,
    MOVMREAX = 0xB8,
    MOVRMEAX = 0xA3,
    MOVSEG = 0x8E,
    MOVMRESP = 0xBC,
    MOVMRAL = 0xB0,
    MOVMRAH = 0xB4,
    MOVMRBH = 0xB7,
    MOVMRDH = 0xB6,
    MOVMRDL = 0xB2,
    MOVMRBL = 0xB3,
    MOVMREBX = 0xBB,
    JMPF = 0xEA,
    JMP = 0xEB,
    INT = 0xCD,
    INAL = 0xE4,
    OUTAL = 0xE6,
    ORAL = 0x0C,
    PUSHDS = 0x1E,
    POPDS = 0x1F,
    OREAX = 0x83,
    
    BIT16 = 0x66,
    
    EXPAND = 0x0F,
    ELGDT = 0x01,
    MOVCRREAX = 0x20,
    MOVRCREAX = 0x22,
    
    DS = 0xDE,
    ES = 0xC6,
    SS = 0xD6
}
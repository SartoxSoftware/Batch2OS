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
    MOVMRCH = 0xB5,
    MOVMRCL = 0xB1,
    MOVMRCX = 0xB9,
    MOVMREBX = 0xBB,
    MOVMREBXW = 0x8B,
    JMPF = 0xEA,
    JMP = 0xEB,
    JMP32 = 0xE9,
    JNE = 0x75,
    INT = 0xCD,
    INC = 0xFE,
    INAL = 0xE4,
    OUTAL = 0xE6,
    ORAL = 0x0C,
    PUSHDS = 0x1E,
    POPDS = 0x1F,
    OREAX = 0x83,
    ORAH = 0x08,
    XOR = 0x31,
    
    BIT16 = 0x66,
    
    EXPAND = 0x0F,
    ELGDT = 0x01,
    MOVCRREAX = 0x20,
    MOVRCREAX = 0x22,
    
    AXES = 0xC0,
    DS = 0xD8,
    SS = 0xD0,
    AH = 0xE4,
    DH = 0xC6,
    DL = 0xC2,
    
    LAST = 0xFE
}
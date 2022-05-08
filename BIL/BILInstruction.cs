namespace Batch2OS.BIL;

public class BILInstruction
{
    public readonly BILOpCodes OpCode;
    public readonly byte[] Operands;

    public BILInstruction(BILOpCodes opcode, params byte[] operands)
    {
        OpCode = opcode;
        Operands = operands;
    }
    
    public BILInstruction(BILOpCodes opcode)
    {
        OpCode = opcode;
        Operands = Array.Empty<byte>();
    }
}
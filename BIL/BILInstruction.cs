namespace Batch2OS.BIL;

public class BILInstruction
{
    public readonly BILOpCode OpCode;
    public readonly IEnumerable<byte> Operands;

    public BILInstruction(BILOpCode opcode, params byte[] operands)
    {
        OpCode = opcode;
        Operands = operands;
    }

    public BILInstruction(BILOpCode opcode, IEnumerable<byte> operands)
    {
        OpCode = opcode;
        Operands = operands;
    }

    public BILInstruction(BILOpCode opcode)
    {
        OpCode = opcode;
        Operands = Enumerable.Empty<byte>();
    }
}
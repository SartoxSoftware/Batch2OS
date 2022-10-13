use crate::compiler::opcode::OpCode;
use crate::compiler::label::Label;

pub struct Builder {
    pub bytes: Vec<u8>,
    pub labels: Vec<Label>
}

impl Builder {
    pub fn get_sectors(&mut self) -> usize {
        let mut sectors = self.bytes.len();

        while sectors % 512 != 0 {
            sectors += 1;
        }

        sectors
    }

    pub fn inst(&mut self, opcode: OpCode) {
        self.bytes.push(opcode as u8);
    }

    pub fn inst1_8(&mut self, opcode: OpCode, operand: u8) {
        self.bytes.push(opcode as u8);
        self.bytes.push(operand);
    }

    pub fn inst2_8(&mut self, opcode: OpCode, operand1: u8, operand2: u8) {
        self.bytes.push(opcode as u8);
        self.bytes.push(operand1);
        self.bytes.push(operand2);
    }

    pub fn inst1_16(&mut self, opcode: OpCode, operand: u16) {
        self.bytes.push(opcode as u8);

        self.bytes.push((operand & 0xFF) as u8);
        self.bytes.push(((operand >> 8) & 0xFF) as u8);
    }

    pub fn inst2_16(&mut self, opcode: OpCode, operand1: u16, operand2: u16) {
        self.bytes.push(opcode as u8);

        self.bytes.push((operand1 & 0xFF) as u8);
        self.bytes.push(((operand1 >> 8) & 0xFF) as u8);

        self.bytes.push((operand2 & 0xFF) as u8);
        self.bytes.push(((operand2 >> 8) & 0xFF) as u8);
    }
}
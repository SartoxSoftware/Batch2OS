use std::ops::Add;
use crate::compiler::builder::Builder;
use crate::compiler::opcode::OpCode;
use crate::compiler::register::Register;

mod opcode;
mod register;
mod builder;

pub fn compile(code: Vec<String>, base_address: u16, load_address: u16) -> Vec<u8>
{
    let mut builder = Builder { bytes: vec![] };
    let mut color: u8 = 7; // Light gray on black by default
    let mut echo = true;

    let ax_es = encode_mod_rm(Register::AL_EAX_ES, Register::AL_EAX_ES);

    // Initialization
    builder.inst1_16(OpCode::JMP_FAR, base_address + 5);
    builder.bytes.push(0); builder.bytes.push(0);

    // Reset segment registers
    builder.inst1_8(OpCode::XOR_RM32_R32, ax_es);
    builder.inst1_8(OpCode::MOV_RM16_SEG, ax_es);
    builder.inst1_8(OpCode::MOV_RM16_SEG, encode_mod_rm(Register::BL_EBX_DS, Register::AL_EAX_ES));
    builder.inst1_8(OpCode::MOV_RM16_SEG, encode_mod_rm(Register::DL_EDX_SS, Register::AL_EAX_ES));

    // Read sectors
    builder.inst1_16(OpCode::MOV_IMM32_EBX, load_address);
    builder.inst1_8(OpCode::MOV_IMM8_AH, 2);
    builder.inst1_8(OpCode::MOV_IMM8_AL, 0);
    let index = builder.bytes.len() - 1;
    builder.inst1_8(OpCode::MOV_IMM8_DL, 0x80);
    builder.inst1_8(OpCode::MOV_IMM8_CH, 0);
    builder.inst1_8(OpCode::MOV_IMM8_CL, 2);
    builder.inst1_8(OpCode::MOV_IMM8_DH, 0);
    builder.inst1_8(OpCode::INT, 0x13);

    // Check for errors
    builder.inst1_8(OpCode::OR_RM8_R8, encode_mod_rm(Register::AH_ESP_FS, Register::AH_ESP_FS));
    builder.inst1_8(OpCode::JNZ, 0xFE); // TODO: Remove this 0xFE

    // If none, jump to the next sector
    builder.inst1_16(OpCode::JMP_FAR, load_address);
    builder.bytes.push(0); builder.bytes.push(0);

    // Pad out with zeroes
    for _i in builder.bytes.len()..510
    {
        builder.bytes.push(0);
    }

    // Make the binary bootable under real mode
    builder.bytes.push(0x55);
    builder.bytes.push(0xAA);

    clear(&mut builder);

    for line in code
    {
        let args: Vec<&str> = line.splitn(2, ' ').collect();
        let cmd = args[0].to_lowercase();

        if echo && !cmd.starts_with('@')
        {
            print(&mut builder, &line, color);
        }

        match cmd.as_str()
        {
            "@echo" => echo = args[1] == "on",
            "help" => print(&mut builder, "help - Shows all commands with a brief description.\ncls - Clears the screen.\necho - Echoes a message, or turns on echo for commands.\npause - Pauses the OS until a key is pressed.\nver - Shows the version of Batch2OS.", color),
            "cls" => clear(&mut builder),
            "echo" =>
                {
                    if args.len() == 1
                    {
                        print(&mut builder, format!("ECHO is {}.", if echo { "on" } else { "off" }).as_str(), color);
                        continue
                    }

                    let msg = args[1];
                    match msg
                    {
                        "on" => echo = true,
                        "off" => echo = false,
                        _ => print(&mut builder, msg, color)
                    }
                },
            "color" => color = u8::from_str_radix(args[1], 16).unwrap(),
            "pause" => pause(&mut builder, color),
            "ver" => print(&mut builder, "Batch2OS v1.1.0", color),
            "rem" | "::" | "title" => {},
            _ => panic!("Unknown or unsupported instruction: {}", cmd)
        }
    }

    builder.bytes[index] = ((builder.get_sectors() - 1) / 512) as u8;
    builder.bytes
}

fn pause(builder: &mut Builder, color: u8)
{
    print(builder, "Press any key to continue...", color);
    builder.inst1_8(OpCode::MOV_IMM8_AH, 0);
    builder.inst1_8(OpCode::INT, 0x16);
}

fn clear(builder: &mut Builder)
{
    // Clear screen
    builder.inst1_8(OpCode::MOV_IMM8_AH, 0);
    builder.inst1_8(OpCode::MOV_IMM8_AL, 3);
    builder.inst1_8(OpCode::INT, 0x10);

    // Set cursor position
    builder.inst1_8(OpCode::MOV_IMM8_AH, 2); // Function code
    builder.inst1_8(OpCode::MOV_IMM8_BH, 0); // Page number
    builder.inst1_8(OpCode::MOV_IMM8_DH, 0); // Row
    builder.inst1_8(OpCode::MOV_IMM8_DL, 0); // Column?
    builder.inst1_8(OpCode::INT, 0x10);
}

fn print(builder: &mut Builder, text: &str, color: u8)
{
    for b in text.bytes()
    {
        if b == 0x0A // ASCII Line Feed
        {
            new_line(builder);
            continue
        }

        // Print character
        builder.inst1_8(OpCode::MOV_IMM8_AL, b);
        builder.inst1_8(OpCode::MOV_IMM8_AH, 9); // Tell BIOS that we need to print a character on screen
        builder.inst1_8(OpCode::MOV_IMM8_BL, color); // Color attribute
        builder.inst2_8(OpCode::MOV_IMM32_ECX, 1, 0); // Print the character once
        builder.inst1_8(OpCode::INT, 0x10);

        // Update cursor
        builder.inst1_8(OpCode::MOV_IMM8_AH, 2); // Function code
        builder.inst1_8(OpCode::INC_RM8, encode_mod_rm(Register::DL_EDX_SS, Register::AL_EAX_ES)); // Update cursor position
        builder.inst1_8(OpCode::INT, 0x10);
    }

    new_line(builder);
}

fn new_line(builder: &mut Builder)
{
    builder.inst1_8(OpCode::MOV_IMM8_AH, 2); // Function code
    builder.inst1_8(OpCode::INC_RM8, encode_mod_rm(Register::DH_ESI, Register::AL_EAX_ES)); // New line
    builder.inst1_8(OpCode::MOV_IMM8_DL, 0); // Column
    builder.inst1_8(OpCode::INT, 0x10);
}

fn encode_mod_rm(dst: Register, src: Register) -> u8
{
    u8::from_str_radix(&String::with_capacity(8)
        .add("11")
        .add(&format!("{:03b}", src as u8))
        .add(&format!("{:03b}", dst as u8)), 2).unwrap()
}
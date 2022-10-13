mod compiler;

use std::{env, fs, io, u16};
use std::io::BufRead;

fn main() {
    let args: Vec<String> = env::args().collect();
    if args.len() < 5 {
        panic!("Usage: Batch2OS <input> <output> <baseAddress> <loadAddress>");
    }

    let input = &args[1];
    let output = &args[2];
    let base_address = as_u16(&args[3]);
    let load_address = as_u16(&args[4]);

    let code = read_lines(input);
    let bytes = compiler::compile(code, base_address, load_address);

    fs::write(output, bytes).expect(&format!("Could not write bytes to output file \"{}\".", output));
}

fn as_u16(str: &String) -> u16 {
    return if str.starts_with("0x") {
        u16::from_str_radix(str.trim_start_matches("0x"), 16).expect(&format!("Could not parse string \"{}\" as a hexadecimal number.", str))
    } else {
        str.parse::<u16>().expect(&format!("Could not parse string \"{}\" as a decimal number.", str))
    }
}

fn read_lines(path: &String) -> Vec<String> {
    let file = fs::File::open(path).expect(&format!("Could not open file \"{}\".", path));
    let reader = io::BufReader::new(file);

    reader.lines().filter_map(Result::ok).collect()
}

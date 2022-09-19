mod compiler;

use std::{env, fs, io, u16};
use std::io::BufRead;

fn main()
{
    let args: Vec<String> = env::args().collect();
    if args.len() < 5
    {
        panic!("Usage: Batch2OS <input> <output> <baseAddress> <loadAddress>");
    }

    let input = &args[1];
    let output = &args[2];
    let base_address = &args[3];
    let load_address = &args[4];

    let code = read_lines(input);
    let bytes = compiler::compile
        (
            code,
            if base_address.starts_with("0x") { u16::from_str_radix(base_address.trim_start_matches("0x"), 16).expect(&format!("Could not parse string \"{}\" as a hexadecimal number.", base_address)) } else { base_address.parse::<u16>().expect(&format!("Could not parse string \"{}\" as a decimal number.", base_address)) },
            if load_address.starts_with("0x") { u16::from_str_radix(load_address.trim_start_matches("0x"), 16).expect(&format!("Could not parse string \"{}\" as a hexadecimal number.", load_address)) } else { load_address.parse::<u16>().expect(&format!("Could not parse string \"{}\" as a decimal number.", load_address)) }
        );

    fs::write(output, bytes).expect(&format!("Could not write bytes to output file \"{}\".", output));
}

fn read_lines(path: &String) -> Vec<String>
{
    let file = fs::File::open(path).expect(&format!("Could not open the file \"{}\".", path));
    let reader = io::BufReader::new(file);

    reader.lines().filter_map(Result::ok).collect()
}

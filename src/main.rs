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
            if base_address.starts_with("0x") { u16::from_str_radix(base_address.trim_start_matches("0x"), 16).unwrap() } else { base_address.parse::<u16>().unwrap() },
            if load_address.starts_with("0x") { u16::from_str_radix(load_address.trim_start_matches("0x"), 16).unwrap() } else { load_address.parse::<u16>().unwrap() }
        );

    fs::write(output, bytes).expect("Couldn't write bytes to output file.");
}

fn read_lines(path: &String) -> Vec<String>
{
    let file = fs::File::open(path).unwrap();
    let reader = io::BufReader::new(file);

    reader.lines().filter_map(Result::ok).collect()
}

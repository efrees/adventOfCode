use intcode::*;

pub fn solve() {
    println!("Day 9");

    let raw_program = adventlib::read_input_raw("day09input.txt");

    let mut computer = Computer::new();

    let program_state = parse_program(&raw_program);
    let mut output_stream = Vec::new();

    computer.load_program(program_state);
    computer.set_input_stream(vec![1]);
    computer.run_program_with_output(&mut output_stream);

    // println!("Diagnostic: {:#?}", output_stream);
    println!("BOOST keycode (part 1): {}", output_stream.last().unwrap());

    let program_state = parse_program(&raw_program);
    let mut output_stream = Vec::new();

    computer.load_program(program_state);
    computer.set_input_stream(vec![2]);
    computer.run_program_with_output(&mut output_stream);

    // println!("Diagnostic: {:#?}", output_stream);
    println!("Coordinates (part 2): {}", output_stream.last().unwrap());
}

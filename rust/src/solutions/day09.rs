use intcode::*;

pub fn solve() {
    println!("Day 9");

    let raw_program = adventlib::read_input_raw("day09input.txt");

    let mut computer = Computer::new();

    let program_state = parse_program(&raw_program);

    computer.load_program(program_state);
    computer.set_input_stream(vec![1]);
    let last_output = computer.run_program();

    println!("BOOST keycode (part 1): {}", last_output.unwrap());

    let program_state = parse_program(&raw_program);

    computer.load_program(program_state);
    computer.set_input_stream(vec![2]);
    let last_output = computer.run_program();

    println!("Coordinates (part 2): {}", last_output.unwrap());
}

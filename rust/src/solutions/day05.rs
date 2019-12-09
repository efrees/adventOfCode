use intcode::*;

pub fn solve() {
    println!("Day 5");

    let raw_program = adventlib::read_input_raw("day05input.txt");
    let diagnostic_input = vec![1];

    let program_state = parse_program(&raw_program);

    let mut computer = Computer::for_program(program_state);
    computer.set_input_stream(diagnostic_input);
    let last_output = computer.run_program();

    println!("Diagnostic code (part 1): {}", last_output.unwrap());

    let program_state = parse_program(&raw_program);

    computer.load_program(program_state);
    computer.set_input_stream(vec![5]);
    let last_output = computer.run_program();

    println!("Diagnostic code (part 2): {}", last_output.unwrap());
}

fn parse_program(raw_program: &String) -> Vec<i64> {
    let int_parser = |x: &str| x.parse::<i64>().unwrap();
    return raw_program.trim().split(',').map(int_parser).collect();
}

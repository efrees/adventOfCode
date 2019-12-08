use intcode::*;

pub fn solve() {
    println!("Day 5");

    let raw_program = adventlib::read_input_raw("day05input.txt");
    let diagnostic_input = vec![1];

    let program_state = parse_program(&raw_program);
    let mut output_stream = Vec::<i32>::new();

    let mut computer = Computer::for_program(program_state);
    computer.set_input_stream(diagnostic_input);
    computer.run_program_with_output(&mut output_stream);

    // println!("Diagnostic output: {:#?}", output_stream);
    println!(
        "Diagnostic code (part 1): {}",
        output_stream.last().unwrap()
    );

    let program_state = parse_program(&raw_program);
    let mut output_stream = Vec::<i32>::new();

    computer.load_program(program_state);
    computer.set_input_stream(vec![5]);
    computer.run_program_with_output(&mut output_stream);

    println!(
        "Diagnostic code (part 2): {}",
        output_stream.last().unwrap()
    );
}

fn parse_program(raw_program: &String) -> Vec<i32> {
    let int_parser = |x: &str| x.parse::<i32>().unwrap();
    return raw_program.trim().split(',').map(int_parser).collect();
}

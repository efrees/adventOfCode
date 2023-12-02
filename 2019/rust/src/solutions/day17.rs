use intcode::{parse_program, Computer};

static ENABLE_CONTINUOUS_DISPLAY: bool = false;

pub fn solve() {
    println!("Day 17");

    let raw_program = adventlib::read_input_raw("day17input.txt");
    let mut computer = Computer::for_raw_program(&raw_program);
    let mut output = Vec::new();
    computer.run_program_with_output(&mut output);

    let image = render_scaffolding(output);
    print!("{}", image);

    let lines: Vec<_> = image.split_whitespace().collect();
    let mut alignment_parameter_sum = 0;
    for row in 0..lines.len() {
        for col in 0..lines[row].len() {
            if is_intersection(&lines, row, col) {
                alignment_parameter_sum += alignment_parameter(row, col);
            }
        }
    }

    println!(
        "Alignment parameter sum (part 1): {}",
        alignment_parameter_sum
    );

    let mut robot_program = parse_program(&raw_program);
    robot_program[0] = 2; // wake robot
    let input_instructions = get_movement_instruction_string();
    let input: Vec<_> = input_instructions
        .as_bytes()
        .iter()
        .map(|&b| b as i64)
        .collect();

    let mut output = Vec::new();
    let mut computer = Computer::for_program(robot_program);
    computer.set_input_stream(input);
    let last_output = computer.run_program_with_output(&mut output);

    println!("Space dust collected (part 2): {}", last_output.unwrap());
}

fn render_scaffolding(image_data: Vec<i64>) -> String {
    image_data.iter().map(|&b| (b as u8 as char)).collect()
}

fn is_intersection(lines: &Vec<&str>, row: usize, col: usize) -> bool {
    if lines[row].chars().nth(col) != Some('#') {
        return false;
    }

    let mut neighbor_count = 0;
    if row > 0 && lines[row - 1].chars().nth(col) == Some('#') {
        neighbor_count += 1;
    }
    if row < lines.len() - 1 && lines[row + 1].chars().nth(col) == Some('#') {
        neighbor_count += 1;
    }
    if col > 0 && lines[row].chars().nth(col - 1) == Some('#') {
        neighbor_count += 1;
    }
    if lines[row].chars().nth(col + 1) == Some('#') {
        neighbor_count += 1;
    }

    return neighbor_count >= 3;
}

fn alignment_parameter(row: usize, col: usize) -> usize {
    row * col
}

fn get_movement_instruction_string() -> String {
    // Determined by manually tracing the path
    let mut input_instructions = String::from("A,A,B,C,B,C,B,A,C,A\n");
    input_instructions.extend("R,8,L,12,R,8\n".chars());
    input_instructions.extend("L,10,L,10,R,8\n".chars());
    input_instructions.extend("L,12,L,12,L,10,R,10\n".chars());
    input_instructions.push(if ENABLE_CONTINUOUS_DISPLAY { 'y' } else { 'n' });
    input_instructions.push('\n');
    input_instructions
}

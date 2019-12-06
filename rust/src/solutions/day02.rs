pub fn solve() {
    println!("Day 2");

    let raw_program = adventlib::read_input_raw("day02input.txt");

    let int_parser = |x: &str| x.parse::<i32>().unwrap();
    let mut program_state: Vec<i32> = raw_program.trim().split(',').map(int_parser).collect();

    set_inputs(&mut program_state, 12, 2);

    let output = run_program(&mut program_state);

    println!("Output (part 1): {}", output);

    let mut noun = 0;
    let mut verb = 0;
    'outer: while noun < 100 {
        verb = 0;
        while verb < 100 {
            let mut program_state: Vec<i32> =
                raw_program.trim().split(',').map(int_parser).collect();

            set_inputs(&mut program_state, noun, verb);

            let output = run_program(&mut program_state);

            if output == 19690720 {
                break 'outer;
            }

            verb += 1;
        }
        noun += 1;
    }

    println!("Input values (part 2): {}", noun * 100 + verb);
}

fn set_inputs(program_state: &mut Vec<i32>, noun: i32, verb: i32) {
    program_state[1] = noun;
    program_state[2] = verb;
}

fn run_program(program_state: &mut Vec<i32>) -> i32 {
    let mut instr_ptr = 0;

    while program_state[instr_ptr] != 99 {
        match program_state[instr_ptr] {
            1 => add(program_state, instr_ptr),
            2 => mul(program_state, instr_ptr),
            op => println!("Unknown opcode: {}", op),
        };

        instr_ptr += 4;

        if instr_ptr >= program_state.len() {
            println!("out of bounds: {}", instr_ptr);
            return -1;
        }
    }

    return program_state[0];
}

fn add(program_state: &mut Vec<i32>, instr_ptr: usize) {
    let result_ptr = program_state[instr_ptr + 3] as usize;
    let op1_ptr = program_state[instr_ptr + 1] as usize;
    let op2_ptr = program_state[instr_ptr + 2] as usize;
    program_state[result_ptr] = program_state[op1_ptr] + program_state[op2_ptr];
}

fn mul(program_state: &mut Vec<i32>, instr_ptr: usize) {
    // println!("mul: {}", instr_ptr);
    let result_ptr = program_state[instr_ptr + 3] as usize;
    let op1_ptr = program_state[instr_ptr + 1] as usize;
    let op2_ptr = program_state[instr_ptr + 2] as usize;
    program_state[result_ptr] = program_state[op1_ptr] * program_state[op2_ptr];
}

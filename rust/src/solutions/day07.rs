use intcode::*;

pub fn solve() {
    println!("Day 7");

    let raw_program = adventlib::read_input_raw("day07input.txt");

    let phases = vec![0, 1, 2, 3, 4];
    let phase_permutations = get_permutations_of(phases);

    let mut max_signal = 0;
    for perm in phase_permutations {
        let signal = run_all_amplifiers(&raw_program, perm);

        if signal > max_signal {
            max_signal = signal;
        }
    }

    println!("Max signal (part 2): {}", max_signal);

    let phases = vec![5, 6, 7, 8, 9];
    let phase_permutations = get_permutations_of(phases);

    let mut max_signal = 0;
    for perm in phase_permutations {
        let signal = run_amplifiers_in_loop(&raw_program, perm);

        if signal > max_signal {
            max_signal = signal;
        }
    }

    println!("Max signal (part 2): {}", max_signal);
}

fn get_permutations_of(mut phases: Vec<u8>) -> Vec<Vec<u8>> {
    let mut phase_permutations = Vec::new();
    let size = phases.len();
    collect_permutations(&mut phases, size, &mut phase_permutations);
    return phase_permutations;
}

fn collect_permutations(phases: &mut Vec<u8>, size: usize, permutations: &mut Vec<Vec<u8>>) {
    if size == 1 {
        permutations.push(phases.clone());
        return;
    }

    for i in 0..size {
        collect_permutations(phases, size - 1, permutations);

        // if size is odd, swap first and last element
        if size % 2 == 1 {
            let temp = phases[0];
            phases[0] = phases[size - 1];
            phases[size - 1] = temp;
        }
        // If size is even, swap ith and last element
        else {
            let temp = phases[i];
            phases[i] = phases[size - 1];
            phases[size - 1] = temp;
        }
    }
}

fn run_all_amplifiers(raw_program: &String, phases: Vec<u8>) -> i64 {
    let first_input_value = 0;

    let mut output = run_amplifier_returning_output(raw_program, phases[0], first_input_value);
    output = run_amplifier_returning_output(raw_program, phases[1], output);
    output = run_amplifier_returning_output(raw_program, phases[2], output);
    output = run_amplifier_returning_output(raw_program, phases[3], output);
    return run_amplifier_returning_output(raw_program, phases[4], output);
}

fn run_amplifiers_in_loop(raw_program: &String, phases: Vec<u8>) -> i64 {
    let first_input_value = 0;
    let mut is_first_run = true;
    let mut computers = vec![
        create_computer(raw_program),
        create_computer(raw_program),
        create_computer(raw_program),
        create_computer(raw_program),
        create_computer(raw_program),
    ];

    let mut next_input = first_input_value;
    while computers[4].run_state != RunState::Halted {
        for i in 0..5 {
            let input = if is_first_run {
                vec![phases[i] as i64, next_input]
            } else {
                vec![next_input]
            };
            next_input = run_computer_for_input(&mut computers[i], input);
        }
        is_first_run = false;
    }

    return next_input;
}

fn run_amplifier_returning_output(raw_program: &String, phase: u8, input: i64) -> i64 {
    let mut computer = create_computer(raw_program);
    return run_computer_for_input(&mut computer, vec![phase as i64, input]);
}

fn create_computer(raw_program: &String) -> Computer<i64> {
    let program_state = parse_program(&raw_program);
    return Computer::for_program(program_state);
}

fn run_computer_for_input(computer: &mut Computer<i64>, input: Vec<i64>) -> i64 {
    computer.set_input_stream(input);
    return computer.run_program().unwrap();
}

fn parse_program(raw_program: &String) -> Vec<i64> {
    let int_parser = |x: &str| x.parse::<i64>().unwrap();
    return raw_program.trim().split(',').map(int_parser).collect();
}

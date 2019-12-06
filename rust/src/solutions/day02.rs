use intcode::*;

pub fn solve() {
    println!("Day 2");

    let raw_program = adventlib::read_input_raw("day02input.txt");

    let int_parser = |x: &str| x.parse::<i32>().unwrap();
    let program_state: Vec<i32> = raw_program.trim().split(',').map(int_parser).collect();

    let mut computer = Computer::for_program(program_state);
    computer.set_noun_and_verb(12, 2);

    let output = computer.run_program();

    println!("Output (part 1): {}", output);

    let mut noun = 0;
    let mut verb = 0;
    'outer: while noun < 100 {
        verb = 0;
        while verb < 100 {
            let program_state: Vec<i32> = raw_program.trim().split(',').map(int_parser).collect();

            computer.load_program(program_state);
            computer.set_noun_and_verb(noun, verb);

            let output = computer.run_program();

            if output == 19690720 {
                break 'outer;
            }

            verb += 1;
        }
        noun += 1;
    }

    println!("Input values (part 2): {}", noun * 100 + verb);
}

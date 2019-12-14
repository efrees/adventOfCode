use adventlib::grid::*;
use intcode::*;

pub fn solve() {
    println!("Day 13");

    let raw_program = adventlib::read_input_raw("day13input.txt");

    let mut screen_state = SparseGrid::<u8>::new();
    run_arcade_game(&mut screen_state, &raw_program);

    let block_count = screen_state.iter().filter(|(_k, &v)| v == 2).count();
    println!("Number of block tiles (part 1): {}", block_count);
}

fn run_arcade_game(grid_state: &mut SparseGrid<u8>, raw_program: &String) {
    let program_state = parse_program(raw_program);
    let mut computer = Computer::for_program(program_state);

    let mut outputs = Vec::new();
    computer.run_program_with_output(&mut outputs);

    for i in (0..outputs.len()).step_by(3) {
        let output_location = Point::new(outputs[i], outputs[i + 1]);

        grid_state.insert(output_location, outputs[i + 2] as u8);
    }
}

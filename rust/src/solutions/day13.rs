use adventlib::grid::*;
use intcode::*;
use std::io::Write;

pub fn solve() {
    println!("Day 13");

    let raw_program = adventlib::read_input_raw("day13input.txt");

    let mut screen_state = SparseGrid::<u8>::new();
    let program_state = parse_program(&raw_program);
    run_arcade_game(&mut screen_state, program_state);

    let block_count = screen_state.iter().filter(|(_k, &v)| v == 2).count();
    screen_state.print(&render_tile);
    println!("Number of block tiles (part 1): {}", block_count);

    let mut screen_state = SparseGrid::<u8>::new();
    let mut program_state = parse_program(&raw_program);
    program_state[0] = 2;
    let score = run_arcade_game(&mut screen_state, program_state);

    println!("Final score (part 2): {}", score);
}

fn run_arcade_game(screen: &mut SparseGrid<u8>, program_state: Vec<i64>) -> i64 {
    let mut computer = Computer::for_program(program_state);
    let mut score = 0;

    let mut ball_position_before = find_ball(screen);
    let mut next_input = 0;
    while computer.run_state != RunState::Halted {
        let mut outputs = Vec::new();
        computer.set_input_stream(vec![next_input]);
        computer.run_program_with_output(&mut outputs);

        for i in (0..outputs.len()).step_by(3) {
            let output_location = Point::new(outputs[i], outputs[i + 1]);
            let output_argument = outputs[i + 2];

            if output_location.x == -1 && output_location.y == 0 {
                score = output_argument;
            } else {
                screen.insert(output_location, output_argument as u8);
            }
        }

        // screen.print(&render_tile);
        // std::io::stdout().flush();
        next_input = decide_move_direction(screen, ball_position_before);
        ball_position_before = find_ball(screen);
    }
    return score;
}

fn decide_move_direction(screen: &SparseGrid<u8>, previous_ball_pos: Option<Point>) -> i64 {
    let ball_position = find_ball(screen).expect("There should be a ball");
    let paddle_position = screen
        .iter()
        .find(|(&_k, &v)| v == 3)
        .expect("There should be a paddle")
        .0;

    let predicted_ball_pos = match previous_ball_pos {
        Some(prev) => ball_position.vec_add(&ball_position).vec_subtract(&prev),
        None => ball_position,
    };

    return if predicted_ball_pos.x < paddle_position.x {
        -1
    } else if predicted_ball_pos.x > paddle_position.x {
        1
    } else {
        0
    };
}

fn find_ball(screen: &SparseGrid<u8>) -> Option<Point> {
    return screen
        .iter()
        .find(|(&_k, &v)| v == 4)
        .map(|(&k, _v)| k)
        .clone();
}

fn render_tile(tile_code: Option<&u8>) -> char {
    match tile_code.cloned().unwrap_or(0) {
        0 => ' ',
        1 => '|',
        2 => '#',
        3 => '_',
        4 => '*',
        x => panic!(format!("Unexpected tile code: {}", x)),
    }
}

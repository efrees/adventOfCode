use adventlib::grid::*;
use intcode::*;
use std::collections::*;

pub fn solve() {
    println!("Day 11");

    let raw_program = adventlib::read_input_raw("day11input.txt");

    let mut grid_state = HashMap::<Point, i64>::new();
    paint_grid(&mut grid_state, &raw_program);

    println!(
        "Number of cells touched (part 1): {}",
        grid_state.keys().len()
    );

    let mut grid_state = HashMap::<Point, i64>::new();
    grid_state.insert(Point::new(0, 0), 1);
    paint_grid(&mut grid_state, &raw_program);

    let min_y = grid_state.keys().map(|&p| p.y).min().unwrap();
    let max_y = grid_state.keys().map(|&p| p.y).max().unwrap();
    let min_x = grid_state.keys().map(|&p| p.x).min().unwrap();
    let max_x = grid_state.keys().map(|&p| p.x).max().unwrap();

    println!("Letters traced by robot (part 2):");
    for i in (min_y..=max_y).rev() {
        for j in min_x..=max_x {
            let color = grid_state.get(&Point::new(j, i)).cloned().unwrap_or(0);
            print!("{}", if color == 1 { '#' } else { ' ' });
        }
        println!("");
    }
}

fn paint_grid(grid_state: &mut HashMap<Point, i64>, raw_program: &String) {
    let program_state = parse_program(raw_program);
    let mut computer = Computer::for_program(program_state);

    let mut cur_direction = Direction::Up;
    let mut cur_position = Point::new(0, 0);

    while computer.run_state != RunState::Halted {
        let mut outputs = Vec::with_capacity(2);

        let cur_color = grid_state.get(&cur_position).cloned().unwrap_or(0);
        computer.set_input_stream(vec![cur_color]);

        computer.run_program_with_output(&mut outputs);
        grid_state.insert(cur_position, outputs[0]);

        cur_direction = match outputs[1] {
            0 => cur_direction.turn_left(),
            1 => cur_direction.turn_right(),
            _ => panic!("Unexpected output: {:#?}", outputs),
        };

        cur_position = cur_position.vec_add(&cur_direction.as_vector());
    }
}

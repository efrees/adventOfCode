use adventlib::grid::*;
use intcode::*;

pub fn solve() {
    println!("Day 11");

    let raw_program = adventlib::read_input_raw("day11input.txt");

    let mut grid_state = SparseGrid::<i64>::new();
    paint_grid(&mut grid_state, &raw_program);

    println!("Number of cells touched (part 1): {}", grid_state.len());

    let mut grid_state = SparseGrid::<i64>::new();
    grid_state.insert(Point::new(0, 0), 1);
    paint_grid(&mut grid_state, &raw_program);

    println!("Letters traced by robot (part 2):");
    let cell_printer = |c: Option<&i64>| {
        if c.cloned().unwrap_or(0) == 1 {
            '#'
        } else {
            ' '
        }
    };
    grid_state.print(&cell_printer)
}

fn paint_grid(grid_state: &mut SparseGrid<i64>, raw_program: &String) {
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

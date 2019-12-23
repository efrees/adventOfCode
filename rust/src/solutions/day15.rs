use adventlib::grid::*;
use intcode::*;

pub fn solve() {
    println!("Day 11");

    let raw_program = adventlib::read_input_raw("day15input.txt");

    let mut grid_state = SparseGrid::<u8>::new();
    explore_map(&mut grid_state, &raw_program);

    let shortest_path = find_shortest_path_to_target(&grid_state);

    let cell_printer = |c: Option<&u8>| match c {
        Some(&x) if x == MAP_OPEN => '.',
        Some(&x) if x == MAP_WALL => '#',
        Some(&x) if x == MAP_START => '^',
        Some(&x) if x == MAP_OXYGEN => '$',
        Some(_) | None => ' ',
    };
    grid_state.print(&cell_printer);

    println!("Fewest moves (part 1): {}", shortest_path);
}

static MAP_WALL: u8 = 0;
static MAP_OPEN: u8 = 1;
static MAP_OXYGEN: u8 = 2;
static MAP_START: u8 = 3;

static MOVE_NORTH: u8 = 1;
static MOVE_SOUTH: u8 = 2;
static MOVE_WEST: u8 = 3;
static MOVE_EAST: u8 = 4;

fn explore_map(grid_state: &mut SparseGrid<u8>, raw_program: &String) {
    let program_state = parse_program(raw_program);
    let mut computer = Computer::for_program(program_state);

    let cur_position = Point::new(0, 0);

    grid_state.insert(cur_position, MAP_START);
    explore_map_rec(grid_state, &mut computer, &cur_position);
}

fn explore_map_rec(
    grid_state: &mut SparseGrid<u8>,
    computer: &mut Computer<i64>,
    cur_position: &Point,
) {
    let directions = vec![
        Direction::Left,  // West
        Direction::Up,    // North
        Direction::Right, // East
        Direction::Down,  // South
    ];

    for direction in directions {
        let next_step = direction.as_vector().vec_add(&cur_position);
        if grid_state.get(&next_step) != None {
            continue; // already been there
        }

        let move_command = get_move_command(direction);
        let mut outputs = Vec::with_capacity(1);
        computer.add_input(move_command);
        computer.run_program_with_output(&mut outputs);

        match outputs[0] {
            0 => {
                grid_state.insert(next_step, MAP_WALL);
                continue;
            }
            1 => {
                grid_state.insert(next_step, MAP_OPEN);
                explore_map_rec(grid_state, computer, &next_step);
            }
            2 => {
                grid_state.insert(next_step, MAP_OXYGEN);
                explore_map_rec(grid_state, computer, &next_step);
            }
            _ => panic!("Unexpected output from robot control."),
        }

        let move_back_command = get_move_command(direction.turn_right().turn_right());
        computer.add_input(move_back_command);
        computer.run_program();
    }
}

fn get_move_command(direction: Direction) -> i64 {
    let cmd = match direction {
        Direction::Up => MOVE_NORTH,
        Direction::Down => MOVE_SOUTH,
        Direction::Left => MOVE_WEST,
        Direction::Right => MOVE_EAST,
    };
    cmd as i64
}

fn find_shortest_path_to_target(grid_state: &SparseGrid<u8>) -> i32 {
    0
}

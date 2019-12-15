use adventlib::grid::*;
use crossterm::event::{poll, read, Event, KeyCode};
use crossterm::{cursor, execute, style, terminal, Result};
use intcode::*;
use std::io::{stdout, Write};

use std::thread;
use std::time::Duration;

static RENDER_GAME_FRAMES: bool = false;
static ENABLE_MANUAL_CONTROL: bool = false;

pub fn solve() {
    println!("Day 13");

    let raw_program = adventlib::read_input_raw("day13input.txt");

    let mut screen_state = SparseGrid::<u8>::new();
    let program_state = parse_program(&raw_program);
    run_arcade_game(&mut screen_state, program_state).expect("Game errored");

    let block_count = screen_state.iter().filter(|(_k, &v)| v == 2).count();
    screen_state.print(&render_tile);
    println!("Number of block tiles (part 1): {}", block_count);

    loop {
        let mut screen_state = SparseGrid::<u8>::new();
        let mut program_state = parse_program(&raw_program);
        program_state[0] = 2;
        let score = run_arcade_game(&mut screen_state, program_state).expect("Game errored");

        println!("Final score (part 2): {}", score);
    }
}

fn run_arcade_game(screen: &mut SparseGrid<u8>, program_state: Vec<i64>) -> Result<i64> {
    let mut computer = Computer::for_program(program_state);
    let mut score = 0;

    let mut ball_position_before = find_ball(screen);
    let mut next_input = 0;

    if RENDER_GAME_FRAMES {
        start_game_window()?
    };

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

        if RENDER_GAME_FRAMES {
            render_frame(screen, score)?
        };

        next_input = decide_move_direction(screen, ball_position_before);
        ball_position_before = find_ball(screen);
    }

    if RENDER_GAME_FRAMES {
        close_game_window()?
    };

    return Ok(score);
}

fn decide_move_direction(screen: &SparseGrid<u8>, previous_ball_pos: Option<Point>) -> i64 {
    if ENABLE_MANUAL_CONTROL {
        return read_direction_from_keyboard().unwrap();
    }
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

fn start_game_window() -> Result<()> {
    execute!(
        stdout(),
        terminal::EnterAlternateScreen,
        terminal::Clear(terminal::ClearType::All),
        cursor::SavePosition,
        cursor::Hide,
    )
}

fn close_game_window() -> Result<()> {
    execute!(stdout(), terminal::LeaveAlternateScreen)
}

fn render_frame(frame: &SparseGrid<u8>, score: i64) -> Result<()> {
    thread::sleep(Duration::from_millis(300));

    let rendered_screen = frame.render_to_string(&render_tile);
    execute!(
        stdout(),
        cursor::RestorePosition,
        style::Print(&rendered_screen),
        cursor::RestorePosition,
        style::Print(format!("{}", score)),
    )
}

fn read_direction_from_keyboard() -> Result<i64> {
    clear_event_stream()?;
    match read()? {
        Event::Key(event) => match event.code {
            KeyCode::Left => Ok(-1),
            KeyCode::Right => Ok(1),
            _ => Ok(0),
        },
        _ => Ok(0),
    }
}

fn clear_event_stream() -> Result<()> {
    while poll(Duration::from_millis(1))? {
        read()?;
    }

    Ok(())
}

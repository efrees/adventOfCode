use adventlib::grid::*;
use intcode::Computer;

pub fn solve() {
    println!("Day 19");

    let raw_program = adventlib::read_input_raw("day19input.txt");

    let mut grid = SparseGrid::new();

    let mut points_affected = 0;
    let mut slope_upper = 0_f64;
    let mut slope_lower = std::f64::MAX;
    for x in 0..50 {
        for y in 0..50 {
            let mut computer = Computer::for_raw_program(&raw_program);
            computer.add_input(x);
            computer.add_input(y);
            let response = computer.run_program();

            // 0 = stationary, 1 = pulled
            points_affected += response.unwrap();

            if x > 0 && response == Some(1) {
                let slope = y as f64 / x as f64;

                if slope > slope_upper {
                    slope_upper = slope;
                }
                if slope < slope_lower {
                    slope_lower = slope;
                }
            }

            grid.insert(
                Point::new(x, y),
                if response == Some(1) { '#' } else { '.' },
            );
        }
    }

    grid.print(&|c| c.cloned().unwrap());
    println!("Points affected in 50x50 (part 1): {}", points_affected);

    // Slopes less than 0.5 make the y-direction the more constrained.
    // Using estimated (conservative) slopes, find the x-value
    // where the y-distance between the lines is >= 100
    // y >= (x+100)/slope_upper
    // y <= x/slope_lower - 100
    // y2-y1 = x*(slope_upper - slope_lower)
    // dbg!(slope_lower, slope_upper);
    // let x = 100.0 / (slope_upper - slope_lower);
    // let y = x * slope_lower;

    let x = (100.0 + 100.0 * slope_lower) / (slope_upper - slope_lower);
    let y = slope_lower * (x + 100.0);

    // > 5380393
    println!(
        "First 100x100 at ({}, {}): {}",
        x.round() as i64,
        y.round() as i64,
        (x.floor() * 10000.0 + y) as i64
    );

    let mut grid = SparseGrid::new();
    for ix in (x.round() as i64..).take(110) {
        for iy in (y.round() as i64..).take(110) {
            let mut computer = Computer::for_raw_program(&raw_program);
            computer.add_input(ix);
            computer.add_input(iy);
            let response = computer.run_program();

            grid.insert(
                Point::new(ix, iy),
                if response == Some(1) { '#' } else { '.' },
            );
        }
    }

    grid.print(&|c| c.cloned().unwrap());
}

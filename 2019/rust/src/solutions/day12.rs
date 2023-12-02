use adventlib::grid::*;
use num_integer::*;
use regex::Regex;
use std::collections::*;

pub fn solve() {
    println!("Day 12");

    let raw_moons = adventlib::read_input_lines("day12input.txt");
    let mut moons: Vec<_> = raw_moons.iter().map(|m| parse_moon(m)).collect();
    let mut velocities: Vec<_> = vec![0; moons.len()]
        .iter()
        .map(|_| Point3d::new(0, 0, 0))
        .collect();

    for _time_step in 0..1000 {
        for (i, moon) in moons.iter().enumerate() {
            let x_pull = moons
                .iter()
                .map(|&other| normalized_compare(other.x, moon.x))
                .sum();
            let y_pull = moons
                .iter()
                .map(|&other| normalized_compare(other.y, moon.y))
                .sum();
            let z_pull = moons
                .iter()
                .map(|&other| normalized_compare(other.z, moon.z))
                .sum();

            let cur_velocity = velocities[i];
            let new_velocity = cur_velocity.vec_add(&Point3d::new(x_pull, y_pull, z_pull));
            velocities[i] = new_velocity;
        }

        moons = moons
            .iter()
            .zip(velocities.iter())
            .map(|(m, v)| m.vec_add(&v))
            .collect();
    }

    let total_energy = get_total_energy(&moons, &velocities);
    println!("Total energy (part 1): {}", total_energy);

    let moons: Vec<_> = raw_moons.iter().map(|m| parse_moon(m)).collect();
    let velocities: Vec<_> = vec![0; moons.len()]
        .iter()
        .map(|_| Point3d::new(0, 0, 0))
        .collect();

    let x_loop = simulate_dimension_until_loop(
        moons.iter().map(|&m| m.x).collect(),
        velocities.iter().map(|&v| v.x).collect(),
    );
    let y_loop = simulate_dimension_until_loop(
        moons.iter().map(|&m| m.y).collect(),
        velocities.iter().map(|&v| v.y).collect(),
    );
    let z_loop = simulate_dimension_until_loop(
        moons.iter().map(|&m| m.z).collect(),
        velocities.iter().map(|&v| v.z).collect(),
    );

    // Haven't had to handle the case of shifted loops.
    assert_eq!(x_loop.0 + y_loop.0 + z_loop.0, 0);
    // x loop: (
    //     0,
    //     268296,
    // )
    // y loop: (
    //     0,
    //     231614,
    // )
    // z loop: (
    //     0,
    //     108344,
    // )
    let system_loop = x_loop.1.lcm(&y_loop.1).lcm(&z_loop.1);
    println!("Steps until repeat (part 2): {}", system_loop);
}

fn simulate_dimension_until_loop(mut locations: Vec<i64>, mut velocities: Vec<i64>) -> (i64, i64) {
    // Same simulation, but independent for x, y, and z
    // Hoping to find lcm of separate loops

    let mut seen_states = HashMap::new();
    let mut time_steps = 0_i64;

    let mut last_state = represent_state(&locations, &velocities);
    while !seen_states.contains_key(&last_state) {
        seen_states.insert(last_state, time_steps);
        time_steps += 1;

        for (i, moon) in locations.iter().enumerate() {
            let pull: i64 = locations
                .iter()
                .map(|&other| normalized_compare(other, *moon))
                .sum();

            let cur_velocity = velocities[i];
            let new_velocity = cur_velocity + pull;
            velocities[i] = new_velocity;
        }

        locations = locations
            .iter()
            .zip(velocities.iter())
            .map(|(m, v)| m + v)
            .collect();

        last_state = represent_state(&locations, &velocities);
    }

    let loop_start = seen_states.get(&last_state).unwrap();
    let loop_len = time_steps - loop_start;

    return (*loop_start, loop_len);
}

fn parse_moon(raw_moon: &String) -> Point3d {
    lazy_static! {
        static ref PATTERN: Regex =
            Regex::new(r"<.=(-?\d+), .=(-?\d+), .=(-?\d+)>").expect("pattern for parsing");
    }

    let captures = PATTERN
        .captures(raw_moon)
        .expect("Line should match format");
    Point3d::new(
        captures[1].parse().unwrap(),
        captures[2].parse().unwrap(),
        captures[3].parse().unwrap(),
    )
}

fn get_total_energy(moons: &Vec<Point3d>, velocities: &Vec<Point3d>) -> i64 {
    return moons
        .iter()
        .zip(velocities.iter())
        .map(|(m, v)| {
            m.manhattan_dist_to(&Point3d::origin()) * v.manhattan_dist_to(&Point3d::origin())
        })
        .sum();
}

fn normalized_compare(a: i64, b: i64) -> i64 {
    if a < b {
        -1
    } else if a > b {
        1
    } else {
        0
    }
}

fn represent_state(locations: &Vec<i64>, velocities: &Vec<i64>) -> String {
    // Assuming 4 points
    return format!(
        "{},{},{},{},{},{},{},{}",
        locations[0],
        locations[1],
        locations[2],
        locations[3],
        velocities[0],
        velocities[1],
        velocities[2],
        velocities[3]
    );
}

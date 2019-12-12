use adventlib::grid::*;
use regex::Regex;

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

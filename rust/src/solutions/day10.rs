use adventlib::grid::*;
use std::collections::HashSet;

pub fn solve() {
    println!("Day 10");

    let raw_map = adventlib::read_input_lines("day10input.txt");

    let asteroid_locations = get_asteroid_locations(&raw_map);
    let mut max_visible = 0;

    for i in 0..asteroid_locations.len() {
        let observing_asteroid = asteroid_locations.get(i).unwrap();
        let mut cur_asteroid_angles = HashSet::new();
        for j in 0..asteroid_locations.len() {
            if i == j {
                continue; // don't count self
            }

            // Reference the angle to each asteroid as the smallest integral vector in its direction
            // to ensure asteroids in the same direction will have the same vector computed.
            let candidate = asteroid_locations.get(j).unwrap();
            let raw_vector = candidate.vec_subtract(observing_asteroid);
            let reduced_vector = reduce_vector(raw_vector);
            cur_asteroid_angles.insert(reduced_vector);
        }

        let visible = cur_asteroid_angles.len();
        if visible > max_visible {
            max_visible = visible;
        }

        cur_asteroid_angles.clear();
    }

    println!("Count from best location (part 1): {}", max_visible);
}

fn get_asteroid_locations(raw_map: &Vec<String>) -> Vec<Point> {
    let mut asteroid_locations = Vec::with_capacity(300);
    let mut cur_y = 0;
    for line in raw_map {
        let mut cur_x = 0;
        for ch in line.chars() {
            if ch == '#' {
                asteroid_locations.push(Point::new(cur_x, cur_y))
            }
            cur_x += 1;
        }

        cur_y += 1;
    }
    return asteroid_locations;
}

fn reduce_vector(vector: Point) -> Point {
    let gcd = gcd(vector.x.abs(), vector.y.abs());
    Point::new(vector.x / gcd, vector.y / gcd)
}

fn gcd(a: i64, b: i64) -> i64 {
    if a == b || b == 0 {
        return a;
    }

    if a < b {
        return gcd(b, a);
    }

    if a % b == 0 {
        return b;
    }

    return gcd(b, a % b);
}

#[test]
fn gcd_simple_cases() {
    assert_eq!(gcd(4, 1), 1);
    assert_eq!(gcd(4, 2), 2);
    assert_eq!(gcd(12, 3), 3);
    assert_eq!(gcd(12, 4), 4);
    assert_eq!(gcd(12, 12), 12);
}

#[test]
fn gcd_reversed_arguments() {
    assert_eq!(gcd(1, 4), 1);
    assert_eq!(gcd(2, 4), 2);
    assert_eq!(gcd(3, 12), 3);
    assert_eq!(gcd(4, 12), 4);
}

#[test]
fn gcd_less_than_both() {
    assert_eq!(gcd(12, 8), 4);
    assert_eq!(gcd(23, 7), 1);
    assert_eq!(gcd(90, 150), 30);
    assert_eq!(gcd(55, 25), 5);
}

#[test]
fn gcd_with_zero() {
    assert_eq!(gcd(12, 0), 12);
    assert_eq!(gcd(0, 7), 7);
}

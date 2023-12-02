use adventlib::grid::*;
use std::cmp::Ordering;
use std::collections::HashMap;
use std::collections::HashSet;
use std::f64;

pub fn solve() {
    println!("Day 10");

    let raw_map = adventlib::read_input_lines("day10input.txt");

    let asteroid_locations = get_asteroid_locations(&raw_map);
    let mut max_visible = 0;
    let mut max_location = Point::new(0, 0);

    for i in 0..asteroid_locations.len() {
        let observing_asteroid = asteroid_locations.get(i).unwrap();
        let mut cur_asteroid_angles = HashSet::new();
        for j in 0..asteroid_locations.len() {
            if i == j {
                continue; // don't count self
            }

            // Reference the angle to each asteroid as the smallest integral vector in its direction
            // to ensure asteroids in the same direction will have the same key.
            let candidate = asteroid_locations.get(j).unwrap();
            let raw_vector = candidate.vec_subtract(observing_asteroid);
            let reduced_vector = reduce_vector(raw_vector);
            cur_asteroid_angles.insert(reduced_vector);
        }

        let visible = cur_asteroid_angles.len();
        if visible > max_visible {
            max_visible = visible;
            max_location = *observing_asteroid;
        }

        cur_asteroid_angles.clear();
    }

    println!("Count from best location (part 1): {}", max_visible);

    let mut asteroids_by_direction =
        collect_asteroids_by_direction(&asteroid_locations, &max_location);

    let mut count_destroyed = 0;
    let mut last_destroyed = Point::new(0, 0);

    let mut clockwise_order: Vec<_> = asteroids_by_direction.keys().cloned().collect();
    clockwise_order.sort_by(|a, b| clockwise_order_from_up(a, b));

    for direction in clockwise_order.iter().cycle() {
        let asteroid_set = asteroids_by_direction.get_mut(direction).unwrap();

        if asteroid_set.is_empty() {
            continue;
        }

        let next_to_destroy = asteroid_set
            .iter()
            .min_by_key(|&a| a.manhattan_dist_to(&max_location))
            .unwrap()
            .clone();
        asteroid_set.remove(&next_to_destroy);
        count_destroyed += 1;
        last_destroyed = next_to_destroy;
        // dbg!(last_destroyed);

        if count_destroyed == 200 {
            break;
        }
    }

    println!(
        "200th asteroid destroyed (part 2): {}",
        last_destroyed.x * 100 + last_destroyed.y
    );
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

fn collect_asteroids_by_direction(
    asteroid_locations: &Vec<Point>,
    reference_point: &Point,
) -> HashMap<Point, HashSet<Point>> {
    let mut asteroids_by_direction = HashMap::new();

    for asteroid in asteroid_locations.iter() {
        let raw_vector = asteroid.vec_subtract(reference_point);

        if raw_vector.x == 0 && raw_vector.y == 0 {
            continue;
        }

        // Reference the angle to each asteroid as the smallest integral vector in its direction
        // to ensure asteroids in the same direction will end up in the same set.
        let reduced_vector = reduce_vector(raw_vector);
        let set_for_angle = asteroids_by_direction
            .entry(reduced_vector)
            .or_insert(HashSet::<Point>::new());
        set_for_angle.insert(*asteroid);
    }

    return asteroids_by_direction;
}

fn clockwise_order_from_up(a: &Point, b: &Point) -> Ordering {
    // Important: "up" in the problem is toward negative y, and
    // clockwise is in the direction from negative y toward positive x.
    // This corresponds to counter-clockwise rotation in a right-handed coordinate system.
    let angle_a = angle_from_negative_y_axis(a);
    let angle_b = angle_from_negative_y_axis(b);
    return angle_a.partial_cmp(&angle_b).unwrap();
}

fn angle_from_negative_y_axis(a: &Point) -> f64 {
    let mut angle = (a.y as f64).atan2(a.x as f64);

    // shift third quadrant to the end of the rotation
    if angle < -f64::consts::FRAC_PI_2 {
        angle += f64::consts::PI * 2.0;
    }

    // shift negative y-axis to zero (mostly for sanity)
    return angle + f64::consts::FRAC_PI_2;
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

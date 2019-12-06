use adventlib::grid::Point;
use std::collections::HashMap;

pub fn solve() {
    println!("Day 3");

    let lines = adventlib::read_input_lines("day03input.txt");

    assert_eq!(lines.len(), 2);

    let mut first_line_points = HashMap::new();
    let mut intersections = HashMap::new();

    let mut cur_x = 0;
    let mut cur_y = 0;
    let mut total_steps = 0;

    let first_line = &lines[0];
    for segment in first_line.split(',') {
        let direction = &segment[0..1];
        let mut dist = get_distance(&segment);
        while dist > 0 {
            match direction {
                "L" => cur_x -= 1,
                "R" => cur_x += 1,
                "U" => cur_y += 1,
                "D" => cur_y -= 1,
                _ => println!("unknown direction: {}", segment),
            }
            total_steps += 1;
            let cur_point = Point::new(cur_x, cur_y);
            if !first_line_points.contains_key(&cur_point) {
                first_line_points.insert(cur_point, total_steps);
            }
            dist -= 1;
        }
    }

    cur_x = 0;
    cur_y = 0;
    total_steps = 0;

    let second_line = &lines[1];
    for segment in second_line.split(',') {
        let direction = &segment[0..1];
        let mut dist = get_distance(&segment);
        while dist > 0 {
            match direction {
                "L" => cur_x -= 1,
                "R" => cur_x += 1,
                "U" => cur_y += 1,
                "D" => cur_y -= 1,
                _ => println!("unknown direction: {}", segment),
            }
            total_steps += 1;

            let cur_point = Point::new(cur_x, cur_y);
            if first_line_points.contains_key(&cur_point) && !intersections.contains_key(&cur_point)
            {
                let first_line_steps = first_line_points.get(&cur_point).unwrap();
                intersections.insert(cur_point, first_line_steps + total_steps);
            }
            dist -= 1;
        }
    }

    let origin = Point::new(0, 0);
    let closest_manhattan = intersections
        .iter()
        .map(|(p, _v)| p.manhattan_dist_to(&origin))
        .min()
        .unwrap();
    println!("Closest intersection (Manhattan): {}", closest_manhattan);

    let closest_length = intersections.values().min().unwrap();
    println!("Closest intersection (wire length): {}", closest_length);
}

fn get_distance(segment: &str) -> i32 {
    segment[1..].parse().unwrap()
}

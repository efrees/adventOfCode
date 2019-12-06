use std::collections::HashMap;
use std::collections::HashSet;

pub fn solve() {
    println!("Day 6");

    let lines = adventlib::read_input_lines("day06input.txt");
    let mut direct_edges = HashMap::new();
    let mut objects = HashSet::new();

    for line in lines.iter() {
        let nodes: Vec<_> = line.split(')').collect();
        direct_edges.insert(nodes[1], nodes[0]);
        objects.insert(nodes[1]);
    }

    let mut total_count = 0;

    for obj in objects {
        total_count += count_orbits_to_center(obj, &direct_edges);
    }

    println!("Total orbit count: {}", total_count);

    let our_start = direct_edges.get("YOU").unwrap();
    let his_start = direct_edges.get("SAN").unwrap();
    let distance = distance_between(our_start, his_start, &direct_edges);

    println!("Orbit transfers required: {}", distance);
}

fn count_orbits_to_center(start_object: &str, edges: &HashMap<&str, &str>) -> i32 {
    if edges.contains_key(start_object) {
        return 1 + count_orbits_to_center(edges.get(start_object).unwrap(), edges);
    }
    return 0;
}

fn distance_between(object1: &str, object2: &str, direct_edges: &HashMap<&str, &str>) -> i32 {
    let mut object1_distances = HashMap::new();
    object1_distances.insert(object1, 0);

    let mut searching_dist = 0;
    let mut searching_object = object1;
    while direct_edges.contains_key(searching_object) {
        let next_object = direct_edges.get(searching_object).unwrap();
        object1_distances.insert(next_object, searching_dist + 1);
        searching_object = next_object;
        searching_dist += 1;
    }

    searching_dist = 0;
    searching_object = object2;

    while !object1_distances.contains_key(searching_object) {
        searching_object = direct_edges.get(searching_object).unwrap();
        searching_dist += 1;
    }

    return object1_distances.get(searching_object).unwrap() + searching_dist;
}

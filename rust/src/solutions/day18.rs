use adventlib::grid::*;
use std::collections::*;

pub fn solve() {
    println!("Day 18");

    let raw_map = adventlib::read_input_lines("day18input.txt");

    let mut target_key_count = 0;
    let mut initial_position = None;
    let mut map = SparseGrid::new();
    for row in 0..raw_map.len() {
        let mut col = 0;
        for ch in raw_map[row].chars() {
            let cur_point = Point::new(col, row as i64);
            map.insert(cur_point, ch);

            if ch == '@' {
                initial_position = Some(cur_point);
            }

            if ch.is_ascii_lowercase() {
                target_key_count += 1;
            }

            col += 1;
        }
    }

    let total_steps = bfs_until_condition(
        &map,
        initial_position.unwrap(),
        &(|_, keys: &String| keys.len() == target_key_count),
    );

    //2796
    println!("Shortest tour (part 1): {}", total_steps);
}

fn bfs_until_condition(
    grid_state: &SparseGrid<char>,
    search_start: Point,
    stop_condition: &dyn (Fn(&Point, &String) -> bool),
) -> i32 {
    let mut search_nodes = vec![(search_start, 0, String::new())];
    let mut visited_with_keys = HashMap::new();

    let mut last_search_depth = -1;

    while search_nodes.len() > 0 {
        let (search_loc, search_depth, mut keys) = search_nodes.remove(0);

        let visited = visited_with_keys
            .entry(keys.clone())
            .or_insert(HashSet::new());

        if visited.contains(&search_loc) {
            continue;
        }

        visited.insert(search_loc);
        last_search_depth = search_depth;

        match grid_state.get(&search_loc) {
            Some(&c) => {
                if c.is_ascii_lowercase() && !keys.contains(c) {
                    let mut keys_vec: Vec<_> = keys.chars().collect();
                    keys_vec.push(c);
                    keys_vec.sort();
                    keys = keys_vec.iter().collect();
                }
            }
            _ => (),
        };

        if stop_condition(&search_loc, &keys) {
            break;
        }

        search_nodes.extend(
            search_loc
                .neighbors4()
                .iter()
                .filter(|&n| is_navigable(grid_state, n, &keys))
                .map(|&n| (n, search_depth + 1, keys.clone())),
        );
    }

    return last_search_depth;
}

fn is_navigable(grid_state: &SparseGrid<char>, location: &Point, keys: &String) -> bool {
    grid_state
        .get(location)
        .map(|&cell| {
            cell == '.'
                || cell == '@'
                || cell.is_ascii_lowercase()
                || keys.contains(ascii_to_lowercase(cell).unwrap_or(cell))
        })
        .unwrap_or(false)
}

fn ascii_to_lowercase(c: char) -> Option<char> {
    c.to_lowercase().nth(0)
}

#[test]
fn lowercase_an_a() {
    assert_eq!(ascii_to_lowercase('A'), Some('a'));
}

#[test]
fn door_navigable_with_key() {
    let mut grid = SparseGrid::new();
    let location = Point::new(1, 2);
    grid.insert(location, 'A');
    let keys = "abc".to_owned();

    assert!(is_navigable(&grid, &location, &keys));
}

#[test]
fn door_unnavigable_without_key() {
    let mut grid = SparseGrid::new();
    let location = Point::new(1, 2);
    grid.insert(location, 'A');
    let keys = "bc".to_owned();

    assert!(!is_navigable(&grid, &location, &keys));
}

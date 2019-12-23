pub fn solve() {
    println!("Day 16");

    let initial_state_raw = adventlib::read_input_raw("day16input.txt");
    let mut current_state: Vec<_> = initial_state_raw
        .trim()
        .as_bytes()
        .iter()
        .map(|&b| b - 48)
        .collect();

    for _phase in 1..=100 {
        current_state = compute_next_state(current_state);
    }

    let first_eight: String = current_state
        .iter()
        .take(8)
        .map(|&b| (b + 48) as char)
        .collect();
    println!("After 100 phases (part 1): {}", first_eight);
}

static BASE_SEQ: [i8; 4] = [0, 1, 0, -1];

fn compute_next_state(current_state: Vec<u8>) -> Vec<u8> {
    let mut next_state = Vec::with_capacity(current_state.len());
    for target_pos in 1..=current_state.len() {
        let target_value: i32 = current_state
            .iter()
            .enumerate()
            .map(|(i, &val)| {
                let base_seq_pos = ((i + 1) / target_pos) % 4;
                val as i32 * BASE_SEQ[base_seq_pos] as i32
            })
            .sum();

        next_state.push((target_value.abs() % 10) as u8);
    }

    return next_state;
}

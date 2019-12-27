pub fn solve() {
    println!("Day 16");

    let initial_state_raw = adventlib::read_input_raw("day16input.txt");
    let initial_state: Vec<_> = initial_state_raw
        .trim()
        .as_bytes()
        .iter()
        .map(|&b| b - 48)
        .collect();
    let mut current_state: Vec<_> = initial_state.iter().cloned().collect();

    for _phase in 1..=100 {
        current_state = compute_next_state_full(current_state);
    }

    let first_eight = get_string(&current_state, 0, 8);
    println!("After 100 phases (part 1): {}", first_eight);

    let original_len = current_state.len();
    let first_seven_str = get_string(&initial_state, 0, 7);
    let output_offset: usize = first_seven_str.parse().unwrap();

    let mut current_state: Vec<_> = initial_state
        .iter()
        .cloned()
        .cycle()
        .take(10000 * original_len)
        .skip(output_offset) // these will not contribute due to zeros in the base pattern
        .collect();
    let mut next_state = vec![0; current_state.len()];
    for _phase in 1..=100 {
        compute_next_state_second_half(&current_state, &mut next_state);
        let swap = current_state;
        current_state = next_state;
        next_state = swap;
    }

    let offset_eight = get_string(&current_state, 0, 8);
    println!("Signal repeated 10000 times (part 2): {}", offset_eight);
}

static BASE_SEQ: [i8; 4] = [0, 1, 0, -1];

fn compute_next_state_full(current_state: Vec<u8>) -> Vec<u8> {
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

fn compute_next_state_second_half(current_state: &Vec<u8>, next_state: &mut Vec<u8>) {
    // For a position N_target past the midpoint of the data, the only non-zero contributers
    // from the source are the contiguous sequence of elements from N_target to the end.
    //
    // These are all included by multiplication with +1 from the base pattern, so we can
    // also assume there are no subtractions and no reason to track more than one digit
    // while we incrementally build up an answer.
    let mut last_digit_of_sum = 0;
    for target_index in (0..current_state.len()).rev() {
        last_digit_of_sum = (last_digit_of_sum + current_state[target_index]) % 10;
        next_state[target_index] = last_digit_of_sum;
    }
}

fn get_string(state: &Vec<u8>, skip: usize, take: usize) -> String {
    state
        .iter()
        .skip(skip)
        .take(take)
        .map(|&b| (b + 48) as char)
        .collect()
}

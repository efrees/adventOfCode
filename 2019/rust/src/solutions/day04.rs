pub fn solve() {
    println!("Day 4");

    //input: 136760-595730
    let range_min = 136760;
    let range_max = 595730;

    assert!(digits_only_increase(111111));
    assert!(includes_doubled_digit(111111));
    assert!(digits_only_increase(123789));
    assert!(!includes_doubled_digit(123789));
    assert!(!digits_only_increase(223450));
    assert!(includes_doubled_digit(223450));

    let mut qualified_count_1 = 0;
    let mut qualified_count_2 = 0;
    for candidate in range_min..range_max + 1 {
        if digits_only_increase(candidate) && includes_doubled_digit(candidate) {
            qualified_count_1 += 1;
        }
        if digits_only_increase(candidate) && includes_exactly_doubled_digit(candidate) {
            qualified_count_2 += 1;
        }
    }

    println!("Qualified passwords (part 1): {}", qualified_count_1);
    println!("Qualified passwords (part 2): {}", qualified_count_2);
}

fn digits_only_increase(candidate: i32) -> bool {
    // in other words, they only *decrease* going backwards
    let mut last_digit = candidate % 10;
    let mut remaining_digits = candidate / 10;
    while remaining_digits > 0 {
        if (remaining_digits % 10) > last_digit {
            return false;
        }
        last_digit = remaining_digits % 10;
        remaining_digits /= 10;
    }

    return true;
}

fn includes_doubled_digit(candidate: i32) -> bool {
    // in other words, they only *decrease* going backwards
    let mut last_digit = candidate % 10;
    let mut remaining_digits = candidate / 10;
    while remaining_digits > 0 {
        if remaining_digits % 10 == last_digit {
            return true;
        }
        last_digit = remaining_digits % 10;
        remaining_digits /= 10;
    }

    return false;
}

fn includes_exactly_doubled_digit(candidate: i32) -> bool {
    // in other words, they only *decrease* going backwards
    let mut group_digit = candidate % 10;
    let mut group_count = 1;
    let mut remaining_digits = candidate / 10;
    while remaining_digits > 0 {
        if remaining_digits % 10 == group_digit {
            group_count += 1;
        } else {
            group_count = 1;
            group_digit = remaining_digits % 10;
        }

        remaining_digits /= 10;

        if group_count == 2 && remaining_digits % 10 != group_digit {
            return true;
        }
    }

    return false;
}

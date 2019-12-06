pub fn solve() {
    println!("Day 1");

    let lines = adventlib::read_input_lines("day01input.txt");

    let int_parser = |x: &String| x.parse::<i32>().unwrap();
    let total: i32 = lines.iter().map(int_parser).map(fuel_for_mass).sum();

    println!("Total fuel requirements (initial): {}", total);

    let total: i32 = lines
        .iter()
        .map(int_parser)
        .map(converged_fuel_for_mass)
        .sum();

    println!("Total fuel requirements (iterated): {}", total);
}

fn fuel_for_mass(mass: i32) -> i32 {
    mass / 3 - 2
}

fn converged_fuel_for_mass(mass: i32) -> i32 {
    let mut mass_including_fuel = mass;
    let mut next_fuel_mass = fuel_for_mass(mass);

    while next_fuel_mass > 0 {
        mass_including_fuel += next_fuel_mass;
        next_fuel_mass = fuel_for_mass(next_fuel_mass);
    }

    return mass_including_fuel - mass;
}

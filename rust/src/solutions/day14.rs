use regex::Regex;
use std::collections::HashMap;

#[derive(Eq, PartialEq, Hash)]
struct Chemical {
    count: u32,
    name: String,
}

pub fn solve() {
    println!("Day 14");

    let raw_reactions = adventlib::read_input_lines("day14input.txt");
    let reactions_list: Vec<_> = raw_reactions.iter().map(|m| parse_reaction(m)).collect();

    let reactions: HashMap<_, _> = reactions_list.iter().map(|(g, v)| (&*g.name, v)).collect();
    let mut reaction_output_count: HashMap<_, _> = reactions_list
        .iter()
        .map(|(g, _)| (&*g.name, g.count))
        .collect();
    let mut remnants_by_chemical: HashMap<_, u64> =
        reactions_list.iter().map(|(g, _)| (&*g.name, 0)).collect();

    reaction_output_count.insert("FUEL", 1);

    let mut ore_required = compute_total_ore_requirements(
        "FUEL",
        1,
        &reactions,
        &reaction_output_count,
        &mut remnants_by_chemical,
    );

    println!("ORE required for one FUEL (part 1): {}", ore_required);

    let ore_supply = 1_000_000_000_000_u64;
    // It's overly pessimistic to think we won't get any extra from all the remnants.
    // It's impossible that we'd end up producing less than supply / requirements_for_one.
    let mut potential_fuel_min = ore_supply / ore_required as u64;
    let mut potential_fuel_max = potential_fuel_min * 2;
    let mut search_interval = potential_fuel_min / 2;

    while search_interval > 0 {
        let next_to_check = potential_fuel_min + search_interval;

        ore_required = compute_total_ore_requirements(
            "FUEL",
            next_to_check,
            &reactions,
            &reaction_output_count,
            &mut remnants_by_chemical,
        );

        if ore_required > ore_supply {
            potential_fuel_max = next_to_check - 1;
        } else {
            potential_fuel_min = next_to_check;
        }

        search_interval = (potential_fuel_max - potential_fuel_min) / 2;
    }

    println!("Total FUEL possible (part 2): {}", potential_fuel_min);
}

fn compute_total_ore_requirements(
    name: &str,
    amount: u64,
    reactions: &HashMap<&str, &Vec<Chemical>>,
    reaction_outputs: &HashMap<&str, u32>,
    remnant_totals: &mut HashMap<&str, u64>,
) -> u64 {
    if name == "ORE" {
        return amount;
    }

    let reaction = reactions
        .get(name)
        .expect(&format!("Missing reaction for {}", name));

    let mut goal = amount;
    let mut remnant = remnant_totals.get(name).cloned().unwrap_or(0) as u64;

    goal -= std::cmp::min(remnant, amount);
    remnant -= std::cmp::min(remnant, amount);

    let mut requirement = 0;
    if goal > 0 {
        let recipe_output = reaction_outputs.get(&name).cloned().unwrap() as u64;
        let recipe_count = (goal + recipe_output - 1) / recipe_output;
        for input in reaction.iter() {
            requirement += compute_total_ore_requirements(
                &input.name,
                input.count as u64 * recipe_count,
                reactions,
                reaction_outputs,
                remnant_totals,
            );

            remnant = recipe_output * recipe_count - goal;
        }
    }

    // dodging lifetime issues that surface with .entry()
    *remnant_totals.get_mut(name).unwrap() = remnant;

    return requirement;
}

fn parse_reaction(raw_reaction: &String) -> (Chemical, Vec<Chemical>) {
    let sides_of_equation: Vec<_> = raw_reaction.split(" => ").collect();
    let result = parse_chemical(sides_of_equation[1]);
    let inputs: Vec<_> = sides_of_equation[0]
        .split(", ")
        .map(|raw| parse_chemical(raw))
        .collect();

    return (result, inputs);
}

fn parse_chemical(raw_chemical: &str) -> Chemical {
    lazy_static! {
        static ref PATTERN: Regex = Regex::new(r"\s*(\d+) (\w+)").expect("pattern for parsing");
    }

    let captures = PATTERN
        .captures(raw_chemical)
        .expect("Line should match format");

    return Chemical {
        count: captures[1]
            .parse()
            .expect("First part of chemical must be a number."),
        name: captures[2].to_string(),
    };
}

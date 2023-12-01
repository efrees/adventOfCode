use std::collections::HashMap;

use adventlib;
use regex::Regex;

pub fn solve() {
    println!("Day 24");

    let mut teams: Vec<_> = adventlib::read_input_raw("day24input.txt")
        .split("\r\n\r\n")
        .flat_map(|raw| raw.split("\n\n"))
        .map(|s| parse_team(s))
        .collect();

    assert_eq!(teams.len(), 2);

    sequence_groups(&mut teams);

    while teams[0].groups.iter().any(|g| g.unit_count > 0)
        && teams[1].groups.iter().any(|g| g.unit_count > 0)
    {
        let mut initiative_order: Vec<_> = teams.iter_mut().flat_map(|t| &mut t.groups).collect();
        initiative_order.sort_by_key(|g| std::cmp::Reverse(g.initiative));

        let mut selection_order: Vec<_> = initiative_order
            .iter()
            .filter(|g| g.unit_count > 0)
            .collect();
        selection_order.sort_by_key(|g| (g.attack_damage * g.unit_count, g.initiative));
        selection_order.reverse();

        let mut selected_targets = HashMap::new();
        for attacking_group in &selection_order {
            let mut max_potential_damage = 0;
            let mut seq_num_of_max = 0;
            let mut eff_pow_of_max = 0;
            let mut initiative_of_max = 0;

            for target_group in &selection_order {
                if attacking_group.team_name == target_group.team_name
                    || selected_targets
                        .values()
                        .any(|&v| v == target_group.seq_num)
                {
                    continue;
                }

                let potential_damage = get_potential_attack_damage(attacking_group, target_group);
                let eff_pow_of_target = target_group.unit_count * target_group.attack_damage;

                if potential_damage > max_potential_damage
                    || (potential_damage == max_potential_damage
                        && eff_pow_of_target > eff_pow_of_max)
                    || (potential_damage == max_potential_damage
                        && eff_pow_of_target == eff_pow_of_max
                        && target_group.initiative > initiative_of_max)
                {
                    max_potential_damage = potential_damage;
                    seq_num_of_max = target_group.seq_num;
                    eff_pow_of_max = eff_pow_of_target;
                    initiative_of_max = target_group.initiative;
                }
            }

            if max_potential_damage > 0 {
                selected_targets.insert(attacking_group.seq_num, seq_num_of_max);
            }
        }

        for i in 0..initiative_order.len() {
            let attacking_group = &initiative_order[i];
            if attacking_group.unit_count == 0
                || !selected_targets.contains_key(&attacking_group.seq_num)
            {
                continue;
            }

            let target_group_index = initiative_order
                .iter()
                .position(|g| Some(&g.seq_num) == selected_targets.get(&attacking_group.seq_num))
                .unwrap();

            let potential_damage =
                get_potential_attack_damage(attacking_group, &initiative_order[target_group_index]);

            let units_destroyed =
                potential_damage / initiative_order[target_group_index].hit_points;

            initiative_order[target_group_index].unit_count =
                if units_destroyed > initiative_order[target_group_index].unit_count {
                    0
                } else {
                    initiative_order[target_group_index].unit_count - units_destroyed
                }
        }
    }

    // < 15956
    let winning_unit_count: u32 = teams
        .iter()
        .flat_map(|t| &t.groups)
        .map(|g| g.unit_count)
        .sum();
    println!("Winning unit count: {}", winning_unit_count);
}

fn sequence_groups(teams: &mut Vec<Team>) {
    let mut seq_num = 0;
    for group in teams[0].groups.iter_mut() {
        group.seq_num = seq_num;
        seq_num += 1;
    }
    for group in teams[1].groups.iter_mut() {
        group.seq_num = seq_num;
        seq_num += 1;
    }
}

fn get_potential_attack_damage(attacking_group: &FightGroup, target_group: &FightGroup) -> u32 {
    let mut potential_damage = if target_group
        .immunities
        .iter()
        .any(|i| *i == attacking_group.attack_type)
    {
        0
    } else {
        attacking_group.attack_damage * attacking_group.unit_count
    };
    if target_group
        .weaknesses
        .iter()
        .any(|w| *w == attacking_group.attack_type)
    {
        potential_damage *= 2;
    }
    potential_damage
}

fn parse_team(team_raw: &str) -> Team {
    let lines: Vec<_> = team_raw.lines().collect();
    let team_name = lines[0].to_string();
    let groups = lines
        .iter()
        .skip(1)
        .map(|s| parse_group(s, &team_name))
        .collect();

    Team { team_name, groups }
}

fn parse_group(group_raw: &str, team_name: &str) -> FightGroup {
    lazy_static! {
        static ref PATTERN: Regex =
            Regex::new(r"(\d+) units each with (\d+) hit points.* with an attack that does (\d+) (\w+) damage at initiative (\d+)")
            .expect("Parse pattern");
    }
    let captures = PATTERN
        .captures(group_raw)
        .expect("Line should match format");
    let unit_count: u32 = captures[1].parse().unwrap();
    let hit_points: u32 = captures[2].parse().unwrap();
    let attack_damage: u32 = captures[3].parse().unwrap();
    let attack_type = captures[4].to_string();
    let initiative: u32 = captures[5].parse().unwrap();

    let weaknesses_pattern: Regex =
        Regex::new(r"weak to (?:(\w+), )*(\w+)[;\)]").expect("Weakness pattern");
    let immunities_pattern: Regex =
        Regex::new(r"immune to (?:(\w+), )*(\w+)[;\)]").expect("Immunity pattern");
    let weaknesses = get_captures_as_strings(weaknesses_pattern, group_raw);
    let immunities = get_captures_as_strings(immunities_pattern, group_raw);
    FightGroup {
        seq_num: 0,
        team_name: team_name.to_string(),
        unit_count,
        hit_points,
        attack_damage,
        attack_type,
        initiative,
        weaknesses,
        immunities,
    }
}

fn get_captures_as_strings(pattern: Regex, line: &str) -> Vec<String> {
    if let Some(caps) = pattern.captures(line) {
        caps.iter()
            .skip(1)
            .filter_map(|c| c.and_then(|v| Some(v.as_str().to_string())))
            .collect()
    } else {
        vec![]
    }
}

struct Team {
    team_name: String,
    groups: Vec<FightGroup>,
}

struct FightGroup {
    seq_num: usize,
    team_name: String,
    unit_count: u32,
    hit_points: u32,
    attack_damage: u32,
    attack_type: String,
    initiative: u32,
    weaknesses: Vec<String>,
    immunities: Vec<String>,
}

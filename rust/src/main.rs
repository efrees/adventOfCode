extern crate adventlib;
extern crate chrono;
extern crate regex;
#[macro_use]
extern crate lazy_static;

use std::time::Instant;

mod solutions;
use solutions::*;

mod intcode;

fn main() {
    time_with_label(&solve_all, "Total time: ");
}

fn solve_all() {
    time(&day01::solve);
    time(&day02::solve);
    time(&day03::solve);
    time(&day04::solve);
    time(&day05::solve);
    time(&day06::solve);
    time(&day07::solve);
    time(&day08::solve);
    time(&day09::solve);
    time(&day10::solve);
    time(&day11::solve);
    time(&day12::solve);
    time(&day13::solve);
}

fn time(f: &dyn Fn()) {
    time_with_label(f, "Solved in");
}

fn time_with_label(f: &dyn Fn(), label: &str) {
    let now = Instant::now();
    f();
    let duration = now.elapsed();
    println!(
        "{} {}.{:09}s\n",
        label,
        duration.as_secs(),
        duration.subsec_nanos()
    );
}

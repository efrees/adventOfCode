pub struct Computer<T> {
    program_state: Vec<T>,
    instr_ptr: usize,
    rel_base: usize,
    input_stream: Vec<T>,
    pub run_state: RunState,
}

#[derive(PartialEq, Debug, Clone, Copy)]
pub enum RunState {
    Initial,
    Running,
    Halted,
    Waiting,
}

pub fn parse_program(raw_program: &String) -> Vec<i64> {
    let int_parser = |x: &str| x.parse::<i64>().unwrap();
    return raw_program.trim().split(',').map(int_parser).collect();
}

impl Computer<i64> {
    pub fn new() -> Computer<i64> {
        Computer {
            program_state: vec![99],
            instr_ptr: 0,
            rel_base: 0,
            input_stream: vec![],
            run_state: RunState::Initial,
        }
    }

    pub fn for_program(program_state: Vec<i64>) -> Computer<i64> {
        Computer {
            program_state: program_state,
            instr_ptr: 0,
            rel_base: 0,
            input_stream: vec![],
            run_state: RunState::Initial,
        }
    }

    pub fn for_raw_program(raw_program: &String) -> Computer<i64> {
        let program = parse_program(raw_program);
        Computer::for_program(program)
    }

    pub fn load_program(&mut self, program_state: Vec<i64>) {
        self.program_state = program_state;
        self.instr_ptr = 0;
    }

    pub fn set_input_stream(&mut self, input_stream: Vec<i64>) {
        self.input_stream = input_stream;
    }

    pub fn add_input(&mut self, additional_input: i64) {
        self.input_stream.push(additional_input);
    }

    pub fn set_noun_and_verb(&mut self, noun: i64, verb: i64) {
        self.program_state[1] = noun;
        self.program_state[2] = verb;
    }

    pub fn get_value_at_zero(&self) -> i64 {
        return self.program_state[0];
    }

    /* Runs the loaded program and returns the value left in location 0 */
    pub fn run_program(&mut self) -> Option<i64> {
        return self.run_program_with_output(&mut vec![]);
    }

    /* Runs the loaded program and returns the value left in location 0 */
    pub fn run_program_with_output(&mut self, output_stream: &mut Vec<i64>) -> Option<i64> {
        self.run_state = RunState::Running;
        while self.run_state == RunState::Running {
            self.execute_next(output_stream);
        }

        // println!("Diagnostic output: {:#?}", output_stream);
        return output_stream.last().cloned();
    }

    fn execute_next(&mut self, output_stream: &mut Vec<i64>) {
        let instruction_code = self.program_state[self.instr_ptr];
        let op_code = instruction_code % 100;
        let param_modes = instruction_code / 100;
        match op_code {
            1 => self.add(param_modes),
            2 => self.mul(param_modes),
            3 => self.input(param_modes),
            4 => self.output(param_modes, output_stream),
            5 => self.jump_true(param_modes),
            6 => self.jump_false(param_modes),
            7 => self.lt(param_modes),
            8 => self.eq(param_modes),
            9 => self.rel_shift(param_modes),
            99 => self.run_state = RunState::Halted,
            op => println!("Unknown opcode: {}", op),
        };

        if self.instr_ptr >= self.program_state.len() {
            println!("out of bounds: {}", self.instr_ptr);
        }
    }

    fn add(&mut self, param_modes: i64) {
        let operands = self.get_operands(param_modes, 2);
        let result_operand = self.get_param(self.instr_ptr + 3, 1);
        let result_val = operands[0] + operands[1];

        let write_mode = Self::get_param_mode(param_modes, 3);
        self.write_safe_with_mode(result_operand, result_val, write_mode);

        self.instr_ptr += 4;
    }

    fn mul(&mut self, param_modes: i64) {
        let operands = self.get_operands(param_modes, 2);
        let result_operand = self.get_param(self.instr_ptr + 3, 1);
        let result_val = operands[0] * operands[1];

        let write_mode = Self::get_param_mode(param_modes, 3);
        self.write_safe_with_mode(result_operand, result_val, write_mode);

        self.instr_ptr += 4;
    }

    fn lt(&mut self, param_modes: i64) {
        let operands = self.get_operands(param_modes, 2);
        let result_operand = self.get_param(self.instr_ptr + 3, 1);
        let result_val = if operands[0] < operands[1] { 1 } else { 0 };

        let write_mode = Self::get_param_mode(param_modes, 3);
        self.write_safe_with_mode(result_operand, result_val, write_mode);

        self.instr_ptr += 4;
    }

    fn eq(&mut self, param_modes: i64) {
        let operands = self.get_operands(param_modes, 2);
        let result_operand = self.get_param(self.instr_ptr + 3, 1);
        let result_val = if operands[0] == operands[1] { 1 } else { 0 };

        let write_mode = Self::get_param_mode(param_modes, 3);
        self.write_safe_with_mode(result_operand, result_val, write_mode);

        self.instr_ptr += 4;
    }

    fn input(&mut self, param_modes: i64) {
        if self.input_stream.is_empty() {
            self.run_state = RunState::Waiting;
            return;
        }

        let result_operand = self.get_param(self.instr_ptr + 1, 1);
        let input_val = self.input_stream.remove(0);

        let write_mode = Self::get_param_mode(param_modes, 1);
        self.write_safe_with_mode(result_operand, input_val, write_mode);

        self.instr_ptr += 2;
    }

    fn output(&mut self, param_modes: i64, output_stream: &mut Vec<i64>) {
        let operands = self.get_operands(param_modes, 1);
        output_stream.push(operands[0]);

        self.instr_ptr += 2;
    }

    fn jump_true(&mut self, param_modes: i64) {
        let operands = self.get_operands(param_modes, 2);

        if operands[0] != 0 {
            self.instr_ptr = operands[1] as usize;
        } else {
            self.instr_ptr += 3;
        }
    }

    fn jump_false(&mut self, param_modes: i64) {
        let operands = self.get_operands(param_modes, 2);

        if operands[0] == 0 {
            self.instr_ptr = operands[1] as usize;
        } else {
            self.instr_ptr += 3;
        }
    }

    fn rel_shift(&mut self, param_modes: i64) {
        let operands = self.get_operands(param_modes, 1);
        let new_base = self.rel_base as i64 + operands[0];
        self.rel_base = new_base as usize;
        self.instr_ptr += 2;
    }

    fn get_operands(&mut self, param_modes: i64, operand_count: u8) -> Vec<i64> {
        let mut moded_operands = vec![];
        for i in 1..operand_count + 1 {
            let pointer = self.instr_ptr + (i as usize);
            let param_mode = Self::get_param_mode(param_modes, i as u8);
            moded_operands.push(self.get_param(pointer, param_mode));
        }
        return moded_operands;
    }

    fn get_param(&mut self, pointer: usize, param_mode: u8) -> i64 {
        let raw_operand = self.get_safe(pointer);
        if param_mode == 1 {
            return raw_operand;
        } else if param_mode == 2 {
            let absolute_position = self.rel_base as i64 + raw_operand;
            return self.get_safe(absolute_position as usize);
        } else {
            return self.get_safe(raw_operand as usize);
        }
    }

    /** op_number should be the 1-based position of the desired operand.
     *  These are read right-to-left from the modes value.
     */
    fn get_param_mode(param_modes: i64, op_number: u8) -> u8 {
        let mut modes = param_modes;
        for _ in 1..op_number {
            modes /= 10;
        }
        return (modes % 10) as u8;
    }

    fn write_safe_with_mode(&mut self, loc: i64, val: i64, loc_mode: u8) {
        let write_pos = if loc_mode == 2 {
            loc + self.rel_base as i64
        } else {
            loc
        };

        self.write_safe(write_pos as usize, val);
    }

    fn write_safe(&mut self, write_pos: usize, val: i64) {
        if write_pos >= self.program_state.len() {
            let additional_size = write_pos + 1 - self.program_state.len();
            self.program_state.extend(vec![0; additional_size].iter());
        }
        self.program_state[write_pos] = val;
    }

    fn get_safe(&self, loc: usize) -> i64 {
        if loc >= self.program_state.len() {
            return 0;
        }
        return self.program_state[loc];
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_param_mode() {
        assert_eq!(Computer::get_param_mode(10, 2), 1);
        assert_eq!(Computer::get_param_mode(1, 1), 1);
        assert_eq!(Computer::get_param_mode(1, 2), 0);
        assert_eq!(Computer::get_param_mode(10, 1), 0);
    }

    #[test]
    fn test_mem_expansion() {
        let program = vec![1101, 5, 7, 100, 4, 100, 99];
        let mut computer = Computer::for_program(program);
        let mut output = Vec::new();
        computer.run_program_with_output(&mut output);

        assert_eq!(*output.last().unwrap(), 12);
    }

    #[test]
    fn test_input() {
        let program = vec![3, 3, 104, -1, 99];
        let mut computer = Computer::for_program(program);
        let mut output = Vec::new();
        let expected_val = 23;
        computer.set_input_stream(vec![expected_val]);
        computer.run_program_with_output(&mut output);

        assert_eq!(*output.last().unwrap(), expected_val);
    }

    #[test]
    fn test_rel_input() {
        let program = vec![109, 2, 203, 3, 104, -1, 99];
        let mut computer = Computer::for_program(program);
        let mut output = Vec::new();
        let expected_val = 23;
        computer.set_input_stream(vec![expected_val]);
        computer.run_program_with_output(&mut output);

        assert_eq!(*output.last().unwrap(), expected_val);
    }
}

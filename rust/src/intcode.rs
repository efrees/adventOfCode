pub struct Computer {
    program_state: Vec<i32>,
    instr_ptr: usize,
    input_stream: Vec<i32>,
    pub run_state: RunState,
}

#[derive(PartialEq, Debug, Clone, Copy)]
pub enum RunState {
    Initial,
    Running,
    Halted,
    Waiting,
}

impl Computer {
    pub fn new() -> Computer {
        Computer {
            program_state: vec![99],
            instr_ptr: 0,
            input_stream: vec![],
            run_state: RunState::Initial,
        }
    }

    pub fn for_program(program_state: Vec<i32>) -> Computer {
        Computer {
            program_state: program_state,
            instr_ptr: 0,
            input_stream: vec![],
            run_state: RunState::Initial,
        }
    }

    pub fn load_program(&mut self, program_state: Vec<i32>) {
        self.program_state = program_state;
        self.instr_ptr = 0;
    }

    pub fn set_input_stream(&mut self, input_stream: Vec<i32>) {
        self.input_stream = input_stream;
    }

    pub fn set_noun_and_verb(&mut self, noun: i32, verb: i32) {
        self.program_state[1] = noun;
        self.program_state[2] = verb;
    }

    /* Runs the loaded program and returns the value left in location 0 */
    pub fn run_program(&mut self) -> i32 {
        return self.run_program_with_output(&mut vec![]);
    }

    /* Runs the loaded program and returns the value left in location 0 */
    pub fn run_program_with_output(&mut self, output_stream: &mut Vec<i32>) -> i32 {
        self.run_state = RunState::Running;
        while self.run_state == RunState::Running {
            self.execute_next(output_stream);
        }

        return self.program_state[0];
    }

    fn execute_next(&mut self, output_stream: &mut Vec<i32>) {
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
            99 => self.run_state = RunState::Halted,
            op => println!("Unknown opcode: {}", op),
        };

        if self.instr_ptr >= self.program_state.len() {
            println!("out of bounds: {}", self.instr_ptr);
        }
    }

    fn add(&mut self, param_modes: i32) {
        let operands = self.get_operands(param_modes, 2);
        let result_operand = self.get_param(self.instr_ptr + 3, 1);
        self.program_state[result_operand as usize] = operands[0] + operands[1];

        self.instr_ptr += 4;
    }

    fn mul(&mut self, param_modes: i32) {
        let operands = self.get_operands(param_modes, 2);
        let result_operand = self.get_param(self.instr_ptr + 3, 1);
        self.program_state[result_operand as usize] = operands[0] * operands[1];

        self.instr_ptr += 4;
    }

    fn lt(&mut self, param_modes: i32) {
        let operands = self.get_operands(param_modes, 2);
        let result_operand = self.get_param(self.instr_ptr + 3, 1);
        self.program_state[result_operand as usize] = if operands[0] < operands[1] { 1 } else { 0 };

        self.instr_ptr += 4;
    }

    fn eq(&mut self, param_modes: i32) {
        let operands = self.get_operands(param_modes, 2);
        let result_operand = self.get_param(self.instr_ptr + 3, 1);
        self.program_state[result_operand as usize] =
            if operands[0] == operands[1] { 1 } else { 0 };

        self.instr_ptr += 4;
    }

    fn input(&mut self, _param_modes: i32) {
        if self.input_stream.is_empty() {
            self.run_state = RunState::Waiting;
            return;
        }

        let result_operand = self.get_param(self.instr_ptr + 1, 1);
        self.program_state[result_operand as usize] = self.input_stream.remove(0);

        self.instr_ptr += 2;
    }

    fn output(&mut self, param_modes: i32, output_stream: &mut Vec<i32>) {
        let operands = self.get_operands(param_modes, 1);
        output_stream.push(operands[0]);

        self.instr_ptr += 2;
    }

    fn jump_true(&mut self, param_modes: i32) {
        let operands = self.get_operands(param_modes, 2);

        if operands[0] != 0 {
            self.instr_ptr = operands[1] as usize;
        } else {
            self.instr_ptr += 3;
        }
    }

    fn jump_false(&mut self, param_modes: i32) {
        let operands = self.get_operands(param_modes, 2);

        if operands[0] == 0 {
            self.instr_ptr = operands[1] as usize;
        } else {
            self.instr_ptr += 3;
        }
    }

    fn get_operands(&mut self, param_modes: i32, operand_count: u8) -> Vec<i32> {
        let mut moded_operands = vec![];
        for i in 1..operand_count + 1 {
            let pointer = self.instr_ptr + (i as usize);
            let param_mode = Self::get_param_mode(param_modes, i as u8);
            moded_operands.push(self.get_param(pointer, param_mode));
        }
        return moded_operands;
    }

    fn get_param(&mut self, pointer: usize, param_mode: i32) -> i32 {
        let raw_operand = self.program_state[pointer];
        if param_mode == 1 {
            return raw_operand;
        } else {
            return self.program_state[raw_operand as usize];
        }
    }

    /** op_number should be the 1-based position of the desired operand.
     *  These are read right-to-left from the modes value.
     */
    fn get_param_mode(param_modes: i32, op_number: u8) -> i32 {
        let mut modes = param_modes;
        for _ in 1..op_number {
            modes /= 10;
        }
        return modes % 10;
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
}

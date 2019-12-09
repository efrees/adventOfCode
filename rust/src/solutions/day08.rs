pub fn solve() {
    println!("Day 8");

    let image_data = adventlib::read_input_raw("day08input.txt");

    let image_w = 25;
    let image_h = 6;
    let layer_size = image_w * image_h;
    let mut result_image: Vec<char> = std::iter::repeat('2').take(layer_size).collect();

    let layers = image_data.len() / layer_size;
    let mut min_zeros = 9999;
    let mut checksum = 9999;
    for l in 0..layers {
        let layer_data = &image_data[l * layer_size + 0..l * layer_size + layer_size];
        let mut zeros = 0;
        let mut ones = 0;
        let mut twos = 0;
        for (i, c) in layer_data.char_indices() {
            match c {
                '0' => zeros += 1,
                '1' => ones += 1,
                '2' => twos += 1,
                _ => (),
            };

            if result_image[i] == '2' && c != '2' {
                result_image[i] = c;
            }
        }
        if zeros < min_zeros {
            min_zeros = zeros;
            checksum = ones * twos;
        }
    }

    println!("Checksum (part 1): {}", checksum);

    println!("Image (part 2):");
    let result_string: String = result_image
        .iter()
        .map(|&c| if c == '0' { ' ' } else { c })
        .collect();
    for i in 0..image_h {
        let row = &result_string[i * image_w..(i + 1) * image_w];
        println!("{}", row);
    }
}

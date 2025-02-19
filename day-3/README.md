# Day 3 - Raspberry Pi Pico Blink (Rust Version)

This project is the Rust implementation of Day 2's C version of the blinking LED program for the Raspberry Pi Pico.

## Prerequisites (Arch Linux)

Install required packages:
```bash
sudo pacman -S arm-none-eabi-gcc arm-none-eabi-newlib rustup
rustup default stable
rustup target add thumbv6m-none-eabi
```

## Setup

1. Clone the repository
2. Create the following structure:
   ```
   pico-blink/
   ├── .cargo/
   │   └── config.toml
   ├── src/
   │   └── main.rs
   └── Cargo.toml
   ```

3. Build the project:
   ```bash
   cargo build --release
   ```

4. Connect your Pico while holding the BOOTSEL button
5. Copy the generated UF2 file:
   ```bash
   cp target/thumbv6m-none-eabi/release/pico-blink.uf2 /run/media/$USER/RPI-RP2/
   ```

## What it Does

Blinks the onboard LED (GPIO 25) at 1Hz (500ms on, 500ms off).

## License

MIT
# ARM Assembly on Raspberry Pi Pico
## LED Blinker Project ( blink_minimal )

This project is part of my #100DaysOfCode challenge, focusing on bare metal programming using ARM assembly for the Raspberry Pi Pico microcontroller.

### Project Description
A simple LED blinker program written in ARM Cortex-M0+ assembly language. It demonstrates basic GPIO control and timing delays without using the SDK, working directly with the hardware registers.

### Prerequisites

Install the following packages on Arch Linux:
```bash
# Core development tools
sudo pacman -S base-devel git cmake

# ARM toolchain
sudo pacman -S arm-none-eabi-gcc arm-none-eabi-newlib

# Raspberry Pi Pico SDK and tools
sudo pacman -S pico-sdk picotool
```

### Project Structure
- `blink.s` - Main assembly source file containing the LED blink logic
- `boot_stage2.ld` - Linker script that defines memory layout
- `Makefile` - Build configuration file

### File Details

#### blink.s
This file contains the assembly code that:
- Sets up the vector table
- Configures GPIO pin 25 (built-in LED)
- Implements a simple delay routine
- Blinks the LED in an infinite loop

#### boot_stage2.ld
Linker script that:
- Defines the memory layout for our program
- Places code in the correct memory region (SRAM at 0x20040000)
- Ensures proper alignment and section ordering

#### Makefile
Build configuration that:
- Sets up compilation flags and tools
- Defines build targets
- Handles conversion to UF2 format for Pico upload

### Building the Project

1. Clone the repository:
```bash
git clone <your-repo-url>
cd <project-directory>
```

2. Compile the project:
```bash
make
```

This will generate several files:
- `blink.o` - Object file
- `blink.elf` - ELF binary
- `blink.bin` - Raw binary
- `blink.uf2` - UF2 format for Pico upload
- `blink.list` - Disassembly listing (useful for debugging)

### Uploading to Raspberry Pi Pico

1. Hold the BOOTSEL button on your Pico
2. While holding BOOTSEL, connect the Pico to your computer via USB
3. Release BOOTSEL - the Pico should mount as a mass storage device
4. Copy the generated UF2 file to the Pico:
```bash
cp blink.uf2 /run/media/$USER/RPI-RP2/
```
The Pico will automatically restart and run the program.

### Debugging

- Check the generated `blink.list` file to verify the assembly and memory layout
- Use a logic analyzer or oscilloscope on GPIO25 to verify timing
- LED should blink with approximately 1-second period (500ms on, 500ms off)

### Common Issues

1. If `make` fails with permission errors:
```bash
sudo chown -R $USER:$USER /usr/share/pico-sdk
```

2. If Pico is not detected:
- Ensure udev rules are set up:
```bash
sudo tee /etc/udev/rules.d/99-pico.rules << EOF
SUBSYSTEM=="usb", ATTRS{idVendor}=="2e8a", ATTRS{idProduct}=="0003", MODE="666"
EOF
sudo udevadm control --reload-rules
```

3. If the LED doesn't blink:
- Verify your solder joints if using an external LED
- Check that you're using GPIO25 (the built-in LED)
- Verify the UF2 file was copied successfully

### Understanding the Code

The key memory-mapped I/O addresses used:
- `0x4000f000` - SIO base address
- `0x400140cc` - GPIO configuration
- `0xd0000020` - GPIO output clear
- `0xd000001c` - GPIO output set

The program:
1. Initializes the stack pointer
2. Configures GPIO25 for output
3. Enters an infinite loop that:
   - Sets the LED on
   - Delays
   - Sets the LED off
   - Delays
   - Repeats

### Next Steps

Possible enhancements:
1. Add more complex blinking patterns
2. Implement multiple LED control
3. Add button input handling
4. Optimize the delay routine
5. Add power-saving features

### Resources

- [ARM Cortex-M0+ Technical Reference Manual](https://developer.arm.com/documentation/ddi0484/latest/)
- [Raspberry Pi Pico Datasheet](https://datasheets.raspberrypi.com/pico/pico-datasheet.pdf)
- [ARM ASM Documentation](https://developer.arm.com/documentation/dui0041/c/ARM-Instruction-Reference)

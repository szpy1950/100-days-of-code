NAME    = blink
CPU     = cortex-m0plus
ARMGNU  = arm-none-eabi
AFLAGS  = --warn --fatal-warnings -mcpu=$(CPU) -g
LDFLAGS = -nostdlib
PICOSDK = ~/pico/pico-sdk
PICOTOOL = /usr/bin

all: $(NAME).bin

$(NAME).o: $(NAME).s
	$(ARMGNU)-as $(AFLAGS) $(NAME).s -o $(NAME).o

$(NAME).bin: boot_stage2.ld $(NAME).s $(NAME).o
	$(ARMGNU)-ld $(LDFLAGS) --entry 0x20040001 -T boot_stage2.ld $(NAME).o -o $(NAME).elf
	$(ARMGNU)-objdump -D $(NAME).elf > $(NAME).list
	$(ARMGNU)-objcopy -O binary $(NAME).elf $(NAME).bin
	$(PICOTOOL)/picotool uf2 convert $(NAME).elf $(NAME).uf2

clean:
	rm -f *.bin *.o *.elf *.list *.uf2
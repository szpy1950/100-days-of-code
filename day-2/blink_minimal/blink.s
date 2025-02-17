;@ Copyright (c) 2023 CarlosFTM
;@ This code is licensed under MIT license (see LICENSE.txt for details)

	.cpu cortex-m0plus
	.thumb

	.section .vectors, "ax"
	.align 2
	.global _vectors
_vectors:
	.word 0x20001000
	.word _reset

	.thumb_func
	.global _reset
_reset:
	ldr r0, =0x20001000
	mov sp, r0

	ldr  r3, =0x4000f000
	movs r2, #32
	str  r2, [r3, #0]

	ldr  r3, =0x400140cc
	movs r2, #5
	str  r2, [r3, #0]

	ldr  r3, =0xd0000020
	movs r2, #128
	lsl  r2, r2, #18
	str  r2, [r3, #0]

_blink:
    ldr  r3, =0xd000001c
    movs r2, #128
    lsl  r2, r2, #18
    str  r2, [r3, #0]

    ldr r1, = up
    ldr r5, [r1]

    ldr r1, =counter
    ldr r2, [r1]
    add r2, #1
    cmp r2, #20

    be invert_up

invert_up:
    eor r5, #1
    str r5, [r1]


    bls counter_ok
    movs r2, #0
counter_ok:
    str r2, [r1]

    movs r3, #100
    lsl r3, #10
    mul r3, r2
    ldr r1, =delay_value
    ldr r0, [r1]

    ldr r1, = up
    ldr r5, [r1]
    cmp r5, #1

    beq is_up

is_up:
    add r0, r3
    bls add_ok
is_down:
    sub r0, r3

add_ok:
    bl  _delay

    b   _blink

.thumb_func
_delay:
	mov r4,r0
_loop:
	sub r4,r4,#1
	cmp r4, #0
	bne _loop
	bx  lr

    .section .data
    .align 4

delay_value:
    .word 100000
counter:
    .word 0
up:
    .word 1

.align 4

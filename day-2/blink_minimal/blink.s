
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

	ldr r0, = 100000
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

.align 4

NEC Infrared Transmission Protocol
The NEC IR transmission protocol uses pulse distance encoding of the message bits. Each pulse burst (mark – RC transmitter ON) is 562.5µs in length, at a carrier frequency of 38kHz (26.3µs). Logical bits are transmitted as follows:
• Logical '0' – a 562.5µs pulse burst followed by a 562.5µs space, with a total transmit time of 1.125ms
• Logical '1' – a 562.5µs pulse burst followed by a 1.6875ms space, with a total transmit time of 2.25ms
When a key is pressed on the remote controller, the message transmitted consists of the following, in order:
• a 9ms leading pulse burst (16 times the pulse burst length used for a logical data bit)
• a 4.5ms space
• the 8-bit address for the receiving device
• the 8-bit logical inverse of the address
• the 8-bit command
• the 8-bit logical inverse of the command
• a final 562.5µs pulse burst to signify the end of message transmission.
The four bytes of data bits are each sent least significant bit first. Figure 4 illustrates the format of an NEC IR transmission frame, for an address of 00h (00000000b) and a command of ADh (10101101b).


http://www.altium.com/files/altiumdesigner/s08/learningguides/cr0183%20wb_ircoder%20infrared%20encoder.pdf

http://www.sbprojects.com/knowledge/ir/nec.php


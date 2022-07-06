# Batch2OS
A fun and dumb project made in C# that compiles batch code into native x86 code

![img.png](img.png)

# Compile
First, you must build Batch2OS. After that, create a batch file with some code in it, then execute this command to get a native binary:</br>
``Batch2OS <file>.bat -o kernel.img -b 0x7C00 -l 0x1000``<br/>
You can now run the OS on a virtual hypervisor like QEMU:<br/>
``qemu-system-x86_64 -hdd kernel.img``<br/>
Or just run it on bare metal!<br/>
Note: You can also get help by doing ``Batch2OS -h``.

# Compatibility
Batch2OS emits x86 code that is theoretically compatible starting from the Intel 8086. That is because Batch2OS currently uses real mode BIOS interrupts, which are 16-bit and were introduced back in the 8086.</br>

# Supported commands
- ``help``
- ``cls``
- ``echo``
- ``pause``
- ``ver``
- ``color``

# Limitations
- Uses BIOS interrupts to do pretty much everything
- It's limited to loading up to 255 sectors because of the byte limit

# TODO
- Go into protected mode (and figure out how to replace the BIOS interrupts with equivalent code??)

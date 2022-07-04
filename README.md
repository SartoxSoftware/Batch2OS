# Batch2OS
A fun and dumb project made in C# that compiles batch code into native x86 code

![img.png](img.png)

# Compile
First, you must build Batch2OS. After that, create a batch file with some code in it, then execute this command to get a native binary:</br>
``Batch2OS <file>.bat``<br/>
You can now run the OS on a virtual hypervisor like QEMU:<br/>
``qemu-system-x86_64 -hdd <file>.img``<br/>
Or just run it on bare metal!

# Supported commands
- ``help``
- ``cls``
- ``echo``
- ``pause``
- ``ver``
- ``color``

# Limitations
- Uses BIOS interrupts to draw text
- It's limited to loading up to 255 sectors because of the byte limit

# TODO
- Go into protected mode and use 0xB8000 to draw text

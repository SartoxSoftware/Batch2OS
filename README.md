# Batch2OS
A fun and dumb project made in C# that compiles batch code into native x86 code

# Compile
Build Batch2OS, then in the directory where it got built create a file named ``os.bat`` where you can put your commands. Note that for now, only ``echo`` is supported.

# Limitations
- Only 512 bytes (first sector)
- Uses BIOS interrupts to draw text
- It's just dumb

# TODO
[x] Go into protected mode
[x] Use 0xB8000 to draw text
[x] Load more sectors than just one

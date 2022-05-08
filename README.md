# Batch2OS
A fun and dumb project made in C# that compiles batch code into native x86 code

![image](https://user-images.githubusercontent.com/49339966/167302328-3a19c666-0121-47ab-bc3b-da3aef58f5bd.png)

# Compile
Build Batch2OS, then in the directory where it got built create a file named ``os.bat`` where you can put your commands.

# Supported commands
- ``help``
- ``cls``
- ``echo``
- ``pause``
- ``ver``

# Limitations
- Only 512 bytes (first sector)
- Uses BIOS interrupts to draw text
- It's just dumb

# TODO
- Go into protected mode
- Use 0xB8000 to draw text
- Load more sectors than just one

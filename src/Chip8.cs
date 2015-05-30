using SFML.Graphics;
using SFML.Window;
using System;
using System.IO;

namespace Chipset8Emu
{
    public class Chip8
    {
        private Memory _memory;
        private CPU _cpu;
        private Graphics _graphics;
        private Input _input;

        private static Chip8 chip8; // Used for error management.

        private byte[] _font = { 0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
                                 0x20, 0x60, 0x20, 0x20, 0x70, // 1
                                 0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
                                 0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
                                 0x90, 0x90, 0xF0, 0x10, 0x10, // 4
                                 0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
                                 0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
                                 0xF0, 0x10, 0x20, 0x40, 0x40, // 7
                                 0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
                                 0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
                                 0xF0, 0x90, 0xF0, 0x90, 0x90, // A
                                 0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
                                 0xF0, 0x80, 0x80, 0x80, 0xF0, // C
                                 0xE0, 0x90, 0x90, 0x90, 0xE0, // D
                                 0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
                                 0xF0, 0x80, 0xF0, 0x80, 0x80  // F
                               };

        public bool ShuttingDown { get; private set; }

        public Chip8()
        {
            _memory = new Memory(4096, 16, 16);
            var window = new RenderWindow(new VideoMode(64, 32), "Chipset8", Styles.Default);
            _graphics = new Graphics(window);
            _input = new Input(window);
            _cpu = new CPU(_memory, _graphics, _input);

            chip8 = this;
        }

        public void Initalize(string programFilePath)
        {
            _cpu.Initalize();
            _memory.ResetRegisters();

            // Load the font into memory.
            for (int i = 0x00; i < 0x50; i++)
            {
                _memory.SetCell(i, _font[i]);
            }

            // Make sure the specified file exists.
            if (!File.Exists(programFilePath))
            {
                Chip8.ThrowError("program file does not exist");
            }

            // Load the program into memory.
            using (FileStream fileStream = File.Open(programFilePath, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    for (int i = 0x200; i < 0x200 + binaryReader.BaseStream.Length; i++)
                    {
                        _memory.SetCell(i, binaryReader.ReadByte());
                    }
                }
            }
        }

        public void EmulateCycle()
        {
            var startCycleTime = Environment.TickCount;
            _cpu.ExecuteCycle();
            _graphics.ExecuteCycle();

            var endCycleTime = Environment.TickCount;
            var cycleTime = endCycleTime - startCycleTime;

            if (cycleTime < (1000 / 60))
                System.Threading.Thread.Sleep((1000 / 60) - cycleTime);
        }

        private void HandleError(string reason)
        {
            Console.WriteLine("Fatal error: {0}!", reason);
            _graphics.Terminate();
            Console.ReadLine();
            Environment.Exit(0);
        }

        public static void ThrowError(string reason)
        {
            chip8.HandleError(reason);
        }
    }
}
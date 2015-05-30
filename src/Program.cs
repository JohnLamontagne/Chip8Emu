using System;

namespace Chipset8Emu
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Chip8 chip8 = new Chip8();

            if (args.Length <= 0)
                Chip8.ThrowError("no specified program file");
            else
                chip8.Initalize(args[0]);

            while (!chip8.ShuttingDown)
            {
                chip8.EmulateCycle();
            }

            Console.ReadLine();
        }
    }
}
/// Credits to http://www.multigesture.net/articles/how-to-write-an-emulator-chip-8-interpreter/ for
/// a fantastic introduction to emulator creation and specific details about implementing
/// a chip8 emulator.

using SFML.Window;
using System;
using System.Media;

namespace Chipset8Emu
{
    public class CPU
    {
        private Memory _memory;
        private Graphics _graphics;
        private Input _input;

        private ushort _opcode;
        private byte _stackPointer;
        private ushort _indexRegister;
        private ushort _programCounter;

        private byte _delayTimer;
        private byte _soundTimer;

        private Random _random;

        public CPU(Memory memory, Graphics graphics, Input input)
        {
            _memory = memory;
            _graphics = graphics;
            _input = input;
            _random = new Random();
        }

        public void Initalize()
        {
            _programCounter = 0x200;
            _opcode = 0;
            _indexRegister = 0;
            _stackPointer = 0;
        }

        public void ExecuteCycle()
        {
            // Read the 2 byte opcode from memory.
            _opcode = (ushort)(((_memory.GetCell(_programCounter) << 8) | _memory.GetCell(_programCounter + 1)));

            switch (_opcode & 0xF000) // Check first 4 bits of opcode.
            {
                case 0x000:
                    if ((_opcode & 0x000F) == 0x0000)
                    {
                        // Clear the screen.
                        _graphics.Clear();

                        _programCounter += 2;
                    }
                    else if ((_opcode & 0x000F) == 0x00E)
                    {
                        _programCounter = _memory.GetStackFrame(--_stackPointer);
                        _programCounter += 2;
                    }
                    break;

                case 0x1000: // Jump to address NNN.
                    // Jump to the specied address.
                    _programCounter = (ushort)(_opcode & 0x0FFF);
                    break;

                case 0x2000: // Call subroutine as address NNN.

                    // Check for stack overflow.
                    if (_stackPointer + 1 >= _memory.AllocatedStackFrames)
                    {
                        Chip8.ThrowError("stack overflow");
                    }

                    // store the current address of the program counter in the stack.
                    _memory.SetStackFrame(_stackPointer, _programCounter);
                    _stackPointer++;
                    // Jump to the specied address.
                    _programCounter = (ushort)(_opcode & 0x0FFF);
                    break;

                case 0x3000:
                    if (_memory.GetRegister((_opcode & 0x0F00) >> 8) == (_opcode & 0x00FF))
                        // Skip the next instruction.
                        _programCounter += 4;
                    else
                        _programCounter += 2;
                    break;

                case 0x4000:
                    if (_memory.GetRegister((_opcode & 0x0F00) >> 8) != (_opcode & 0x00FF))
                        // Skip the next instruction.
                        _programCounter += 4;
                    else
                        _programCounter += 2;
                    break;

                case 0x5000:
                    if (_memory.GetRegister((_opcode & 0x0F00) >> 8) == _memory.GetRegister((_opcode & 0x00F0) >> 4))
                        // Skip the next instruction.
                        _programCounter += 4;
                    else
                        _programCounter += 2;
                    break;

                case 0x6000: // Sets register X to the value NN.
                    _memory.SetRegister((_opcode & 0x0F00) >> 8, (byte)(_opcode & 0x00FF));
                    _programCounter += 2;
                    break;

                case 0x7000: // Adds NN to the value stored at register X.
                    _memory.SetRegister((_opcode & 0x0F00) >> 8, (byte)(_memory.GetRegister((_opcode & 0x0F00) >> 8) + (_opcode & 0x00FF)));
                    _programCounter += 2;
                    break;

                case 0x8000:
                    this.OpSRT8();
                    break;

                case 0x9000: // Skips the next instruction if the value at register X != the value at register Y.
                    if (_memory.GetRegister((_opcode & 0x0F00) >> 8) != _memory.GetRegister((_opcode & 0x00F0) >> 4))
                        _programCounter += 4; // Skip the next instruction.
                    else
                        _programCounter += 2;
                    break;

                case 0xA000: // Sets the value of the index register to NNN.
                    _indexRegister = (ushort)(_opcode & 0x0FFF);
                    _programCounter += 2;

                    break;

                case 0xB000: // Jumps to the address NNN + the value stored at register 0.
                    _programCounter = (ushort)((_opcode & 0x0FFF) + _memory.GetRegister(0x0));
                    break;

                case 0xC000:
                    _memory.SetRegister((_opcode & 0x0F00) >> 8, (byte)((byte)(_random.NextDouble() * byte.MaxValue) & (_opcode & 0x00FF)));
                    _programCounter += 2;
                    break;

                case 0xD000:
                    ushort sX = _memory.GetRegister((_opcode & 0x0F00) >> 8); // Sprite X pos
                    ushort sY = _memory.GetRegister((_opcode & 0x00F0) >> 4); // Sprite Y pos
                    ushort sHeight = (ushort)(_opcode & 0x000F); // Sprite height
                    ushort pixel;

                    _memory.SetRegister(0xF, 0);
                    for (int yline = 0; yline < sHeight; yline++)
                    {
                        pixel = _memory.GetCell(_indexRegister + yline);
                        for (int xline = 0; xline < 8; xline++)
                        {
                            if ((pixel & (0x80 >> xline)) != 0)
                            {
                                if (_graphics.GetPixel((ushort)(sX + xline), (ushort)(sY + yline)) == 1)
                                    _memory.SetRegister(0xF, 1);

                                _graphics.TogglePixel((uint)(sX + xline), (uint)(sY + yline));
                            }
                        }
                    }

                    _programCounter += 2;
                    break;

                case 0xE000:
                    if ((_opcode & 0x00FF) == 0x009E)
                    {
                        if (_memory.GetRegister((_opcode & 0x0F00) >> 8) == _input.KeyPressed)
                            _programCounter += 4;
                        else
                            _programCounter += 2;
                    }
                    else if ((_opcode & 0x00FF) == 0x00A1)
                    {
                        if (_memory.GetRegister((_opcode & 0x0F00) >> 8) != _input.KeyPressed)
                            _programCounter += 4;
                        else
                            _programCounter += 2;
                    }
                    else
                    {
                        _programCounter += 2;
                        Console.WriteLine("Unknown instruction {0}!", _opcode);
                    }
                    break;

                case 0xF000:
                    this.OpSRTF();
                    break;

                default:
                    _programCounter += 2;
                    Console.WriteLine("Unknown instruction {0}!", _opcode);
                    break;
            }

            // Decrease the timers by 1.
            if (_delayTimer != 0)
                _delayTimer--;
            if (_soundTimer != 0)
                _soundTimer--;
        }

        private void OpSRTF()
        {
            switch (_opcode & 0x00FF)
            {
                case 0x0007: // Set value of register X to the delay timer.
                    _memory.SetRegister((_opcode & 0x0F00) >> 8, _delayTimer);
                    _programCounter += 2;
                    break;

                case 0x000A:
                    if (_input.KeyPressed != null) // Wait until a key is pressed and then store said key into register X.
                    {
                        _memory.SetCell((_opcode & 0x0F00) >> 8, (byte)_input.KeyPressed);
                        _programCounter += 2;
                    }
                    break;

                case 0x0015:
                    // Set the delay timer to the value stored at register X.
                    _delayTimer = _memory.GetRegister((_opcode & 0x0F00) >> 8);
                    _programCounter += 2;
                    break;

                case 0x0018:
                    // Set the sound timer to the value stored at register X.
                    _soundTimer = _memory.GetRegister((_opcode & 0x0F00) >> 8);
                    _programCounter += 2;
                    break;

                case 0x001E:
                    _indexRegister += _memory.GetRegister((_opcode & 0x0F00) >> 8);
                    _programCounter += 2;
                    break;

                case 0x0029:
                    _indexRegister = (ushort)(_memory.GetCell((_opcode & 0x0F00) >> 8) * 5);
                    _programCounter += 2;
                    break;

                case 0x0033:
                    _memory.SetCell(_indexRegister, (byte)(_memory.GetRegister((_opcode & 0x0F00) >> 8) / 100));
                    _memory.SetCell(_indexRegister + 1, (byte)((_memory.GetRegister((_opcode & 0x0F00) >> 8) / 10) % 10));
                    _memory.SetCell(_indexRegister + 1, (byte)((_memory.GetRegister((_opcode & 0x0F00) >> 8) % 100) % 10));
                    _programCounter += 2;
                    break;

                case 0x0055:
                    byte endRegister = (byte)((_opcode & 0x0F00) >> 8);

                    for (byte i = 0; i < endRegister; i++)
                    {
                        _memory.SetCell(_indexRegister + i, _memory.GetRegister(i));
                    }

                    _programCounter += 2;

                    break;

                case 0x0065:
                    byte endReg = (byte)((_opcode & 0x0F00) >> 8);

                    for (byte i = 0; i < endReg; i++)
                    {
                        _memory.SetRegister(i, _memory.GetCell(_indexRegister + i));
                    }

                    _programCounter += 2;
                    break;

                default:
                    Console.WriteLine("Invalid instruction {0}!", _opcode.ToString("x4"));
                    _programCounter += 2;
                    break;
            }
        }

        private void OpSRT8()
        {
            switch ((_opcode & 0x000F))
            {
                case 0x0000:
                    _memory.SetRegister((_opcode & 0x0F00) >> 8, _memory.GetRegister((_opcode & 0x00F0) >> 4));
                    _programCounter += 2;
                    break;

                case 0x0001:
                    _memory.SetRegister((_opcode & 0x0F00) >> 8, (byte)(_memory.GetRegister((_opcode & 0x0F00) >> 8) | _memory.GetRegister((_opcode & 0x00F0) >> 4)));
                    _programCounter += 2;
                    break;

                case 0x0002:
                    _memory.SetRegister((_opcode & 0x0F00) >> 8, (byte)(_memory.GetRegister((_opcode & 0x0F00) >> 8) & _memory.GetRegister((_opcode & 0x00F0) >> 4)));
                    _programCounter += 2;
                    break;

                case 0x0003:
                    _memory.SetRegister((_opcode & 0x0F00) >> 8, (byte)(_memory.GetRegister((_opcode & 0x0F00) >> 8) ^ _memory.GetRegister((_opcode & 0x00F0) >> 4)));
                    _programCounter += 2;
                    break;

                case 0x0004:
                    if (_memory.GetRegister((_opcode & 0x00F0) >> 4) > (0xFF - _memory.GetRegister((_opcode & 0x0F00) >> 8)))
                        _memory.SetRegister(0xF, 1); // Set the register used to signify carry to 1.
                    else
                        _memory.SetRegister(0xF, 0);

                    _memory.SetRegister((_opcode & 0x0F00) >> 8, (byte)(_memory.GetRegister((_opcode & 0x0F00) >> 8) + _memory.GetRegister((_opcode & 0x00F0) >> 4)));
                    _programCounter += 2;
                    break;

                case 0x0005:
                    if (_memory.GetRegister((_opcode & 0x00F0) >> 4) > (_memory.GetRegister((_opcode & 0x0F00) >> 8)))
                        _memory.SetRegister(0xF, 1); // Set the register used to signify borrow to 1.
                    else
                        _memory.SetRegister(0xF, 0);

                    _memory.SetRegister((_opcode & 0x0F00) >> 8, (byte)(_memory.GetRegister((_opcode & 0x0F00) >> 8) - _memory.GetRegister((_opcode & 0x00F0) >> 4)));
                    _programCounter += 2;
                    break;

                case 0x0006:
                    _memory.SetRegister(0xF, (byte)(_memory.GetRegister((_opcode & 0x0F00) >> 8) & 0x01)); // Get LSB.
                    _memory.SetRegister((_opcode & 0x0F00) >> 8, (byte)(_memory.GetRegister((_opcode & 0x0F00) >> 8) >> 1));
                    _programCounter += 2;
                    break;

                case 0x0007:
                    if (_memory.GetRegister((_opcode & 0x0F00) >> 8) > _memory.GetRegister((_opcode & 0x00F0) >> 4))
                        _memory.SetRegister(0xF, 1); // Set the register used to signify borrow to 1.
                    else
                        _memory.SetRegister(0xF, 0);

                    _memory.SetRegister((_opcode & 0x0F00) >> 8, (byte)(_memory.GetRegister((_opcode & 0x00F0) >> 4) - _memory.GetRegister((_opcode & 0x0F00) >> 8)));

                    _programCounter += 2;
                    break;

                case 0x000E:
                    _memory.SetRegister(0xF, (byte)(_memory.GetRegister((_opcode & 0x0F00) >> 8) & 0xFF)); // Get MSB.
                    _memory.SetRegister((_opcode & 0x0F00) >> 8, (byte)(_memory.GetRegister((_opcode & 0x0F00) >> 8) << 1));
                    _programCounter += 2;
                    break;

                default:
                    Console.WriteLine("Invalid instruction!");
                    _programCounter += 2;
                    break;
            }
        }
    }
}
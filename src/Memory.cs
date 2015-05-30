using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chipset8Emu
{
    public class Memory
    {
        private byte[] _memory;
        private byte[] _registers;
        private ushort[] _stack;

        public int AllocatedStackFrames { get { return _stack.Length; } }

        public int AllocatedMemory { get { return _memory.Length; } }

        public int AllocatedRegisters { get { return _registers.Length; } }

        public Memory(uint memoryAllocation, byte registerCount, uint stackFrameCount)
        {
            _memory = new byte[memoryAllocation];
            _registers = new byte[registerCount];
            _stack = new ushort[stackFrameCount]; // 16 layered stack.
        }

        public byte GetCell(int cellIndex)
        {
            if (cellIndex >= _memory.Length)
            {
                Chip8.ThrowError("invalid memory access: sector " + cellIndex.ToString("x4") + " is out of bounds!");
            }

            return _memory[cellIndex];
        }

        public void SetCell(int cellIndex, byte value)
        {
            _memory[cellIndex] = value;
        }

        public byte GetRegister(int registerIndex)
        {
            return _registers[registerIndex];
        }

        public void SetRegister(int registerIndex, byte value)
        {
            _registers[registerIndex] = value;
        }

        public ushort GetStackFrame(int frameIndex)
        {
            return _stack[frameIndex];
        }

        public void SetStackFrame(int frameIndex, ushort value)
        {
            _stack[frameIndex] = value;
        }

        public void ResetRegisters()
        {
            for (int i = 0; i < _registers.Length; i++)
                _registers[i] = 0;
        }
    }
}
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;

namespace Chipset8Emu
{
    public class Input
    {
        private RenderWindow _renderWindow;

        public bool[] KeyPressed { get; private set; }

        public Input(RenderWindow renderWindow)
        {
            _renderWindow = renderWindow;

            _renderWindow.KeyPressed += _renderWindow_KeyPressed;
            _renderWindow.KeyReleased += _renderWindow_KeyReleased;

            this.KeyPressed = new bool[16];
        }

        private void _renderWindow_KeyReleased(object sender, SFML.Window.KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Num0:
                    this.KeyPressed[0x0] = false;
                    break;

                case Keyboard.Key.Num1:
                    this.KeyPressed[0x1] = false;
                    break;

                case Keyboard.Key.Num2:
                    this.KeyPressed[0x2] = false;
                    break;

                case Keyboard.Key.Num3:
                    this.KeyPressed[0x3] = false;
                    break;

                case Keyboard.Key.Num4:
                    this.KeyPressed[0x4] = false;
                    break;

                case Keyboard.Key.Num5:
                    this.KeyPressed[0x5] = false;
                    break;

                case Keyboard.Key.Num6:
                    this.KeyPressed[0x6] = false;
                    break;

                case Keyboard.Key.Num7:
                    this.KeyPressed[0x7] = false;
                    break;

                case Keyboard.Key.Num8:
                    this.KeyPressed[0x8] = false;
                    break;

                case Keyboard.Key.Num9:
                    this.KeyPressed[0x9] = false;
                    break;

                case Keyboard.Key.A:
                    this.KeyPressed[0xA] = false;
                    break;

                case Keyboard.Key.B:
                    this.KeyPressed[0xB] = false;
                    break;

                case Keyboard.Key.C:
                    this.KeyPressed[0xC] = false;
                    break;

                case Keyboard.Key.D:
                    this.KeyPressed[0xD] = false;
                    break;

                case Keyboard.Key.E:
                    this.KeyPressed[0xE] = false;
                    break;

                case Keyboard.Key.F:
                    this.KeyPressed[0xF] = false;
                    break;
            }
        }

        private void _renderWindow_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Num0:
                    this.KeyPressed[0x0] = true;
                    break;

                case Keyboard.Key.Num1:
                    this.KeyPressed[0x1] = true;
                    break;

                case Keyboard.Key.Num2:
                    this.KeyPressed[0x2] = true;
                    break;

                case Keyboard.Key.Num3:
                    this.KeyPressed[0x3] = true;
                    break;

                case Keyboard.Key.Num4:
                    this.KeyPressed[0x4] = true;
                    break;

                case Keyboard.Key.Num5:
                    this.KeyPressed[0x5] = true;
                    break;

                case Keyboard.Key.Num6:
                    this.KeyPressed[0x6] = true;
                    break;

                case Keyboard.Key.Num7:
                    this.KeyPressed[0x7] = true;
                    break;

                case Keyboard.Key.Num8:
                    this.KeyPressed[0x8] = true;
                    break;

                case Keyboard.Key.Num9:
                    this.KeyPressed[0x9] = true;
                    break;

                case Keyboard.Key.A:
                    this.KeyPressed[0xA] = true;
                    break;

                case Keyboard.Key.B:
                    this.KeyPressed[0xB] = true;
                    break;

                case Keyboard.Key.C:
                    this.KeyPressed[0xC] = true;
                    break;

                case Keyboard.Key.D:
                    this.KeyPressed[0xD] = true;
                    break;

                case Keyboard.Key.E:
                    this.KeyPressed[0xE] = true;
                    break;

                case Keyboard.Key.F:
                    this.KeyPressed[0xF] = true;
                    break;
            }
        }
    }
}
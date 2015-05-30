using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;

namespace Chipset8Emu
{
    public class Input
    {
        private RenderWindow _renderWindow;

        public byte? KeyPressed { get; private set; }

        public Input(RenderWindow renderWindow)
        {
            _renderWindow = renderWindow;

            _renderWindow.KeyPressed += _renderWindow_KeyPressed;
            _renderWindow.KeyReleased += _renderWindow_KeyReleased;
        }

        private void _renderWindow_KeyReleased(object sender, SFML.Window.KeyEventArgs e)
        {
            this.KeyPressed = null;
        }

        private void _renderWindow_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Num0:
                    this.KeyPressed = 0x0;
                    break;

                case Keyboard.Key.Num1:
                    this.KeyPressed = 0x1;
                    break;

                case Keyboard.Key.Num2:
                    this.KeyPressed = 0x2;
                    break;

                case Keyboard.Key.Num3:
                    this.KeyPressed = 0x3;
                    break;

                case Keyboard.Key.Num4:
                    this.KeyPressed = 0x4;
                    break;

                case Keyboard.Key.Num5:
                    this.KeyPressed = 0x5;
                    break;

                case Keyboard.Key.Num6:
                    this.KeyPressed = 0x6;
                    break;

                case Keyboard.Key.Num7:
                    this.KeyPressed = 0x7;
                    break;

                case Keyboard.Key.Num8:
                    this.KeyPressed = 0x8;
                    break;

                case Keyboard.Key.Num9:
                    this.KeyPressed = 0x9;
                    break;

                case Keyboard.Key.A:
                    this.KeyPressed = 0xA;
                    break;

                case Keyboard.Key.B:
                    this.KeyPressed = 0xB;
                    break;

                case Keyboard.Key.C:
                    this.KeyPressed = 0xC;
                    break;

                case Keyboard.Key.D:
                    this.KeyPressed = 0xD;
                    break;

                case Keyboard.Key.E:

                    this.KeyPressed = 0xE;
                    break;

                case Keyboard.Key.F:
                    this.KeyPressed = 0xF;
                    break;
            }
        }
    }
}
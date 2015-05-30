using SFML.Graphics;
using SFML.Window;
using System;

namespace Chipset8Emu
{
    public class Graphics
    {
        private RenderWindow _window;
        private Image _frameImage;
        private Sprite _frameSprite;

        public Graphics(RenderWindow renderWindow)
        {
            _window = renderWindow;
            _frameImage = new Image(64, 32);
            _frameSprite = new Sprite();
        }

        public void Clear()
        {
            _frameImage = new Image(_frameImage.Size.X, _frameImage.Size.Y);
        }

        public byte GetPixel(uint x, uint y)
        {
            return (byte)(_frameImage.GetPixel(x, y) == Color.Black ? 0 : 1);
        }

        public void SetPixel(uint x, uint y, bool state)
        {
            _frameImage.SetPixel(x, y, state ? Color.White : Color.Black);
        }

        public void TogglePixel(uint x, uint y)
        {
            _frameImage.SetPixel(x, y, _frameImage.GetPixel(x, y) == Color.White ? Color.Black : Color.White);
        }

        public void TogglePixels()
        {
            for (uint x = 0; x < _frameImage.Size.X; x++)
            {
                for (uint y = 0; y < _frameImage.Size.Y; y++)
                {
                    this.TogglePixel(x, y);
                }
            }
        }

        public void ExecuteCycle()
        {
            _window.DispatchEvents();
            _window.Clear(Color.Black);

            _frameSprite.Texture = new Texture(_frameImage);

            _window.Draw(_frameSprite);

            _window.Display();
        }

        public void Terminate()
        {
            _window.Close();
        }
    }
}
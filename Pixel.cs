using System;

namespace Snake
{
    public readonly struct Pixel
    {
        private const char PixelChar = '#';
        public int X { get; }
        public int Y { get; }
        public ConsoleColor Color { get; }

        public Pixel(int x, int y, ConsoleColor color) : this()
        {
            X = x;
            Y = y;
            Color = color;
        }

        public void DrawFood()
        {
            Console.ForegroundColor = Color;
            Console.SetCursorPosition(X, Y);
            Console.Write("$");
        }

        public void DrawHead()
        {
            Console.ForegroundColor = Color;
            Console.SetCursorPosition(X, Y);
            Console.Write("@");
        }

        public void Draw()
        {
            Console.ForegroundColor = Color;
            Console.SetCursorPosition(X, Y);
            Console.Write(PixelChar);
        }

        public void Erase()
        {
            Console.SetCursorPosition(X, Y);
            Console.Write(' ');
        }
    }
}
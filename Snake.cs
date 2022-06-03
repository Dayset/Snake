using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class Snake
    {
        private readonly ConsoleColor headColor;
        private readonly ConsoleColor bodyColor;

        public Snake(
            int initialX,
            int initialY,
            ConsoleColor headColor,
            ConsoleColor bodyColor,
            int bodyLenght = 3
                    )
        {
            this.headColor = headColor;
            this.bodyColor = bodyColor;

            Head = new Pixel(initialX, initialY, headColor);
            for (int i = bodyLenght; i >= 0; i--)

            {
                Body.Enqueue(new Pixel(Head.X - i - 1, initialY, this.bodyColor));
            }
            Draw();
        }

        public void Move(Direction directions)
        {
            Clear();
            Body.Enqueue(new Pixel(Head.X, Head.Y, this.bodyColor));
            Body.Dequeue();
            Head = directions switch
            {
                Direction.Up => new Pixel(Head.X, Head.Y - 1, this.headColor),
                Direction.Down => new Pixel(Head.X, Head.Y + 1, this.headColor),
                Direction.Left => new Pixel(Head.X - 1, Head.Y, this.headColor),
                Direction.Right => new Pixel(Head.X + 1, Head.Y, this.headColor),
                _ => Head
            };
            Draw();
        }

        public Pixel Head { get; private set; }
        public Queue<Pixel> Body { get; } = new Queue<Pixel>();

        public void Draw()
        {
            Head.Draw();
            foreach (Pixel pixel in Body)
            {
                pixel.Draw();
            }
        }

        public void Clear()
        {
            Head.Erase();
            foreach (Pixel pixel in Body)
            {
                pixel.Erase();
            }
        }
    }
}
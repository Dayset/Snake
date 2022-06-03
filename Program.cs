using System;
using System.Collections.Generic;
using static System.Console;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snake
{
    internal class Program
    {
        private const int MapWidth = 60;
        private const int MapHeight = 60;
        private const int FrameMS = 100; //initial speed, more is slower
        private static readonly Random random = new();

        private static void Main(string[] args)
        {
            SetWindowSize(MapWidth, MapHeight);
            SetBufferSize(MapWidth, MapHeight);
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
            CursorVisible = false;

            while (true)
            {
                StartGame();
                GameOver();
            }
        }

        private static void GameOver()
        {
            SetCursorPosition(MapWidth / 2 - 5, MapHeight / 3);
            Console.Write("Game Over !");
            Console.Write("\nScore: " + snake.Score);
            for (int i = 1; i < 4; i++)
            {
                Beep(425 * i - (100 * i * i), 100);
            }
            Beep(200, 1250);
            //Thread.Sleep(500);
            while (Console.KeyAvailable)
            {
                Console.ReadKey();
            }
            ReadKey();
        }

        private static void StartGame()
        {
            Clear();
            DrawBorder();
            Direction currentMovement = Direction.Right;
            var snake = new Snake(20, 20, ConsoleColor.DarkRed, ConsoleColor.DarkGreen);

            Stopwatch sw = new Stopwatch();

            while (true)
            {
                sw.Restart();
                Direction oldMovement = currentMovement;

                while (sw.ElapsedMilliseconds <= FrameMS)
                {
                    if (oldMovement == currentMovement)
                    {
                        currentMovement = ReadMovement(currentMovement);
                    }
                }
                snake.Move(currentMovement);
                if (snake.Head.X == MapWidth - 2 ||
                    snake.Head.X == 0 ||
                    snake.Head.Y == MapHeight - 2 ||
                    snake.Head.Y == 0 ||
                    snake.Body.Any(b => b.X == snake.Head.X && b.Y == snake.Head.Y))
                    break;
            }
            snake.Clear();
        }

        private static Direction ReadMovement(Direction currentDirection)
        {
            if (!KeyAvailable)
                return currentDirection;
            ConsoleKey key = ReadKey(true).Key;
            currentDirection = key switch
            {
                ConsoleKey.UpArrow when currentDirection != Direction.Down => Direction.Up,
                ConsoleKey.DownArrow when currentDirection != Direction.Up => Direction.Down,
                ConsoleKey.LeftArrow when currentDirection != Direction.Right => Direction.Left,
                ConsoleKey.RightArrow when currentDirection != Direction.Left => Direction.Right,
                _ => currentDirection
            };
            return currentDirection;
        }

        private static void DrawBorder()
        {//This method uses 17sec to draw 1000cycles 2s is Clear() only
            Console.Clear();
            ForegroundColor = ConsoleColor.White;
            var whiteSpace = new StringBuilder();
            var blackSpace = new StringBuilder();
            whiteSpace.Append('#', WindowWidth - 1);
            blackSpace.Append(' ', WindowWidth - 3);

            SetCursorPosition(0, 0);
            var sb = new StringBuilder();
            sb.AppendLine($"{whiteSpace}");
            for (int i = 1; i < MapHeight - 2; i++)
            {
                SetCursorPosition(0, i);
                sb.Append("#");
                sb.Append($"{blackSpace}");
                sb.AppendLine("#");
            }
            sb.AppendLine($"{whiteSpace}");
            Console.Write(sb);

            //Method with FOR to Write each block [67sec =1000 cycles]
            /*for (int i = 0; i < MapHeight; i++)
            {
                new Pixel(0, i, ConsoleColor.White).Draw();
                new Pixel(MapWidth - 1, i, ConsoleColor.White).Draw();
            }
            for (int i = 0; i < MapWidth; i++)
            {
                new Pixel(i, 1, ConsoleColor.White).Draw();
                new Pixel(i, MapHeight - 1, ConsoleColor.White).Draw();
            }*/
        }
    }
}
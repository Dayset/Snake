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
        private const int FrameMS = 100;
        private static readonly Random random = new Random();
        /*private const int std_output_handle = -11;
          private const uint enable_virtual_terminal_processing = 4;

          [DllImport("kernel32.dll", SetLastError = true)]
          private static extern IntPtr GetStdHandle(int nStdHandle);

          [DllImport("kernel32.dll")] private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

          [DllImport("kernel32.dll")] private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

          private static void Main()
          {
              var handle = GetStdHandle(std_output_handle);
              uint mode;
              GetConsoleMode(handle, out mode);
              mode |= enable_virtual_terminal_processing;
              SetConsoleMode(handle, mode);
              const string underline = "\x1B[4m";
              const string reset = "\x1B[0m";
              Console.WriteLine("Some " + underline + "underlined" + reset + " text");
          }
      }//
  } */

        private static void Main(string[] args)
        {
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
            CursorVisible = false;

            // //Find the right font in your Console Settings
            //so this square (4x4) looks about right
            //
            //    for (int i = 0; i < 4; i++)
            //    { WriteLine("####"); }
            //

            SetWindowSize(MapWidth, MapHeight);
            SetBufferSize(MapWidth, MapHeight);
            while (true)
            {
                StartGame();
                Thread.Sleep(1000);
                ReadKey();
            }
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
            SetCursorPosition(MapWidth / 2 - 5, MapHeight / 3);
            Console.WriteLine("Game Over !");
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
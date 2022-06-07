﻿/*
Important

To properly launch Snake Classic Game as windows console application, you should set Raster fonts in console properties.
Otherwise the application will throw an exception and will not work. Look for square font, if you don't have such, pick 8x9.

Unfortunately windows console is very limited, and there is no 'civilized' way to set font from application.
Otherwise without square like fonts text-ASCII graphics will look very ugly.
*/

using System;
using static System.Console;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snake
{
    internal class Program
    {
        private const int MapWidth = 60;
        private const int MapHeight = 40;

        private static Stopwatch timePlayed = new Stopwatch();

        private static readonly Random Random = new Random();

        private const ConsoleColor FoodColor = ConsoleColor.Yellow;
        private const ConsoleColor HeadColor = ConsoleColor.Red;
        private const ConsoleColor BodyColor = ConsoleColor.DarkGreen;
        public const int GameSpeedDelay = 100;

        // 200    = Easy 'Default'
        // 100    = Normal
        // 50     = Hard

        private static void Main(string[] args)
        {
            ConsoleSettings();
            while (true)
            {
                DrawBorder();
                DefaultConsoleColors();

                Welcome();
            }
        }

        private static void DefaultConsoleColors()
        {
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
        }

        private static void ConsoleSettings()
        {
            SetWindowSize(MapWidth, MapHeight);
            SetBufferSize(MapWidth, MapHeight);
            CursorVisible = false;
        }

        private static void DrawBorder()
        {
            // This method uses 17sec to draw 1000 cycles from which 2s is Console.Clear()
            // In total it's x4.4 times faster than second method.
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

            //Cooling the 'cw gun'. Draws border a bit smoother.
            Thread.Sleep(50);

            //  Old simple and relatively slow method  to write each block [1000 cycles = 67sec]
            //  for (int i = 0; i < MapHeight; i++)
            //      {
            //      new Pixel(0, i, ConsoleColor.White).Draw();
            //      new Pixel(MapWidth - 1, i, ConsoleColor.White).Draw();
            //      }
            //  for (int i = 0; i < MapWidth; i++)
            //      {
            //      new Pixel(i, 1, ConsoleColor.White).Draw();
            //      new Pixel(i, MapHeight - 1, ConsoleColor.White).Draw();
            //      }
        }

        private static void Welcome()
        {
            
            WriteText("THE SNAKE!");
            WriteText("Use Arrows to direct the Snake", 2);
            WriteText(" Press SPACE to start and pause the Game", 4);
            //Gives the player time to read the message
            Thread.Sleep(500);

            //Just to be sure..
            FlushKeyboard();
            //Eliminate screen blinking when user input is not Space
            SetCursorPosition(0, MapHeight - 1);
            while (ReadKey(true).Key != ConsoleKey.Spacebar)
            {
                SetCursorPosition(0, MapHeight - 1);
            }
            //Start the game when user input is Space

            //When the game starts clean the above text
            ClearText();
            ClearText(2);
            ClearText(4);
            
            //3 2 1 + Beeps
            for (int l = 3; l > 0; l--)
            {
                Task.Run(() => Beep(1500, 100));
                WriteText(l.ToString());
                Thread.Sleep(500);

                ClearText();
                Thread.Sleep(400);
            }

            StartGame();
        }

        private static void SnakeBornBeeps()
        {
            for (int i = 1; i < 6; i++)
            {
                Beep(370 * i, 100);
                Beep(500, 70);
            }
        }

        private static Pixel GenFood(Snake snake)
        {
            Pixel food;
            do
            {
                food = new Pixel(Random.Next(2, MapWidth - 3), (Random.Next(2, MapHeight - 3)), FoodColor);
            }
            while (snake.Head.X == food.Y && snake.Head.Y == food.Y
            || snake.Body.Any(b => b.X == food.X && b.Y == food.Y));

            return food;
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
            if (key == ConsoleKey.Spacebar)
            {
                PauseGame();
            }
            return currentDirection;
        }

        private static void StartGame()
        {
            Task.Run(() => SnakeBornBeeps());

            Direction currentMovement = Direction.Right;
            int MapX = (Random.Next(5, MapWidth - 20));
            int MapY = (Random.Next(10, MapHeight - 10));
            var snake = new Snake(MapX, MapY, HeadColor, BodyColor);

            Pixel food = GenFood(snake);
            food.DrawFood();

            FlushKeyboard();

            var sw = new Stopwatch();
            int Score = 0;
            int lagMs = 0;
            sw.Start();

            //Main Game Logic.
            while (true)
            {
                sw.Restart();
                timePlayed.Start();
                CursorVisible = false; 
                Direction oldMovement = currentMovement;

                while (sw.ElapsedMilliseconds <= GameSpeedDelay - lagMs)

                {
                    if (oldMovement == currentMovement)
                    {
                        currentMovement = ReadMovement(currentMovement);
                    }
                }
                if (snake.Head.X == food.X && snake.Head.Y == food.Y)
                {
                    Task.Run(() => Beep(2000 + (Score * 50), 100));
                    snake.Move(currentMovement, true);
                    food = GenFood(snake);
                    food.DrawFood();
                    Score++;
                }
                sw.Restart();

                snake.Move(currentMovement);
                if (snake.Head.X == MapWidth - 2 ||
                    snake.Head.X == 0 ||
                    snake.Head.Y == MapHeight - 2 ||
                    snake.Head.Y == 0 ||
                    snake.Body.Any(b => b.X == snake.Head.X && b.Y == snake.Head.Y))
                    break;
                lagMs = (int)(sw.ElapsedMilliseconds);
            }

            // GameOver Procedure.
            snake.Clear();
            food.Erase();
            DefaultConsoleColors();
            SetCursorPosition(MapWidth / 2 - 5, MapHeight / 3);
            Console.Write("Game Over!");
            SetCursorPosition(MapWidth / 2 - 6, MapHeight / 3 + 2);
            timePlayed.Stop();
            TimeSpan timeSpan = TimeSpan.FromSeconds(Convert.ToInt32(timePlayed.Elapsed.TotalSeconds));
            Console.Write("Time : " + timeSpan.Minutes + "m " + timeSpan.Seconds + "s");
            SetCursorPosition(MapWidth / 2 - 5, MapHeight / 3 + 4);
            if (Score < 10)
                Write("Score : " + Score + "$");
            else { Write("Score: " + Score + "$"); }
            GameOverTune();
        }

        private static void GameOverTune()
        {
            // 'Wow you failed' tune.
            for (int i = 1; i < 4; i++)
            {
                Beep(425 * i - (100 * i * i), 100);
            }
            Beep(200, 1000);
            Thread.Sleep(3000); //Additional chill timer
            ReadKey();
        }

        private static void FlushKeyboard()
        {
            while (Console.KeyAvailable) { Console.ReadKey(true); }
        }

        private static void PauseGame()
        {
            timePlayed.Stop();
            //Console.WriteLine("Game Paused");
            if (ReadKey(true).Key == ConsoleKey.Spacebar)
            {
                timePlayed.Start();
            }
            else
            {
                //if pressed any other button keep pausing game.
                PauseGame();
            }
        }

        /// <summary>
        /// Writes a text in the middle of the console
        /// </summary>
        /// <param name="text"></param>
        private static void WriteText(string text)
        {
            var length = text.Length;

            SetCursorPosition(MapWidth / 2 - length / 2, MapHeight / 3);
            Console.WriteLine(text);
        }

        /// <summary>
        /// Writes a text in the middle of the console
        /// </summary>
        /// <param name="text"></param>
        private static void WriteText(string text, int yOffset)
        {
            var length = text.Length;

            SetCursorPosition(MapWidth / 2 - length / 2, MapHeight / 3 + yOffset);
            Console.WriteLine(text);
        }

        /// <summary>
        /// Replaces the text written with line of whitespace
        /// </summary>
        /// <param name="length"></param>
        private static void ClearText()
        {
            SetCursorPosition(1, MapHeight / 3);
            Console.WriteLine(new string(' ', MapWidth - 3));
        }

        /// <summary>
        /// Replaces the text written with line of whitespace
        /// For lines underneath
        /// </summary>
        /// <param name="length"></param>
        private static void ClearText(int yOffset)
        {
            SetCursorPosition(1, MapHeight / 3 + yOffset);
            Console.WriteLine(new string(' ', MapWidth - 3));
        }
    }
}
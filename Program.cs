/*
Important

To properly launch Snake Classic Game as windows console application, you should set Raster fonts in console properties.
Otherwise the application will throw an exception and will not work. Look for square font, if you don't have such, pick 8x9.

Unfortunately windows console is very limited, and there is no 'civilized' way to set font from application.
Otherwise without square like fonts text-ASCII graphics will look very ugly.
*/

using System;
using System.Collections.Generic;
using static System.Console;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snake
{
    public class Program
    {
        //With 8x9 raster font and 640x480 the biggest Map size is 40x32.
        //I might be wrong: my Windows DPI scale is 150%
        private const int MapWidth = 40;

        private const int MapHeight = 32;

        private static ScreenOperations _screenOperations;
        private static AudioOperations _audioOperations;

        private static Stopwatch timePlayed = new Stopwatch();

        private static readonly Random Random = new Random();

        private const ConsoleColor FoodColor = ConsoleColor.Yellow;
        private const ConsoleColor HeadColor = ConsoleColor.Red;
        private const ConsoleColor BodyColor = ConsoleColor.DarkGreen;
        //public const int GameSpeedDelay = 100;

        // 200    = Easy 'Default'
        // 100    = Normal
        // 50     = Hard

        public static void Main(string[] args)
        {
            ScreenOperations.DisableResize();

            _screenOperations = new ScreenOperations(MapWidth, MapHeight);
            Console.Title = "The Snake! (Classic Console Game)";

            while (true)
            {
                Welcome();
            }
        }

        private static void Welcome()
        {
            var welcomeTexts = new List<string>();
            welcomeTexts.Add("The SNAKE!");
            welcomeTexts.Add("Arrows to direct the Snake");
            welcomeTexts.Add("SPACE to start and pause the Game");
            welcomeTexts.Add("1 = Easy  2 = Normal  3 = Hard");
            welcomeTexts.Add("ESCAPE to quit");

            _screenOperations.WriteText(welcomeTexts);
            //Gives the player time to read the message
            Thread.Sleep(100);

            //Just to be sure..
            FlushKeyboard();

            var gameDifficulty = SelectLevel();

            _screenOperations.ClearText(welcomeTexts.Count());
            AudioOperations.CountDown();
            StartGame(gameDifficulty);
        }

        private static int SelectLevel()
        {
            int gameDifficulty = 100;
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                    gameDifficulty = 200;
                    break;

                case ConsoleKey.D2:
                    gameDifficulty = 100;
                    break;

                case ConsoleKey.D3:
                    gameDifficulty = 50;
                    break;

                case ConsoleKey.Spacebar:
                    break;

                case ConsoleKey.Escape:
                    Clear();
                    Environment.Exit(0);
                    break;

                default:
                    Welcome();
                    break;
            };
            return gameDifficulty;
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
            if (key == ConsoleKey.Escape)
            {
                Clear();
                Environment.Exit(0);
            }
            return currentDirection;
        }

        private static void StartGame(int gameSpeedDelay)
        {
            _audioOperations = new AudioOperations();

            Task.Run(() => AudioOperations.SnakeBornBeeps());

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

            //Main Game Logic
            while (true)
            {
                sw.Restart();
                timePlayed.Start();
                CursorVisible = false;
                Direction oldMovement = currentMovement;

                while (sw.ElapsedMilliseconds <= gameSpeedDelay - lagMs)
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
            //Color changes somewhere before here so needs resetting.
            _screenOperations.DefaultConsoleColors();

            var gameOverTexts = new List<string>();
            gameOverTexts.Add("Game Over!");
            TimeSpan timeSpan = TimeSpan.FromSeconds(Convert.ToInt32(timePlayed.Elapsed.TotalSeconds));
            gameOverTexts.Add("Time : " + timeSpan.Minutes + "m " + timeSpan.Seconds + "s");
            gameOverTexts.Add("Score : " + Score + "$");
            _screenOperations.WriteText(gameOverTexts);

            AudioOperations.GameOverTune();
            //a single pixel missing in the border after game over text so screen needs reset here.
            _screenOperations.ResetScreen();
        }

        public static void FlushKeyboard()
        {
            while (Console.KeyAvailable) { Console.ReadKey(true); }
        }

        private static void PauseGame()
        {
            timePlayed.Stop();
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Spacebar:
                    timePlayed.Start();
                    break;

                case ConsoleKey.Escape:
                    Clear();
                    Environment.Exit(0);
                    break;

                default:
                    PauseGame();
                    break;
            };
        }
    }
}
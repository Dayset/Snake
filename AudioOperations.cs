using System;
using static System.Console;
using System.Threading;
using System.Threading.Tasks;

/*
Important

To properly launch Snake Classic Game as windows console application, you should set Raster fonts in console properties.
Otherwise the application will throw an exception and will not work. Look for square font, if you don't have such, pick 8x9.

Unfortunately windows console is very limited, and there is no 'civilized' way to set font from application.
Otherwise without square like fonts text-ASCII graphics will look very ugly.
*/

namespace Snake
{
    public class AudioOperations
    {
        private static ScreenOperations _screenOperations;

        public static void CountDown()
        {
            _screenOperations = new ScreenOperations(BufferWidth, BufferHeight);

            //3 2 1 + Beeps
            for (int l = 3; l > 0; l--)
            {
                Task.Run(() => Beep(1500, 100));
                _screenOperations.WriteText(l.ToString());
                Thread.Sleep(500);

                _screenOperations.ClearText(1);
                Thread.Sleep(400);
            }
        }

        public static void GameOverTune()
        {
            // 'Wow you failed' tune.
            for (int i = 1; i < 4; i++)
            {
                Beep(425 * i - (100 * i * i), 100);
            }
            Beep(200, 1000);
            Thread.Sleep(1000); //Additional chill timer
            Program.FlushKeyboard();

            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Spacebar:
                    //Should skips Welcome text if user
                    //presses Space after Game Over
                    break;

                default:
                    Program.Main(args: null);
                    break;
            };
        }

        public static void SnakeBornBeeps()
        {
            for (int i = 1; i < 6; i++)
            {
                Beep(370 * i, 100);
                Beep(500, 70);
            }
        }
    }
}
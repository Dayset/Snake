using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static System.Console;

namespace Snake
{
    public class ScreenOperations
    {
        //popular naming convention for private fields
        private int _mapWidth;
        private int _mapHeight;
        public ScreenOperations(int mapWidth, int mapHeight)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            ConsoleSettings();
            DrawBorder();
            DefaultConsoleColors();
        }

        public void ResetScreen()
        {
            DrawBorder();
        }
        private void ConsoleSettings()
        {
            SetWindowSize(_mapWidth, _mapHeight);
            SetBufferSize(_mapWidth, _mapHeight);
            CursorVisible = false;
        }

        private void DrawBorder()
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
            for (int i = 1; i < _mapHeight - 2; i++)
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

        public void DefaultConsoleColors()
        {
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// Write text in single line
        /// </summary>
        /// <param name="text"></param>
        public void WriteText(string text)
        {
            var length = text.Length;

            SetCursorPosition(_mapWidth / 2 - length / 2, _mapHeight / 3);
            Console.WriteLine(text);
        }
        /// <summary>
        /// Write text in multiple lines
        /// </summary>
        /// <param name="texts"></param>
        public void WriteText(List<string> texts)
        {
            var yOffset = 0;
            foreach (var text in texts)
            {
                var length = text.Length;

                SetCursorPosition(_mapWidth / 2 - length / 2, _mapHeight / 3 + yOffset);
                Console.WriteLine(text);
                yOffset += 2;
            }
        }

        /// <summary>
        /// Clear written text replacing with space
        /// </summary>
        /// <param name="amountOfLines"> How many lines to clean</param>
        public void ClearText(int amountOfLines)
        {
            var yOffset = 0;
            for (int i = 0; i < amountOfLines; i++)
            {
                SetCursorPosition(1, _mapHeight / 3 + yOffset);
                Console.WriteLine(new string(' ', _mapWidth - 3));
                yOffset += 2;
            }
        }


    }
}

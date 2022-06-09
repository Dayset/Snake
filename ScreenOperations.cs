using System;
using System.Collections.Generic;
using static System.Console;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Snake
{
    public class ScreenOperations
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

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

        public static void DisableResize()
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                //DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }
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

                SetCursorPosition(_mapWidth / 2 - (length / 2) - 1, _mapHeight / 3 + yOffset);
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
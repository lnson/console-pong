using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Rectangle
    {
        public Rectangle(int left, int top, int width, int height)
        {
            this.left = left;
            this.top = top;
            this.row = new String(' ', width);
            this.height = height;
        }

        protected void Draw(ConsoleColor color)
        {
            var savedBackgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = color;
            for (int i = 0; i < Height; ++i)
            {
                Console.SetCursorPosition(Left, Top + i);
                Console.Write(row);
            }
            Console.BackgroundColor = savedBackgroundColor;
        }

        private readonly String row;
        private readonly int height;
        private int left;
        private int top;

        public int Height => height;
        public int Width => row.Length;

        public int Left { get => left; set => left = value; }
        public int Top { get => top; set => top = value; }
        public int Right => Left + Width;
        public int Bottom => Top + Height;
    }

    class VerticalPad : Rectangle
    {
        public VerticalPad(int height, int column, int topMargin, int bottomMargin, ConsoleColor padColor, ConsoleColor backgroundColor)
            : base(column, (topMargin + bottomMargin - height) / 2, 2, height)
        {
            this.topMargin = topMargin;
            this.bottomMargin = bottomMargin;
            this.padColor = padColor;
            this.backgroundColor = backgroundColor;
            Draw(padColor);
        }

        public void MoveUp()
        {
            Move(-1);
        }

        public void MoveDown()
        {
            Move(1);
        }

        protected void Move(int delta)
        {
            int newTop = Top + delta;
            if (newTop < topMargin || newTop > bottomMargin - Height)
            {
                return;
            }

            Draw(backgroundColor);
            Top = newTop;
            Draw(padColor);
        }

        private readonly int topMargin;
        private readonly int bottomMargin;
        private readonly ConsoleColor padColor;
        private readonly ConsoleColor backgroundColor;
    }

    class HorizontalPad : Rectangle
    {
        public HorizontalPad(int width, int row, int leftMargin, int rightMargin, ConsoleColor padColor, ConsoleColor backgroundColor)
            : base((leftMargin + rightMargin - width) / 2, row, width, 1)
        {
            this.leftMargin = leftMargin;
            this.rightMargin = rightMargin;
            this.padColor = padColor;
            this.backgroundColor = backgroundColor;
            Draw(padColor);
        }

        public void MoveLeft()
        {
            Move(-1);
        }

        public void MoveRight()
        {
            Move(1);
        }

        protected void Move(int delta)
        {
            int newLeft = Left + delta;
            if (newLeft < leftMargin || newLeft > rightMargin - Width)
            {
                return;
            }

            Draw(backgroundColor);
            Left = newLeft;
            Draw(padColor);
        }

        private readonly int leftMargin;
        private readonly int rightMargin;
        private readonly ConsoleColor padColor;
        private readonly ConsoleColor backgroundColor;
    }

    class Ball
    {
        public Ball(int left, int top, ConsoleColor foregroundColor, ConsoleColor backgroundColor, HorizontalPad topPad, HorizontalPad bottomPad, VerticalPad leftPad, VerticalPad rightPad)
        {
            this.left = left;
            this.top = top;
            this.foregroundColor = foregroundColor;
            this.backgroundColor = backgroundColor;
            var rand = new Random();
            this.dx = rand.Next(2) == 0 ? -1 : 1;
            this.dy = rand.Next(2) == 0 ? -1 : 1;
            this.topPad = topPad;
            this.bottomPad = bottomPad;
            this.leftPad = leftPad;
            this.rightPad = rightPad;
            DrawBall();
        }

        public void Move()
        {
            while (MaybeReflectHorizontally() || MaybeReflectVertically())
            {
            }

            EraseBall();
            left += dx;
            top += dy;
            if (!IsDead())
            {
                DrawBall();
            }
        }

        public bool IsDead()
        {
            return top < topPad.Bottom || top > bottomPad.Top || left < leftPad.Right || left > rightPad.Left;
        }

        private void Draw(string what)
        {
            var savedForegroundColor = Console.ForegroundColor;
            var savedBackgroundColor = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.SetCursorPosition(left, top);
            Console.Write(what);
            Console.ForegroundColor = savedForegroundColor;
            Console.BackgroundColor = savedBackgroundColor;
        }

        private void DrawBall()
        {
            Draw("O");
        }

        private void EraseBall()
        {
            Draw(" ");
        }

        private bool MaybeReflectHorizontally()
        {
            int newLeft = left + dx;
            int newTop = top + dy;
            if ((newTop == topPad.Bottom - 1 && topPad.Left <= newLeft && newLeft <= topPad.Right) ||
                (newTop == bottomPad.Top && bottomPad.Left <= newLeft && newLeft <= bottomPad.Right))
            {
                dy = -dy;
                return true;
            }
            return false;
        }

        private bool MaybeReflectVertically()
        {
            int newLeft = left + dx;
            int newTop = top + dy;
            if ((newLeft == leftPad.Right - 1 && leftPad.Top <= newTop && newTop <= leftPad.Bottom) ||
                (newLeft == rightPad.Left && rightPad.Top <= newTop && newTop <= rightPad.Bottom))
            {
                dx = -dx;
                return true;
            }
            return false;
        }

        private int left;
        private int top;
        private int dx;
        private int dy;
        private HorizontalPad topPad;
        private HorizontalPad bottomPad;
        private VerticalPad leftPad;
        private VerticalPad rightPad;
        private ConsoleColor foregroundColor;
        private ConsoleColor backgroundColor;
    }

    class Program
    {
        public Program()
        {
            Console.CursorVisible = false;
            topPad = new HorizontalPad(10, 0, 2, Console.WindowWidth - 2, ConsoleColor.Red, ConsoleColor.Black);
            bottomPad = new HorizontalPad(10, Console.WindowHeight - 1, 2, Console.WindowWidth - 2, ConsoleColor.Red, ConsoleColor.Black);
            leftPad = new VerticalPad(5, 0, 1, Console.WindowHeight - 1, ConsoleColor.Blue, ConsoleColor.Black);
            rightPad = new VerticalPad(5, Console.WindowWidth - 2, 1, Console.WindowHeight - 1, ConsoleColor.Blue, ConsoleColor.Black);
        }

        static void WriteMiddle(String prompt)
        {
            var savedBackgroundColor = Console.BackgroundColor;
            var savedForegroundColor = Console.ForegroundColor;
            Console.SetCursorPosition((Console.WindowWidth - prompt.Length) / 2, Console.WindowHeight / 2);
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(prompt);
            Console.BackgroundColor = savedBackgroundColor;
            Console.ForegroundColor = savedBackgroundColor;
        }

        private void Reset()
        {
            ball = new Ball(Console.WindowWidth / 2, Console.WindowHeight / 2, ConsoleColor.Green, ConsoleColor.Black, topPad, bottomPad, leftPad, rightPad);
            stopWatch.Start();
        }

        void Run()
        {
            bool running = true;
            int pace = 200;  // 2ms per move.
            Reset();
            while (running)
            {
                if (stopWatch.ElapsedMilliseconds >= pace)
                {
                    stopWatch.Restart();
                    ball.Move();
                }
                if (ball.IsDead())
                {
                    stopWatch.Stop();
                    WriteMiddle("Game Over! Press R to reset.");
                }
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.DownArrow:
                            leftPad.MoveDown();
                            rightPad.MoveDown();
                            break;
                        case ConsoleKey.UpArrow:
                            leftPad.MoveUp();
                            rightPad.MoveUp();
                            break;
                        case ConsoleKey.LeftArrow:
                            topPad.MoveLeft();
                            bottomPad.MoveLeft();
                            break;
                        case ConsoleKey.RightArrow:
                            topPad.MoveRight();
                            bottomPad.MoveRight();
                            break;
                        case ConsoleKey.Escape:
                            running = false;
                            break;
                        case ConsoleKey.R:
                            Reset();
                            break;
                        case ConsoleKey.Z:
                            if (pace < 800)
                            {
                                pace *= 2;
                            }
                            break;
                        case ConsoleKey.A:
                            if (pace > 50)
                            {
                                pace /= 2;
                            }
                            break;
                    }
                }
            }
            stopWatch.Stop();
        }

        HorizontalPad topPad;
        HorizontalPad bottomPad;
        VerticalPad leftPad;
        VerticalPad rightPad;
        Ball ball;

        Stopwatch stopWatch = new Stopwatch();

        static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    class Board
    {
        public int WidthHeight { get; set; }
        public int FieldsCount { get; set; }
        public Field[] Fields { get; set; }
        public Direction SnakeDirection { get; set; }
        public int SnakeHeadX { get; set; }
        public int SnakeHeadY { get; set; }
        public int FoodX { get; set; }
        public int FoodY { get; set; }
        public int[] TailX { get; set; }
        public int[] TailY { get; set; }
        public int TailLength { get; set; }
        public int Score { get; set; }
        public int Tick { get; set; } = 0;
        public int TicksLeft { get; set; } = 100;
        public bool GameOver = false;
        private bool CanSwitchDir = true;
        private readonly Random rng = new Random();
        public Board(int widthHeight)
        {
            if (widthHeight % 2 != 0)
                widthHeight++;
            WidthHeight = widthHeight;

            SnakeDirection = Direction.right;
            SnakeHeadX = (int)Math.Floor((double)WidthHeight / 2);
            SnakeHeadY = (int)Math.Floor((double)WidthHeight / 2);

            TailX = new int[WidthHeight * WidthHeight];
            TailY = new int[WidthHeight * WidthHeight];
            TailLength = 1;
            TailX[0] = SnakeHeadX - 1;
            TailY[0] = SnakeHeadY;

            int rnNum = FindEmptyField();
            FoodX = X(rnNum);
            FoodY = Y(rnNum);

            FieldsCount = widthHeight * widthHeight;
            Fields = new Field[FieldsCount];

            RedrawFields();
        }
        public void ChangeDirection(Direction dir)
        {
            int k = (int)SnakeDirection;
            if (CanSwitchDir)
                if (dir == Direction.right)
                    SnakeDirection = (Direction)((k + 1) % 4);
            if (dir == Direction.left)
                SnakeDirection = (Direction)((k + 3) % 4);
        }
        public bool Progress1Tick()
        {
            CanSwitchDir = true;

            int prevX = TailX[0];
            int prevY = TailY[0];
            TailX[0] = SnakeHeadX;
            TailY[0] = SnakeHeadY;
            for (int i = 1; i < TailLength; i++)
            {
                int prevX2 = TailX[i];
                int prevY2 = TailY[i];

                TailX[i] = prevX;
                TailY[i] = prevY;

                prevX = prevX2;
                prevY = prevY2;
            }

            switch (SnakeDirection)
            {
                case Direction.up:
                    SnakeHeadY++;
                    if (SnakeHeadY >= WidthHeight) SnakeHeadY = 0;
                    break;
                case Direction.right:
                    SnakeHeadX++;
                    if (SnakeHeadX >= WidthHeight) SnakeHeadX = 0;
                    break;
                case Direction.down:
                    SnakeHeadY--;
                    if (SnakeHeadY < 0) SnakeHeadY = WidthHeight - 1;
                    break;
                case Direction.left:
                    SnakeHeadX--;
                    if (SnakeHeadX < 0) SnakeHeadX = WidthHeight - 1;
                    break;
                default:
                    break;
            }

            if (SnakeHeadX == FoodX && SnakeHeadY == FoodY)
            {
                int ef = FindEmptyField();
                TailX[TailLength] = TailX[TailLength - 1];
                TailY[TailLength] = TailY[TailLength - 1];
                if (TailLength < FieldsCount)
                    TailLength++;
                Score += 10;
                FoodX = X(ef);
                FoodY = Y(ef);

                TicksLeft = 200;
            }

            Tick++;
            RedrawFields();

            for (int i = 0; i < TailLength; i++)
            {
                if (TailX[i] == SnakeHeadX && TailY[i] == SnakeHeadY)
                {
                    return false;
                }
            }
            //timeout
            if (--TicksLeft < 1)
                return false;

            return true;
        }
        private void RedrawFields()
        {
            for (int i = 0; i < FieldsCount; i++)
                Fields[i] = Field.empty;

            for (int i = 0; i < TailLength; i++)
                Fields[TailX[i] + TailY[i] * WidthHeight] = Field.tail;

            Fields[SnakeHeadX + SnakeHeadY * WidthHeight] = Field.head;
            Fields[FoodX + FoodY * WidthHeight] = Field.food;
        }
        /*
         * randomly finds an empty field. Requires >10% of the board to be empty in order not to take too long. 
         */
        private int FindEmptyField()
        {

            int max = (int)Math.Pow(WidthHeight, 2);
            int randomField;

            while (true)
            {
                randomField = rng.Next(max);


                bool isEmpty = true;
                if (SnakeHeadX == X(randomField) ||
                    SnakeHeadY == Y(randomField))
                {
                    isEmpty = false;
                }

                if (isEmpty)
                    for (int i = 0; i < TailLength; i++)
                    {
                        if (TailX[i] == X(randomField) &&
                            TailY[i] == Y(randomField))
                        {
                            isEmpty = false;
                            break;
                        }
                    }
                if (isEmpty) return randomField;
            }
        }
        public int DistanceLeft(int x1, int x2)
        {
            if (x1 > x2)
                return x1 - x2;
            else if (x1 < x2)
                return x1 + (WidthHeight - x2);
            else
                return 0;
        }
        public int DistanceUp(int y1, int y2)
        {
            if (y2 > y1)
                return y2 - y1;
            else if (y2 < y1)
                return y2 + (WidthHeight - y1);
            else
                return 0;
        }
        public int DistanceRight(int x1, int x2)
        {
            if (x2 > x1)
                return x2 - x1;
            else if (x2 < x1)
                return x2 + (WidthHeight - x1);
            else
                return 0;
        }
        public int DistanceDown(int y1, int y2)
        {
            if (y1 > y2)
                return y1 - y2;
            else if (y1 < y2)
                return y1 + (WidthHeight - y2);
            else
                return 0;
        }
        private int X(int num)
        {// returns the X position congruent with the value of num
            return num % WidthHeight;
        }
        private int Y(int num)
        {// returns the Y position congruent with the value of num
            return (int)Math.Floor((decimal)num / WidthHeight);
        }
        internal enum Field
        {
            empty,
            head,
            tail,
            food
        }
        internal enum Direction
        {
            left,
            up,
            right,
            down
        }
    }

}

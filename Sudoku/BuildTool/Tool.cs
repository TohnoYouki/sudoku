using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public struct Operation
    {
        public SolutionNumber number;
        public int index;
        public Operation(SolutionNumber source, int value)
        {
            number = source;
            index = value;
        }
    }

    public struct OperationRecord    //候选值的去除操作
    {
        public int operationIndex;   //去除的候选值
        public int operationTime;    //去除操作发生的阶段
        public OperationRecord(int index, int time)
        {
            operationIndex = index;
            operationTime = time;
        }
    }

    public struct IntVector2
    {
        public int X;
        public int Y;

        public IntVector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static IntVector2 operator +(IntVector2 vector1, IntVector2 vector2)
        {
            return new IntVector2(vector1.X + vector2.X, vector1.Y + vector2.Y);
        }

        public static IntVector2 operator -(IntVector2 vector1, IntVector2 vector2)
        {
            return new IntVector2(vector1.X - vector2.X, vector1.Y - vector2.Y);
        }

        public static IntVector2 operator -(IntVector2 vector)
        {
            return new IntVector2(-vector.X, -vector.Y);
        }

        public static IntVector2 operator *(IntVector2 vector, int scale)
        {
            return new IntVector2(vector.X * scale, vector.Y * scale);
        }

        public static IntVector2 operator *(int scale, IntVector2 vector)
        {
            return new IntVector2(vector.X * scale, vector.Y * scale);
        }

        public static IntVector2 operator /(IntVector2 vector, int scale)
        {
            return new IntVector2(vector.X / scale, vector.Y / scale);
        }

        public static bool operator ==(IntVector2 vector1, IntVector2 vector2)
        {
            return (vector1.X == vector2.X) & (vector1.Y == vector2.Y);
        }

        public static bool operator !=(IntVector2 vector1, IntVector2 vector2)
        {
            return !(vector1 == vector2);
        }

        public IntVector2 Up()
        {
            return new IntVector2(X - 1, Y);
        }

        public IntVector2 Down()
        {
            return new IntVector2(X + 1, Y);
        }

        public IntVector2 Left()
        {
            return new IntVector2(X, Y - 1);
        }

        public IntVector2 Right()
        {
            return new IntVector2(X, Y + 1);
        }
    }

    public struct SudokuNumber
    {
        public IntVector2 Index;
        public int Number;

        public SudokuNumber(IntVector2 index, int number)
        {
            Index = index;
            Number = number;
        }
    }
}

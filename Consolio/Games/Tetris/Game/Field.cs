using System.Runtime.CompilerServices;

namespace Consolio.Tetris
{
    public sealed class Field
    {
        private readonly int[,] Cells;

        public readonly Vector2Int Size;

        public int Width  => Size.X;
        public int Height => Size.Y;


        public Field(Vector2Int FieldSize)
        {
            Size  = FieldSize;
            Cells = new int[Width, Height];
        }


        public int this[int X, int Y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Cells[X, Y];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Cells[X, Y] = value;
        }


        public bool Overlaps(Vector2Int Position, Block Block)
        {
            var Size  = this.Size;
            var Cells = this.Cells;

            foreach (var Cell in Block.EnumerateCells(Position))
            {
                if (Cell.Y >= Size.Y)
                {
                    continue;
                }
                if (Cell.X < 0 || Cell.X >= Size.X || Cell.Y < 0
                    || Cells[Cell.X, Cell.Y] != 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsFullLine(int Line)
        {
            if (Line >= Height)
            {
                return false;
            }

            var Cells = this.Cells;
            var Width = this.Width;

            for (var i = 0; i < Width; i++)
            {
                if (Cells[i, Line] == 0)
                {
                    return false;
                }
            }

            return true;
        }


        public bool Add(Vector2Int Position, Block Block)
        {
            var Cells  = this.Cells;
            var Height = this.Height;

            foreach (var Cell in Block.EnumerateCells(Position))
            {
                if (Cell.Y >= Height)
                {
                    return false;
                }
                Cells[Cell.X, Cell.Y] = Block.Color;
            }

            return true;
        }


        public void CopyRow(int From, int To)
        {
            for (var X = 0; X < Width; X++)
            {
                Cells[X, To] = Cells[X, From];
            }
        }

        public void ClearRow(int Line)
        {
            for (var X = 0; X < Width; X++)
            {
                Cells[X, Line] = 0;
            }
        }
    }
}
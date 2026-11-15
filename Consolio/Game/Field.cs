using System.Runtime.CompilerServices;

namespace Tetris
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

            bool CheckCell(Vector2Int Cell)
            {
                return !(Cell >= Vector2Int.Zero && Cell.X < Size.X
                    && (Cell.Y >= Size.Y || Cells[Cell.X, Cell.Y] != 0));
            }

            return Block.EnumerateCells(Position).Where(Cell => Cell.Y < Size.Y).Any(CheckCell);
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


        public void Add(Vector2Int Position, Block Block)
        {
            var Cells  = this.Cells;
            var Height = this.Height;

            foreach (var Cell in Block.EnumerateCells(Position))
            {
                if (Cell.Y < Height)
                {
                    Cells[Cell.X, Cell.Y] = Block.Color;
                }
            }
        }

        public void CopyTo(Field DestinationField)
        {
            var Source      = Cells;
            var Destination = DestinationField.Cells;
            Buffer.BlockCopy(Source, 0, Destination, 0, Size.X * Size.Y * sizeof(int));
        }


        public void CopyRow(int From, int To)
        {
            Buffer.BlockCopy
            (
                Cells, From * Width * sizeof(int),
                Cells, To * Width * sizeof(int),
                Width * sizeof(int)
            );
        }

        public void ClearRow(int Line)
        {
            Array.Clear(Cells, Line * Width, Width);
        }
    }
}
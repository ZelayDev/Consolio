namespace Tetris
{
    public struct Block
    {
        private const int BlockCount = 7;

        private static readonly Vector2Int[] Blocks =
        [
            new(0, 0), new(0, 1), new(1, 0), new(1, 1),  //O
            new(0, 0), new(0, -1), new(0, 1), new(0, 2), //I
            new(0, 0), new(-1, 0), new(1, 0), new(0, 1), //T
            new(0, 0), new(-1, 0), new(0, 1), new(1, 1), //S
            new(0, 0), new(1, 0), new(0, 1), new(-1, 1), //Z
            new(0, 0), new(-1, 0), new(1, 0), new(1, 1), //L
            new(0, 0), new(-1, 0), new(1, 0), new(1, -1) //J
        ];


        public readonly Vector2Int[] Cells;
        public readonly int          Color;

        private Block(Vector2Int[] Cells, int Color)
        {
            this.Cells = Cells;
            this.Color = Color;
        }

        public Block(Block Base, int Color)
        {
            this.Cells = Base.Cells;
            this.Color = Color;
        }


        public Block RotateRight()
        {
            return new Block
            (
                Array.ConvertAll
                (
                    Cells,
                    static Cell => new Vector2Int(-Cell.Y, Cell.X)
                ),
                Color
            );
        }

        public Block RotateLeft()
        {
            return new Block
            (
                Array.ConvertAll
                (
                    Cells,
                    static Cell => new Vector2Int(Cell.Y, Cell.X)
                ),
                Color
            );
        }


        public Block Clone()
        {
            var Clone = new Vector2Int[4];
            Cells.CopyTo(Clone, 0);
            return new Block(Clone, Color);
        }


        public IEnumerable<Vector2Int> EnumerateCells(Vector2Int Position)
        {
            foreach (var Cell in Cells)
            {
                yield return Position + Cell;
            }
        }

        public IEnumerable<int> EnumerateUniqueYs(Vector2Int Position)
        {
            var Used = new int[4];
            var UsedCount = 0;

            foreach (var Cell in EnumerateCells(Position))
            {
                for (var i = 0; i < UsedCount; i++)
                {
                    if (Used[i] == Cell.Y)
                    {
                        goto Continue;
                    }
                }

                Used[UsedCount] = Cell.Y;
                UsedCount++;

                yield return Cell.Y;
                
                Continue: ;
            }
        }


        public static Block GetRandomBlock(Random Random, int ColorCount)
        {
            var BlockIndex = Random.Next(BlockCount) * 4;
            var ColorIndex = Random.Next(Configuration.ColorReserved, ColorCount);

            var Cells = new Vector2Int[4];

            Array.Copy(Blocks, BlockIndex, Cells, 0, 4);

            return new Block(Cells, ColorIndex);
        }
    }
}
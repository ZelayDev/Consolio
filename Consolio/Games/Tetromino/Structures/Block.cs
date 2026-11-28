using Consolio;

namespace Consolio.Tetromino
{
    public struct Block
    {
        public const int BlockCount = 7;
        
        private static readonly Vector2Int[] BlockCells =
        [
            //O
            new(0, 0), new(0, 1), new(1, 0), new(1, 1),
            new(0, 0), new(0, 1), new(1, 0), new(1, 1),
            new(0, 0), new(0, 1), new(1, 0), new(1, 1),
            new(0, 0), new(0, 1), new(1, 0), new(1, 1),
            
            //T
            new(0, 0), new(-1, 0), new(0, 1), new(1, 0),
            new(0, 0), new(0, 1), new(1, 0), new(0, -1),
            new(0, 0), new(1, 0), new(0, -1), new(-1, 0),
            new(0, 0), new(0, -1), new(-1, 0), new(0, 1),
            
            //L
            new(0, 0), new(0, 1), new(0, -1), new(1, -1),
            new(0, 0), new(1, 0), new(-1, 0), new(-1),
            new(0, 0), new(0, -1), new(0, 1), new(-1, 1),
            new(0, 0), new(-1, 0), new(1, 0), new(1, 1),
            
            //J
            new(0, 0), new(0, 1), new(0, -1), new(-1),
            new(0, 0), new(-1, 0), new(1, 0), new(1, -1),
            new(0, 0), new(0, -1), new(0, 1), new(1, 1),
            new(0, 0), new(1, 0), new(-1, 0), new(-1, 1),
            
            //S
            new(0, 0), new(0, 1), new(1, 0), new(1, -1),
            new(0, 0), new(0, 1), new(-1, 0), new(1, 1),
            new(0, 0), new(0, 1), new(1, 0), new(1, -1),
            new(0, 0), new(0, 1), new(-1, 0), new(1, 1),
            
            //Z
            new(0, 0), new(0, -1), new(1, 0), new(1, 1),
            new(0, 0), new(0, 1), new(1, 0), new(-1, 1),
            new(0, 0), new(0, -1), new(1, 0), new(1, 1),
            new(0, 0), new(0, 1), new(1, 0), new(-1, 1),
            
            //I
            new(0, 0), new(0, 1), new(0, 2), new(0, -1),
            new(0, 0), new(1, 0), new(2, 0), new(-1, 0),
            new(0, 0), new(0, 1), new(0, 2), new(0, -1),
            new(0, 0), new(1, 0), new(2, 0), new(-1, 0)
        ];


        private static int GlobalId;

        public readonly int Index;
        public readonly int Color;
        public readonly int Id;

        public ReadOnlySpan<Vector2Int> Cells => BlockCells.AsSpan(Index, 4);

        public Block(int Index, int Color, int Id)
        {
            this.Index = Index;
            this.Color = Color;
            this.Id    = Id;
        }

        
        public static bool operator ==(Block A, Block B)
        {
            return A.Index == B.Index && A.Id == B.Id && A.Color == B.Color;
        }

        public static bool operator !=(Block A, Block B)
        {
            return !(A == B);
        }
        

        public readonly Block RotateRight()
        {
            int Base   = Index & ~0xF;
            int Rotation    = (Index >> 2) & 0x3;
            int NewRotation = (Rotation + 1) & 0x3;
            return new Block(Base | (NewRotation << 2), Color, Id);
        }

        public readonly Block RotateLeft()
        {
            int Base   = Index & ~0xF;
            int Rotation    = (Index >> 2) & 0x3;
            int NewRotation = (Rotation + 3) & 0x3;
            return new Block(Base | (NewRotation << 2), Color, Id);
        }


        public readonly IEnumerable<Vector2Int> EnumerateCells(Vector2Int Position)
        {
            var Start = Index;
            var Cells = BlockCells;
            
            for (int i = 0; i < 4; i++)
            {
                yield return Position + Cells[Start + i];
            }
        }

        public readonly IEnumerable<int> EnumerateUniqueYs(Vector2Int Position)
        {
            var Used      = new int[4];
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


        public static int GetNextId()
        {
            return GlobalId++;
        }
    }
}
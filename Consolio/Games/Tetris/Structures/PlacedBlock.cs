using Consolio;

namespace Consolio.Tetromino
{
    public readonly struct PlacedBlock
    {
        public readonly Vector2Int Position;
        public readonly Block      Block;

        public PlacedBlock(Vector2Int Position, Block Block)
        {
            this.Position = Position;
            this.Block    = Block;
        }

        public PlacedBlock(Vector2Int Position, PlacedBlock Block)
        {
            this.Position = Position;
            this.Block    = Block.Block;
        }


        public IEnumerable<Vector2Int> EnumerateAbsolutePositions()
        {
            return Block.EnumerateCells(Position);
        }
    }
}
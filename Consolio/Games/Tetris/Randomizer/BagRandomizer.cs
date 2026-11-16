namespace Consolio.Tetris
{
    public sealed class BagRandomizer : IBlockRandomizer
    {
        private readonly Random       Random;
        private readonly Queue<Block> Blocks;
        private readonly int          ColorCount;


        public BagRandomizer(int Colors)
        {
            Random     = new Random();
            Blocks     = new Queue<Block>(Block.BlockCount);
            ColorCount = Colors;
        }


        public Block Next()
        {
            if (Blocks.Count <= 0)
            {
                FillQueue();
            }
            return Blocks.Dequeue();
        }

        private void FillQueue()
        {
            var Random = this.Random;
            var Queue  = Blocks;
            var Colors = ColorCount;

            Span<Block> Bag = stackalloc Block[Block.BlockCount];
            for (var I = 0; I < Block.BlockCount; I++)
            {
                var BlockIndex = I << 4;
                var ColorIndex = Random.Next(Config.ColorReserved, Colors);
                Bag[I] = new Block(BlockIndex, ColorIndex, Block.GetNextId());
            }

            for (var I = Bag.Length - 1; I > 0; I--)
            {
                var J = Random.Next(I + 1);
                (Bag[I], Bag[J]) = (Bag[J], Bag[I]);
            }

            foreach (var Item in Bag)
            {
                Queue.Enqueue(Item);
            }
        }
    }
}
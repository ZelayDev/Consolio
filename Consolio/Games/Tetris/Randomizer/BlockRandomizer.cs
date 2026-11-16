namespace Consolio.Tetris
{
    public sealed class BlockRandomizer : IBlockRandomizer
    {
        private readonly Random Random;
        private readonly int    ColorCount;


        public BlockRandomizer(int Colors)
        {
            Random     = new Random();
            ColorCount = Colors;
        }


        public Block Next()
        {
            var BlockIndex = Random.Next(Block.BlockCount) << 4;
            var ColorIndex = Random.Next(Config.ColorReserved, ColorCount);

            return new Block(BlockIndex, ColorIndex, Block.GetNextId());
        }
    }
}
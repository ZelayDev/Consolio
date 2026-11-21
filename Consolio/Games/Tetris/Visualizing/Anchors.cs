namespace Consolio.Tetris
{
    public sealed class Anchors
    {
        private readonly int HeaderLength;
        private readonly int BlockLength;

        public readonly int NextBlocksLimit;

        public readonly Vector2Int HeaderSize;
        public readonly Vector2Int FieldSize;
        public readonly Vector2Int StatisticSize;
        public readonly Vector2Int NextBlocksSize;
        public readonly Vector2Int KeymapSize;
        public readonly Vector2Int BlockStatisticSize;

        public Vector2Int HeaderPosition         { get; private set; }
        public Vector2Int FieldPosition          { get; private set; }
        public Vector2Int StatisticPosition      { get; private set; }
        public Vector2Int NextBlocksPosition     { get; private set; }
        public Vector2Int KeymapPosition         { get; private set; }
        public Vector2Int BlockStatisticPosition { get; private set; }

        public Vector2Int FieldContent          => FieldPosition + Vector2Int.OneOne;
        public Vector2Int NextBlocksContent     => NextBlocksPosition + Vector2Int.OneOne;
        public Vector2Int BlockStatisticContent => BlockStatisticPosition + new Vector2Int(BlockStatisticSize.X - 1, 1);

        public Vector2Int ScorePosition => StatisticPosition + new Vector2Int(StatisticSize.X - 1, 1);
        public Vector2Int LinesPosition => StatisticPosition + new Vector2Int(StatisticSize.X - 1, 2);
        public Vector2Int LevelPosition => StatisticPosition + new Vector2Int(StatisticSize.X - 1, 3);

        private int ScreenWidth, ScreenHeight;


        public Anchors(Config Config)
        {
            HeaderLength = Config.Theme.Header.Length;
            BlockLength = Config.Block.Length;

            HeaderSize = new Vector2Int(HeaderLength + 2, 3);
            FieldSize = new Vector2Int(BlockLength * Config.FieldSize.X + 2, Config.FieldSize.Y + 2);
            StatisticSize = new Vector2Int(19, 5);
            NextBlocksSize = new Vector2Int(19, 12);
            KeymapSize = new Vector2Int(19, 10);
            BlockStatisticSize = new Vector2Int(19, 17);

            NextBlocksLimit = (NextBlocksSize.X - 2) / 6 * (NextBlocksSize.Y - 2);
        }


        public bool Update()
        {
            var WindowWidth = Console.WindowWidth;
            var WindowHeight = Console.WindowHeight;

            if (WindowWidth != ScreenWidth || WindowHeight != ScreenHeight)
            {
                ScreenWidth = WindowWidth;
                ScreenHeight = WindowHeight;

                Update(ScreenWidth, ScreenHeight);
                return true;
            }

            return false;
        }

        private void Update(int ScreenWidth, int ScreenHeight)
        {
            var Center = (ScreenWidth + 1) / 2;

            HeaderPosition = new Vector2Int(Center - (HeaderLength + 1) / 2, 0);
            FieldPosition = new Vector2Int(Center - FieldSize.X / 2 + 1, 3);
            StatisticPosition = new Vector2Int(FieldPosition.X - StatisticSize.X, FieldPosition.Y);
            NextBlocksPosition = new Vector2Int(FieldPosition.X + FieldSize.X, FieldPosition.Y);
            KeymapPosition = new Vector2Int(NextBlocksPosition.X, NextBlocksPosition.Y + NextBlocksSize.Y);
            BlockStatisticPosition = new Vector2Int(StatisticPosition.X, StatisticPosition.Y + 5);
        }
    }
}
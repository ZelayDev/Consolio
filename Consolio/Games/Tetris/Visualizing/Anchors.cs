namespace Consolio.Tetris
{
    public sealed class Anchors
    {
        private readonly int HeaderLength;
        private readonly int BlockLength;

        public readonly Vector2Int HeaderSize;
        public readonly Vector2Int FieldSize;
        public readonly Vector2Int StatisticSize;
        public readonly Vector2Int NextBlockSize;
        public readonly Vector2Int KeymapSize;
        public readonly Vector2Int BlockStatisticSize;

        public Vector2Int HeaderPosition         { get; private set; }
        public Vector2Int FieldPosition          { get; private set; }
        public Vector2Int StatisticPosition      { get; private set; }
        public Vector2Int NextBlockPosition      { get; private set; }
        public Vector2Int KeymapPosition         { get; private set; }
        public Vector2Int BlockStatisticPosition { get; private set; }

        public Vector2Int HeaderContent         { get; private set; }
        public Vector2Int FieldContent          { get; private set; }
        public Vector2Int NextBlockContent      { get; private set; }
        public Vector2Int KeymapContent         { get; private set; }
        public Vector2Int BlockStatisticContent { get; private set; }

        public Vector2Int ScorePosition { get; private set; }
        public Vector2Int LinesPosition { get; private set; }
        public Vector2Int LevelPosition { get; private set; }

        private int ScreenWidth, ScreenHeight;


        public Anchors(Config Config)
        {
            HeaderLength = Config.Theme.Header.Length;
            BlockLength  = Config.Block.Length;
            
            HeaderSize         = new Vector2Int(HeaderLength + 2, 3);
            FieldSize          = new Vector2Int(BlockLength * Config.FieldSize.X + 2, Config.FieldSize.Y + 2);
            StatisticSize      = new Vector2Int(19, 5);
            NextBlockSize      = new Vector2Int(19, 12);
            KeymapSize         = new Vector2Int(19, 10);
            BlockStatisticSize = new Vector2Int(19, 17);
        }


        public bool Update()
        {
            var WindowWidth  = Console.WindowWidth;
            var WindowHeight = Console.WindowHeight;

            if (WindowWidth != ScreenWidth || WindowHeight != ScreenHeight)
            {
                ScreenWidth  = WindowWidth;
                ScreenHeight = WindowHeight;

                Update(ScreenWidth, ScreenHeight);
                return true;
            }

            return false;
        }

        private void Update(int ScreenWidth, int ScreenHeight)
        {
            var Center = (ScreenWidth + 1) / 2;

            HeaderPosition         = new Vector2Int(Center - (HeaderLength + 1) / 2, 0);
            FieldPosition          = new Vector2Int(Center - FieldSize.X / 2 + 1, 3);
            StatisticPosition      = new Vector2Int(FieldPosition.X - StatisticSize.X, FieldPosition.Y);
            NextBlockPosition      = new Vector2Int(FieldPosition.X + FieldSize.X, FieldPosition.Y);
            KeymapPosition         = new Vector2Int(NextBlockPosition.X, NextBlockPosition.Y + NextBlockSize.Y);
            BlockStatisticPosition = new Vector2Int(StatisticPosition.X, StatisticPosition.Y + 5);

            HeaderContent         = HeaderPosition + Vector2Int.Up;
            FieldContent          = FieldPosition + Vector2Int.Up;
            NextBlockContent      = NextBlockPosition + Vector2Int.Up;
            KeymapContent         = KeymapPosition + Vector2Int.Up;
            BlockStatisticContent = BlockStatisticPosition + Vector2Int.OneOne;

            var StatisticX = StatisticPosition.X + 7;
            ScorePosition = new Vector2Int(StatisticX, StatisticPosition.Y + 1);
            LinesPosition = new Vector2Int(StatisticX, StatisticPosition.Y + 2);
            LevelPosition = new Vector2Int(StatisticX, StatisticPosition.Y + 3);
        }
    }
}
namespace Consolio.Snake
{
    public sealed class Anchors
    {
        public readonly Vector2Int HeaderSize;
        public readonly Vector2Int StatisticSize;
        public readonly Vector2Int FieldSize;
        public readonly Vector2Int KeymapSize;

        public Vector2Int HeaderStart;
        public Vector2Int StatisticStart;
        public Vector2Int FieldStart;
        public Vector2Int KeymapStart;

        public Vector2Int FieldContent;
        public Vector2Int StatisticContent;
        
        public Anchors(Config Config)
        {
            var Theme      = Config.Theme;
            var CellLength = Theme.Cell.Length;

            var Width = Config.FieldSize.X * CellLength + 2;
            
            HeaderSize    = new Vector2Int(Theme.Header.Length + 2, 3);
            StatisticSize = new Vector2Int(Width, 3);
            FieldSize     = new Vector2Int(Width, Config.FieldSize.Y + 2);
            KeymapSize    = new Vector2Int(20, 10);
        }

        public void Update(int ScreenWidth, int ScreenHeight)
        {
            var CenterX = ScreenWidth / 2;

            HeaderStart    = new Vector2Int(CenterX - HeaderSize.X / 2, 0);
            StatisticStart = new Vector2Int(CenterX - StatisticSize.X / 2, HeaderStart.Y + HeaderSize.Y);
            FieldStart     = new Vector2Int(CenterX - FieldSize.X / 2, StatisticStart.Y + StatisticSize.Y);
            KeymapStart = new Vector2Int(FieldStart.X + FieldSize.X, FieldStart.Y);
            
            FieldContent = FieldStart + Vector2Int.OneOne;
            StatisticContent = new Vector2Int(StatisticStart.X + StatisticSize.X - 1, StatisticStart.Y + StatisticSize.Y - 1);
        }
    }
}
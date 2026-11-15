namespace Tetris
{
    public sealed class Configuration
    {
        public const int ColorReserved = 2;


        public static readonly Configuration Default = new()
        {
            FieldSize = new Vector2Int(10, 20),
            Delay     = 500,
            Colors =
            [
                new RGBColor(0, 0, 0),
                new RGBColor(0, 240, 240),
                new RGBColor(240, 240, 0),
                new RGBColor(160, 0, 240),
                new RGBColor(0, 240, 0),
                new RGBColor(240, 0, 0),
                new RGBColor(0, 0, 240),
                new RGBColor(240, 160, 0)
            ],

            BorderStyle = new BorderStyle
            {
                Style = BorderLine.DoubleLine,
                Color = new RGBColor(200, 50, 150)
            },

            Header = new ColorText
            {
                Text  = "===TETRIS===",
                Color = new RGBColor(240, 240, 240)
            },

            Block = "[]",
            Empty = "[]"
        };

        public int Delay       { get; init; }
        public int InputBuffer { get; init; }

        public Vector2Int FieldSize { get; init; }
        public RGBColor[] Colors    { get; init; }

        public BorderStyle BorderStyle { get; init; }

        public ColorText Header { get; init; }
        public string    Block  { get; init; }
        public string    Empty  { get; init; }

        public int      ColorCount  => Colors.Length;
        public RGBColor EmptyColor  => Colors[0];
        public RGBColor ShadowColor => Colors[1];
    }
}
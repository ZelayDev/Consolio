namespace Consolio.Tetris
{
    public sealed class Config
    {
        public const int ColorReserved = 2;

        public static readonly Config Default = new()
        {
            FieldSize = new Vector2Int(10, 20),
            Delay = new Delay
            {
                StartDelay = 350,
                MinDelay   = 100
            },

            Theme = new Theme
            {
                Colors =
                [
                    new RGBColor(20, 20, 20),
                    new RGBColor(65, 65, 65),
                    new RGBColor(0, 240, 240),
                    new RGBColor(240, 240, 0),
                    new RGBColor(160, 0, 240),
                    new RGBColor(0, 240, 0),
                    new RGBColor(240, 0, 0),
                    new RGBColor(0, 0, 240),
                    new RGBColor(240, 160, 0)
                ],
                IndicatorColor = new RGBColor(200, 200, 200),
                ScoreColor = new RGBColor(250, 80, 3),
                LinesColor = new RGBColor(250, 230, 3),
                LevelColor = new RGBColor(110, 250, 3),
                
                BorderStyle = new BorderStyle
                {
                    Style = BorderLine.DoubleLine,
                    Color = new RGBColor(22, 148, 139)
                },
                Header = new ColorText
                {
                    Text  = "===TETRIS===",
                    Color = new RGBColor(240, 240, 240)
                },
                
                Blocks =
                [
                    "██  ",
                    "▄█▄ ",
                    "▄▄█ ",
                    "█▄▄ ",
                    "▄█▀ ",
                    "▀█▄ ",
                    "▄▄▄▄"
                
                    //"⣿⣿  "
                    //"⣤⣿⣤ "
                    //"⣤⣤⣿ "
                    //"⣿⣤⣤ "
                    //"⣤⣿⠛ "
                    //"⠛⣿⣤ "
                    //"⣤⣤⣤⣤"
                ]
            },

            Block       = "[]",
            Empty       = "[]",
            InputBuffer = 5
        };


        public Vector2Int FieldSize { get; init; }
        public Delay      Delay     { get; init; }

        public Theme Theme { get; init; }

        public string Block       { get; init; }
        public string Empty       { get; init; }
        public int    InputBuffer { get; init; }
                
        public int ColorCount => Theme.Colors.Length;
    }
}
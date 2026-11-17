namespace Consolio.Tetris
{
    public sealed class Theme
    {
        public static readonly Theme Default = new Theme
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
            ScoreColor     = new RGBColor(250, 80, 3),
            LinesColor     = new RGBColor(250, 230, 3),
            LevelColor     = new RGBColor(110, 250, 3),

            BorderStyle = new BorderStyle
            {
                Style = BorderLine.DoubleLine,
                Color = new RGBColor(22, 148, 139)
            },
            Header = new ColorText
            {
                Text  = "===TETRIS===",
                Color = new RGBColor(211, 235, 33)
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
        };


        public RGBColor[] Colors
        {
            get => field ?? Default.Colors;
            init
            {
                field = value.Length < 3
                    ? Default.Colors
                    : value;
            }
        }
        
        public RGBColor    IndicatorColor { get; init; }
        public RGBColor    ScoreColor     { get; init; }
        public RGBColor    LinesColor     { get; init; }
        public RGBColor    LevelColor     { get; init; }
        
        public BorderStyle BorderStyle    { get; init; }
        public ColorText   Header         { get; init; }

        public string[] Blocks
        {
            get => field ?? Default.Blocks;
            init => field = CorrectBlocks(value);
        }


        private static string[] CorrectBlocks(string[] Blocks)
        {
            if (Blocks.Length != Block.BlockCount)
            {
                return Default.Blocks;
            }
            
            for (int i = 0; i < Block.BlockCount; i++)
            {
                var Block = Blocks[i];
                if (Block.Length < 4)
                {
                    Blocks[i] = Block.PadLeft(4, ' ');
                }
                else if (Block.Length > 4)
                {
                    Blocks[i] = Block[..4];
                }
            }

            return Blocks;
        }
    }
}
namespace Consolio.Tetromino
{
    public sealed class Theme
    {
        public static readonly Theme Default = new();
        
        private string _Block = "[]";
        private string _Empty = "[]";
        

        public RGBColor IndicatorColor { get; init; } = new(200, 200, 200);
        public RGBColor LeaderDots     { get; init; } = new(150, 150, 150);
        public RGBColor PauseColor     { get; init; } = new(211, 235, 33);

        public RGBColor ScoreColor { get; init; } = new(250, 80, 3);
        public RGBColor LinesColor { get; init; } = new(250, 230, 3);
        public RGBColor LevelColor { get; init; } = new(110, 250, 3);

        public RGBColor KeymapIcons { get; init; } = new(191, 230, 53);
        public RGBColor KeymapBinds { get; init; } = new(24, 154, 201);

        public BorderStyle BorderStyle { get; init; } = new()
        {
            Style = BorderLine.DoubleLine,
            Color = new RGBColor(22, 148, 139)
        };

        public ColorText Header { get; init; } = new()
        {
            Text  = "===TETROMINO===",
            Color = new RGBColor(211, 235, 33)
        };

        public ColorText QueuePlaceHolder { get; init; } = new()
        {
            Text = "~~~~",
            Color = new RGBColor(205, 247, 66)
        };
        
        public RGBColor[] Colors
        {
            get;
            init
            {
                if (value.Length >= 3)
                {
                    field = value;
                }
            }
        } =
        [
            new(20, 20, 20),
            new(65, 65, 65),
            new(0, 240, 240),
            new(240, 240, 0),
            new(160, 0, 240),
            new(0, 240, 0),
            new(240, 0, 0),
            new(0, 0, 240),
            new(240, 160, 0)
        ];

        public string[] Blocks
        {
            get;
            init => field = CorrectBlocks(value);
        } =
        [
            "██  ",
            "▄█▄ ",
            "▄▄█ ",
            "█▄▄ ",
            "▄█▀ ",
            "▀█▄ ",
            "▄▄▄▄"
        ];
        
        public string Block
        {
            get => _Block;
            init => _Block = value;
        }

        public string Empty
        {
            get => _Empty;
            init => _Empty = value;
        }
        

        private static string[] CorrectBlocks(string[] Blocks)
        {
            if (Blocks.Length != Consolio.Tetromino.Block.BlockCount)
            {
                return Default.Blocks;
            }

            for (var i = 0; i < Consolio.Tetromino.Block.BlockCount; i++)
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
        
        
        public Theme Correct()
        {
            if (_Block.Length != _Empty.Length)
            {
                _Block = Default._Block;
                _Empty = Default._Empty;
            }

            return this;
        }
    }
}
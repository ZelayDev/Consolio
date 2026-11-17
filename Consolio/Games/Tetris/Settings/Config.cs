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

            Theme = Theme.Default,
            
            Block       = "[]",
            Empty       = "[]",
            InputBuffer = 5,
            BlockQueue  = 3
        };

        public Vector2Int FieldSize { get; init; }
        public Delay      Delay     { get; init; }

        public Theme Theme { get; init; }

        private string _Block;
        public string Block
        {
            get => _Block;
            init => _Block = value;
        }

        private string _Empty;
        public string Empty
        {
            get => _Empty;
            init => _Empty = value;
        }

        public int InputBuffer
        {
            get;
            init => field = Math.Max(1, value);
        }

        public int BlockQueue
        {
            get;
            init => field = Math.Clamp(value, 1, 15);
        }
        
        public RandomizerKind RandomizerKind { get; init; }

        public int ColorCount => Theme.Colors.Length;


        public Config Correct()
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
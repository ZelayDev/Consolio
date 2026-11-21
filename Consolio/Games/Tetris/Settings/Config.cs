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
                MinDelay   = 100,
                ResumeDelay = 200
            },
            
            Block       = "[]",
            Empty       = "[]",
            InputBuffer = 5,
            BlockQueue  = 15,
            
            RandomizerKind = RandomizerKind.Bag,

            Theme = Theme.Default,
            Keymap = Keymap.Default
        };

        
        private string _Block = "[]";
        private string _Empty = "[]";

        
        public Vector2Int FieldSize { get; init; }
        public Delay      Delay     { get; init; }
        
        public RandomizerKind RandomizerKind { get; init; }
        
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

        public Theme  Theme  { get; init; } = Theme.Default;
        public Keymap Keymap { get; init; } = Keymap.Default;
        
        
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
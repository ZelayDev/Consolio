namespace Consolio.Tetromino
{
    public sealed class Config
    {
        public const int ColorReserved = 2;

        public static readonly Config Default = new();


        public Vector2Int FieldSize
        {
            get;
            init => field = Vector2Int.Max(value, Field.MinSize);
        } = Field.DefaultSize;

        public Delay Delay { get; init; } = new()
        {
            StartDelay  = 350,
            MinDelay    = 100,
            ResumeDelay = 200
        };

        public RandomizerKind RandomizerKind { get; init; } = RandomizerKind.Bag;

        public int InputBuffer
        {
            get;
            init => field = Math.Max(1, value);
        } = 5;

        public int BlockQueue
        {
            get;
            init => field = Math.Clamp(value, 1, 15);
        } = 15;

        public Keymap Keymap { get; init; } = Keymap.Default;
        public Theme Theme
        {
            get;
            init => field = value.Correct();
        } = Theme.Default;
    }
}
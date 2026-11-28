namespace Consolio.Snake
{
    public sealed class Config
    {
        public static readonly Config Default = new();


        public Vector2Int FieldSize
        {
            get;
            init => field = Vector2Int.Max(value, Field.MinSize);
        } = Field.DefaultSize;

        public Delay Delay { get; init; } = new()
        {
            StartDelay = 280,
            MinDelay = 80,
            ResumeDelay = 200
        };


        public int InputBuffer
        {
            get;
            init => field = Math.Max(1, value);
        } = 5;
        
        public Theme  Theme  { get; init; } = Theme.Default;
        public Keymap Keymap { get; init; } = Keymap.Default;
    }
}
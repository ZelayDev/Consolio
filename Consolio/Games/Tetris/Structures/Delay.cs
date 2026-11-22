namespace Consolio.Tetromino
{
    public readonly struct Delay
    {
        public int StartDelay  { get; init; }
        public int MinDelay    { get; init; }
        public int ResumeDelay { get; init; }
    }
}
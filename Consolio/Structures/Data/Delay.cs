namespace Consolio
{
    public readonly struct Delay
    {
        public int StartDelay
        {
            get;
            init => field = Math.Max(0, value);
        }

        public int MinDelay
        {
            get;
            init => field = Math.Max(0, value);
        }

        public int ResumeDelay
        {
            get;
            init => field = Math.Max(0, value);
        }
    }
}
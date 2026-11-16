namespace Consolio
{
    public static class Program
    {
        public static async Task Main(string[] Arguments)
        {
            await new Tetris.Tetris().Run();
        }
    }
}
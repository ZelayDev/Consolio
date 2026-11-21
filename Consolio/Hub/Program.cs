using System.Text;

namespace Consolio
{
    public static class Program
    {
        public static async Task Main(string[] Arguments)
        {
            Console.OutputEncoding = Encoding.UTF8;
            await new Tetris.Tetris().Run();
        }
    }
}
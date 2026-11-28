namespace Consolio.Snake
{
    public sealed class Theme
    {
        public static readonly Theme Default = new();

        public ColorText Header { get; init; } = new()
        {
            Text  = "===TETROMINO===",
            Color = new RGBColor(211, 235, 33)
        };

        public BorderStyle BorderStyle { get; init; } = new()
        {
            Style = BorderLine.DoubleLine,
            Color = new RGBColor(22, 148, 139)
        };
        
        public RGBColor SnakeColor { get; init; }
    }
}
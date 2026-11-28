namespace Consolio.Snake
{
    public sealed class Theme
    {
        public static readonly Theme Default = new();

        
        public RGBColor IndicatorColor { get; init; } = new(200, 200, 200);
        public RGBColor LeaderDots     { get; init; } = new(150, 150, 150);
        public RGBColor PauseColor     { get; init; } = new(211, 235, 33);

        public RGBColor ScoreColor { get; init; } = new(250, 80, 3);

        public RGBColor KeymapIcons { get; init; } = new(191, 230, 53);
        public RGBColor KeymapBinds { get; init; } = new(24, 154, 201);
        
        
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

        public ColorText Cell { get; init; } = new("..", new(20, 20, 20));
    }
}
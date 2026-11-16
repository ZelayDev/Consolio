namespace Consolio.Tetris
{
    public sealed class Theme
    {
        public RGBColor[]  Colors         { get; init; }
        
        public RGBColor    IndicatorColor { get; init; }
        public RGBColor    ScoreColor     { get; init; }
        public RGBColor    LinesColor     { get; init; }
        public RGBColor    LevelColor     { get; init; }
        
        public BorderStyle BorderStyle    { get; init; }
        public ColorText   Header         { get; init; }
    }
}
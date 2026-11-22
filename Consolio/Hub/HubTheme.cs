namespace Consolio
{
    public sealed class HubTheme
    {
        public static readonly HubTheme Default = new();

        public RGBColor HeaderColor       { get; init; } = new(240, 240, 240);
        public RGBColor NameColor         { get; init; } = new(140, 140, 140);
        public RGBColor SelectedNameColor { get; init; } = new(0, 240, 240);

        public RGBColor SeparatorColor    {get; init; } = new(200, 50, 150);
        public RGBColor SectionColor      {get; init; } = new(240, 240, 240);
        
        public BorderStyle BorderStyle { get; init; } = new()
        {
            Style = BorderLine.DoubleLine,
            Color = new RGBColor(200, 50, 150)
        };
    }
}
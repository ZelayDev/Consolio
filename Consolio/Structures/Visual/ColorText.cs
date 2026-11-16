namespace Consolio
{
    public readonly struct ColorText
    {
        public string   Text  { get; init; }
        public RGBColor Color { get; init; }
        
        public int Length => Text.Length;

        
        public ColorText(string Text, RGBColor Color)
        {
            this.Text = Text;
            this.Color = Color;
        }


        public override string ToString()
        {
            return $"\x1b[38;2;{Color.R};{Color.G};{Color.B}m{Text}\x1b[0m";
        }
    }
}
using System.Text.Json.Serialization;

namespace Consolio
{
    public readonly struct BorderStyle
    {
        private const string DefaultPalette = "─│┌┐└┘├┤";
        
        [JsonIgnore] public string Palette { get; private init; }

        public RGBColor   Color { get; init; }
        public BorderLine Style
        {
            get;
            init
            {
                field = value;
                Palette = GetBorderPalette(value);
            }
        }


        public BorderStyle()
        {
            Palette = DefaultPalette;
        }
        
        
        public static string GetBorderPalette(BorderLine Style)
        {
            return Style switch
            {
                BorderLine.DoubleLine => "═║╔╗╚╝╠╣",
                _                     => DefaultPalette
            };
        }
    }
}
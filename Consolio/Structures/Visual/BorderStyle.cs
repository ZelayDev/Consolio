using System.Text.Json.Serialization;

namespace Consolio
{
    public readonly struct BorderStyle
    {
        [JsonIgnore]public string Palette { get; private init; }

        public RGBColor   Color { get; init; }
        public BorderLine Style
        {
            get;
            init
            {
                field = value;
                Palette = UIHelper.GetBorderPalette(value);
            }
        }
    }
}
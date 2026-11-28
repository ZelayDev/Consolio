using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Consolio
{
    public struct RGBColor
    {
        public byte R, G, B;

        public RGBColor(byte R, byte G, byte B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public sealed class JsonConverter : JsonConverter<RGBColor>
        {
            public override void Write(Utf8JsonWriter Writer, RGBColor Value, JsonSerializerOptions Options)
            {
                Writer.WriteStringValue($"#{Value.R:X2}{Value.G:X2}{Value.B:X2}");
            }

            public override RGBColor Read(ref Utf8JsonReader Reader, Type TypeToConvert, JsonSerializerOptions Options)
            {
                var Raw = Reader.GetString()?.Trim();

                if (string.IsNullOrEmpty(Raw))
                {
                    throw new JsonException("Color string cannot be empty.");
                }

                if (Raw.StartsWith('#'))
                {
                    var Hex = Raw.AsSpan(1);
                    if (Hex.Length == 6 &&
                        byte.TryParse(Hex[..2], NumberStyles.HexNumber, null, out var R) &&
                        byte.TryParse(Hex[2..4], NumberStyles.HexNumber, null, out var G) &&
                        byte.TryParse(Hex[4..6], NumberStyles.HexNumber, null, out var B))
                    {
                        return new RGBColor(R, G, B);
                    }

                    throw new JsonException($"Invalid HEX color: {Raw}");
                }

                if (Raw.StartsWith("rgb", StringComparison.OrdinalIgnoreCase))
                {
                    var OpenBracket  = Raw.IndexOf('(');
                    var CloseBracket = Raw.LastIndexOf(')');

                    if (OpenBracket != -1 && CloseBracket > OpenBracket)
                    {
                        var Inner = Raw.Substring(OpenBracket + 1, CloseBracket - OpenBracket - 1);
                        var Parts = Inner.Split(',', StringSplitOptions.TrimEntries);

                        if (Parts.Length == 3 &&
                            byte.TryParse(Parts[0], out byte R) &&
                            byte.TryParse(Parts[1], out byte G) &&
                            byte.TryParse(Parts[2], out byte B))
                        {
                            return new RGBColor(R, G, B);
                        }
                    }

                    throw new JsonException($"Invalid RGB color: {Raw}");
                }

                throw new JsonException($"Unknown color format: {Raw}. Expected #RRGGBB or rgb(R, G, B).");
            }
        }
    }
}
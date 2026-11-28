using System.Text;

namespace Consolio
{
    public static class StringBuilderExtensions
    {
        extension(StringBuilder Builder)
        {
            public StringBuilder SetCursorX(int X)
            {
                return Builder.Append("\x1b[").Append(X + 1).Append('G');
            }

            public StringBuilder SetCursorY(int Y)
            {
                return Builder.Append("\x1b[").Append(Y + 1).Append('d');
            }

            public StringBuilder SetCursorPosition(int X, int Y)
            {
                return Builder.Append("\x1b[")
                              .Append(Y + 1).Append(';')
                              .Append(X + 1).Append('H');
            }

            public StringBuilder SetCursorPosition(Vector2Int Position)
            {
                return Builder.SetCursorPosition(Position.X, Position.Y);
            }


            public StringBuilder AppendIndicator<T>(string Header, RGBColor HeaderColor, T Value, RGBColor ValueColor, int TotalWidth, RGBColor LeaderDotsColor)
            {
                var String = Value!.ToString()!;
                var LeaderDots = TotalWidth - Header.Length - String.Length;
                
                Builder.SetColor(HeaderColor).Append(Header);
                Builder.SetColor(LeaderDotsColor).Append('.', LeaderDots);
                Builder.SetColor(ValueColor).Append(String);
                
                return Builder;
            }

            public StringBuilder NextLine(int CursorX)
            {
                return Builder.Append("\n\x1b[").Append(CursorX + 1).Append('G');
            }

            public StringBuilder Append(string Text, RGBColor Color)
            {
                return Builder.SetColor(Color).Append(Text);
            }

            public StringBuilder Append(ColorText Text)
            {
                return Builder.Append(Text.Text, Text.Color);
            }


            public StringBuilder DrawBorder(Vector2Int Start, Vector2Int Size, BorderStyle Style)
            {
                var Palette = Style.Palette;
            
                Builder.SetColor(Style.Color)
                       .SetCursorPosition(Start);

                Builder.Append(Palette[2])
                       .Append(Palette[0], Size.X - 2)
                       .Append(Palette[3]);

                var Height = Size.Y - 2;
                var StartX = Start.X;
                var EndX = Start.X + Size.X - 1;
                var VerticalLine = Palette[1];

                for (var i = 0; i < Height; i++)
                {
                    Builder.Append('\n')
                           .SetCursorX(StartX)
                           .Append(VerticalLine)
                           .SetCursorX(EndX)
                           .Append(VerticalLine);
                }

                Builder.Append('\n')
                       .SetCursorX(StartX);

                Builder.Append(Palette[4])
                       .Append(Palette[0], Size.X - 2)
                       .Append(Palette[5]);

                Builder.SetCursorPosition(Start + Vector2Int.OneOne);

                return Builder;
            }
            

            public StringBuilder SetColor(RGBColor Color)
            {
                return Builder.Append($"\x1b[38;2;{Color.R};{Color.G};{Color.B}m");
            }

            public StringBuilder ResetColor()
            {
                return Builder.Append("\x1b[0m");
            }
        }
    }
}
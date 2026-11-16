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
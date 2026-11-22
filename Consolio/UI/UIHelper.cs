using System.Text;

namespace Consolio
{
    public static class UIHelper
    {
        public static void DrawBorder(Vector2Int Start, Vector2Int Size, BorderStyle Style)
        {
            Console.Out.Write(new StringBuilder(512).DrawBorder(Start, Size, Style));
        }
    }
}
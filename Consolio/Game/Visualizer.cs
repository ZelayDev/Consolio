using System.Runtime.CompilerServices;
using System.Text;

namespace Tetris
{
    public sealed class Visualizer
    {
        private readonly Configuration Config;
        private readonly Anchors       Anchors;
        private readonly Tetris        Tetris;
        private readonly Field         Field;

        private readonly string   BorderChars;
        private readonly string[] Palette;


        public Visualizer(Tetris Tetris, Field Field, Configuration Config)
        {
            this.Tetris = Tetris;
            this.Field  = Field;
            this.Config = Config;
            Anchors     = new Anchors(Config);

            BorderChars = GetBorderChars(Config.BorderStyle.Style);
            Palette = Array.ConvertAll(this.Config.Colors, static Color => $"\x1b[38;2;{Color.R};{Color.G};{Color.B}m");
        }


        public void Initialize()
        {
            Tetris.Score.Changed += (_, _) => Update();
            Tetris.Lines.Changed += (_, _) => Update();
            Tetris.Level.Changed += (_, _) => Update();

            Tetris.CurrentBlock.Changed += (_, _) => Update();
            Tetris.Shadow.Changed       += (_, _) => Update();
            Tetris.NextBlock.Changed    += (_, _) => Update();
            
            
            Tetris.Score.Changed += UpdateScore;
            Tetris.Lines.Changed += UpdateLines;
            Tetris.Level.Changed += UpdateLevel;

            Tetris.CurrentBlock.Changed += UpdateBlock;
            Tetris.Shadow.Changed       += UpdateShadow;
            Tetris.NextBlock.Changed    += UpdateNextBlock;

            Anchors.Update();
            DrawUI();
        }

        public void UpdateLinesAbove(int Line)
        {
            Update();
            
            
        }

        
        private void Update()
        {
            if (Anchors.Update())
            {
                DrawUI();
            }
        }
        
        
        private void UpdateBlock(PlacedBlock Old, PlacedBlock New)
        {
        }

        private void UpdateShadow(PlacedBlock Old, PlacedBlock New)
        {
        }

        private void UpdateNextBlock(Block Old, Block New)
        {
        }


        private void UpdateScore(int Old, int New)
        {
            UpdateStatistic(Anchors.ScorePosition, New);
        }

        private void UpdateLines(int Old, int New)
        {
            UpdateStatistic(Anchors.LinesPosition, New);
        }

        private void UpdateLevel(int Old, int New)
        {
            UpdateStatistic(Anchors.LevelPosition, New);
        }
        

        private void DrawUI()
        {
            var Anchors = this.Anchors;

            //Header
            SetCursorPosition(Anchors.HeaderPosition);
            WriteBorder(Anchors.HeaderSize);
            Console.Write(Config.Header);

            //Field
            SetCursorPosition(Anchors.FieldPosition);
            WriteBorder(Anchors.FieldSize);
            UpdateLinesAbove(0);

            //Statistics
            SetCursorPosition(Anchors.StatisticPosition);
            WriteBorder(Anchors.StatisticSize);
            Console.WriteLine("SCORE: ");
            Console.CursorLeft = Anchors.StatisticPosition.X;
            Console.WriteLine("LINES: ");
            Console.CursorLeft = Anchors.StatisticPosition.X;
            Console.WriteLine("LEVEL: ");
            
            //NextBlock
            SetCursorPosition(Anchors.NextBlockPosition);
            WriteBorder(Anchors.NextBlockSize);

            //Keymap
            SetCursorPosition(Anchors.KeymapPosition);
            WriteBorder(Anchors.KeymapSize);
        }


        private void WriteBorder(Vector2Int Size)
        {
            var StartY = Console.CursorTop;
            var LeftSide = Console.CursorLeft;
            var RightSide = LeftSide + Size.X - 1;

            var Palette = BorderChars;
            var Horizontal = Palette[0];
            var Vertical = Palette[1];

            var Builder = new StringBuilder(512);

            void SetX(int X)
            {
                Builder.Append('\x1b').Append('[').Append(X).Append('G');
            }

            SetX(LeftSide);
            Builder.Append(Palette[2]);
            Builder.Append(Horizontal, Size.X - 2);
            Builder.Append(Palette[3]);
            Builder.Append('\n');

            var VerticalLineLength = Size.Y - 2;

            for (var i = 0; i < VerticalLineLength; i++)
            {
                SetX(LeftSide);
                Builder.Append(Vertical);

                SetX(RightSide);
                Builder.Append(Vertical);
                Builder.Append('\n');
            }

            SetX(LeftSide);
            Builder.Append(Palette[4]);
            Builder.Append(Horizontal, Size.X - 2);
            Builder.Append(Palette[5]);

            SetColor(Config.BorderStyle.Color);

            Console.Out.Write(Builder);
            Console.SetCursorPosition(LeftSide, StartY + 1);

            ResetColor();
        }

        private void UpdateStatistic(Vector2Int Position, int New)
        {
            SetCursorPosition(Position);
            Console.Write(New.ToString().PadLeft(9, '.'));
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetCursorPosition(Vector2Int Position)
        {
            Console.SetCursorPosition(Position.X, Position.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetColor(RGBColor Color)
        {
            Console.Write($"\x1b[38;2;{Color.R};{Color.G};{Color.B}m");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ResetColor()
        {
            Console.Write("\x1b[0m");
        }


        private static string GetBorderChars(BorderLine Line)
        {
            return Line switch
            {
                BorderLine.DoubleLine => "═║╔╗╚╝",
                BorderLine.DottedLine => "┅┋┏┓┗┛",
                _                     => "─│┌┐└┘"
            };
        }
    }
}
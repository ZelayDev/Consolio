using System.Runtime.CompilerServices;
using System.Text;

namespace Consolio.Tetris
{
    public sealed class Visualizer
    {
        private readonly Config  Config;
        private readonly Anchors Anchors;
        private readonly Tetris  Tetris;
        private readonly Field   Field;

        private readonly string   BorderChars;
        private readonly string[] Palette;


        public Visualizer(Tetris Tetris, Field Field, Config Config)
        {
            this.Tetris = Tetris;
            this.Field  = Field;
            this.Config = Config;
            Anchors     = new Anchors(Config);

            BorderChars = GetBorderChars(Config.Theme.BorderStyle.Style);
            Palette     = Array.ConvertAll(this.Config.Theme.Colors, static Color => $"\x1b[38;2;{Color.R};{Color.G};{Color.B}m");
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
            Tetris.Shadow.Changed       += UpdateBlock;
            Tetris.NextBlock.Changed    += UpdateNextBlock;

            Tetris.OnBlockPlaced += UpdateBlockCount;

            Anchors.Update();
            DrawUI();
        }

        public void UpdateLinesAbove(int Line)
        {
            Update();

            var Field   = this.Field;
            var Palette = this.Palette;
            var Config  = this.Config;
            var Anchors = this.Anchors;

            var Size  = Field.Size;
            var Start = Anchors.FieldContent;

            var BlockView = Config.Block;
            var EmptyView = Config.Empty;

            var CursorX = $"\x1b[{Start.X + 1}G";
            var Builder = new StringBuilder(512);

            var LastColor = -1;

            void DrawLine(int Y)
            {
                for (var i = 0; i < Size.X; i++)
                {
                    var Cell = Field[i, Y];
                    var View = Cell != 0 ? BlockView : EmptyView;

                    if (Cell != LastColor)
                    {
                        Builder.Append(Palette[Cell]);
                        LastColor = Cell;
                    }

                    Builder.Append(View);
                }
            }

            Builder.SetCursorPosition(Start);

            for (var i = Size.Y - 1; i >= Line; i--)
            {
                DrawLine(i);
                Builder.Append('\n');
                Builder.Append(CursorX);
            }

            Builder.ResetColor();

            Console.Out.Write(Builder);
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
            if (New.Block.Id == Old.Block.Id)
            {
                RemoveBlock(Old);
            }
            DrawBlock(New);
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

        private void UpdateBlockCount(int Index, int Count)
        {
            var String   = Count.ToString();
            var Position = Anchors.BlockStatisticContent + new Vector2Int(15 - String.Length, Index * 2 + 1);
            var Builder  = new StringBuilder(64);

            Builder.SetCursorPosition(Position).SetColor(Config.Theme.IndicatorColor).Append(String).ResetColor();
            Console.Out.Write(Builder);
        }


        private void DrawUI()
        {
            var Anchors = this.Anchors;

            //Header
            SetCursorPosition(Anchors.HeaderPosition);
            WriteBorder(Anchors.HeaderSize);
            Console.Write(Config.Theme.Header);

            //Field
            SetCursorPosition(Anchors.FieldPosition);
            WriteBorder(Anchors.FieldSize);
            UpdateLinesAbove(0);

            //Statistics
            SetCursorPosition(Anchors.StatisticPosition);
            WriteBorder(Anchors.StatisticSize);
            DrawStatisticUI();

            //BlockStatistic
            SetCursorPosition(Anchors.BlockStatisticPosition);
            WriteBorder(Anchors.BlockStatisticSize);
            DrawBlockStatisticUI(Anchors.BlockStatisticContent.X);

            //NextBlock
            SetCursorPosition(Anchors.NextBlockPosition);
            WriteBorder(Anchors.NextBlockSize);

            //Keymap
            SetCursorPosition(Anchors.KeymapPosition);
            WriteBorder(Anchors.KeymapSize);
        }

        private void DrawStatisticUI()
        {
            var CursorX = Anchors.StatisticPosition.X;
            var Theme   = Config.Theme;
            var Builder = new StringBuilder(128);

            Builder.SetColor(Theme.ScoreColor).Append("SCORE: \n").SetCursorX(CursorX);
            Builder.SetColor(Theme.LinesColor).Append("LINES: \n").SetCursorX(CursorX);
            Builder.SetColor(Theme.LevelColor).Append("LEVEL: ");

            Console.Out.Write(Builder);
        }

        private void DrawBlockStatisticUI(int CursorX)
        {
            var Builder        = new StringBuilder(256);
            var Colors         = Palette.AsSpan(2);
            var IndicatorColor = Config.Theme.IndicatorColor;

            void AddBlock(Span<string> Colors, string Block, int ColorIndex)
            {
                Builder.Append(Colors[ColorIndex % Colors.Length])
                       .Append(Block)
                       .SetColor(IndicatorColor)
                       .Append("..........0\n\n")
                       .SetCursorX(CursorX);
            }

            Builder.Append("\n").SetCursorX(CursorX);

            AddBlock(Colors, "⣿⣿  ", 0);
            AddBlock(Colors, "⣤⣿⣤ ", 1);
            AddBlock(Colors, "⣤⣤⣿ ", 2);
            AddBlock(Colors, "⣿⣤⣤ ", 3);
            AddBlock(Colors, "⣤⣿⠛ ", 4);
            AddBlock(Colors, "⠛⣿⣤ ", 5);
            AddBlock(Colors, "⣤⣤⣤⣤", 6);

            Builder.ResetColor();

            Console.Out.Write(Builder);
        }


        private void WriteBorder(Vector2Int Size)
        {
            var StartY    = Console.CursorTop;
            var LeftSide  = Console.CursorLeft;
            var RightSide = LeftSide + Size.X - 1;

            var Palette    = BorderChars;
            var Horizontal = Palette[0];
            var Vertical   = Palette[1];

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

            SetColor(Config.Theme.BorderStyle.Color);

            Console.Out.Write(Builder);
            Console.SetCursorPosition(LeftSide, StartY + 1);

            ResetColor();
        }

        private void UpdateStatistic(Vector2Int Position, int New)
        {
            var Builder = new StringBuilder(64);
            Builder.SetCursorPosition(Position)
                   .SetColor(Config.Theme.IndicatorColor)
                   .Append(New.ToString().PadLeft(9, '.'))
                   .ResetColor();
            Console.Out.Write(Builder);
        }

        private void RemoveBlock(PlacedBlock Block)
        {
            DrawBlock(Block, Config.Empty, Palette[0]);
        }

        private void DrawBlock(PlacedBlock Block)
        {
            DrawBlock(Block, Config.Block, Palette[Block.Block.Color]);
        }

        private void DrawBlock(PlacedBlock Block, string View, string Color)
        {
            var Height  = Field.Height;
            var Builder = new StringBuilder(64);

            Builder.Append(Color);

            foreach (var Cell in Block.EnumerateAbsolutePositions())
            {
                if (Cell.Y < Height)
                {
                    Builder.SetCursorPosition(GetCursorPosition(Cell));
                    Builder.Append(View);
                }
            }

            Builder.ResetColor();
            Console.Out.Write(Builder);
        }


        private Vector2Int GetCursorPosition(Vector2Int Cell)
        {
            var Anchors     = this.Anchors;
            var Size        = Field.Size;
            var BlockLength = Config.Block.Length;

            var X = Anchors.FieldContent.X + Cell.X * BlockLength;
            var Y = Anchors.FieldContent.Y + (Size.Y - 1 - Cell.Y);

            return new Vector2Int(X, Y);
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
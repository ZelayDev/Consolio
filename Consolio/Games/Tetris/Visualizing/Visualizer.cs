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

        private readonly string BlockView;
        private readonly string EmptyView;


        public Visualizer(Tetris Tetris, Field Field, Config Config)
        {
            this.Tetris = Tetris;
            this.Field  = Field;
            this.Config = Config;

            BlockView = Config.Block;
            EmptyView = Config.Empty;
            
            Anchors     = new Anchors(Config);
            
            BorderChars = GetBorderChars(Config.Theme.BorderStyle.Style);
            Palette     = Array.ConvertAll(this.Config.Theme.Colors, static Color => $"\x1b[38;2;{Color.R};{Color.G};{Color.B}m");
        }


        public void Initialize()
        {
            Tetris.Paused.Changed += Update;
            Tetris.Score.Changed += Update;
            Tetris.Lines.Changed += Update;
            Tetris.Level.Changed += Update;

            Tetris.CurrentBlock.Changed += Update;
            Tetris.Shadow.Changed       += Update;
            Tetris.NextBlocksChanged    += _ => Update();

            Tetris.Paused.Changed += UpdatePaused;
            Tetris.Score.Changed += UpdateScore;
            Tetris.Lines.Changed += UpdateLines;
            Tetris.Level.Changed += UpdateLevel;

            Tetris.CurrentBlock.Changed += UpdateBlock;
            Tetris.Shadow.Changed       += UpdateBlock;
            Tetris.NextBlocksChanged    += UpdateNextBlocks;

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
            
            var BlockView = this.BlockView;
            var EmptyView = this.EmptyView;

            var Size  = Field.Size;
            var Start = Anchors.FieldContent;


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


        private void Update<T>(T A, T B)
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
            if (New.Block.Id == Old.Block.Id)
            {
                RemoveBlock(Old);
            }
            DrawBlock(New);
        }

        private void UpdateNextBlocks(Queue<Block> Queue)
        {
            
        }


        private void UpdateScore(int Old, int New)
        {
            UpdateIndicator(Anchors.ScorePosition, New);
        }

        private void UpdateLines(int Old, int New)
        {
            UpdateIndicator(Anchors.LinesPosition, New);
        }

        private void UpdateLevel(int Old, int New)
        {
            UpdateIndicator(Anchors.LevelPosition, New);
        }

        private void UpdateBlockCount(int Index, int Count)
        {
            var String   = Count.ToString();
            var Position = Anchors.BlockStatisticContent + new Vector2Int(Anchors.BlockStatisticSize.X - 4 - String.Length, Index * 2 + 1);
            var Builder  = new StringBuilder(64);

            Builder.SetCursorPosition(Position).SetColor(Config.Theme.IndicatorColor).Append(String).ResetColor();
            Console.Out.Write(Builder);
        }

        private void UpdatePaused(bool Old, bool New)
        {
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
            DrawKeymap();
        }

        private void DrawStatisticUI()
        {
            var CursorX = Anchors.StatisticPosition.X;
            var Theme   = Config.Theme;
            var Builder = new StringBuilder(128);

            void AppendIndicator(string Indicator, RGBColor Color, bool IsNotLast = true)
            {
                Builder.SetColor(Color).Append(Indicator);
                Builder.SetColor(Theme.LeaderDots).Append('.', 9);
                if (IsNotLast)
                {
                    Builder.Append('\n').SetCursorX(CursorX);
                }
            }

            AppendIndicator("SCORE: ", Theme.ScoreColor);
            AppendIndicator("LINES: ", Theme.LinesColor);
            AppendIndicator("LEVEL: ", Theme.LevelColor, false);

            Console.Out.Write(Builder);
        }

        private void DrawBlockStatisticUI(int CursorX)
        {
            var Builder         = new StringBuilder(256);
            var Colors          = Palette.AsSpan(2);
            var Theme           = Config.Theme;
            var LeaderDotsColor = Theme.LeaderDots;
            var IndicatorColor  = Theme.IndicatorColor;

            void AddBlock(Span<string> Colors, int ColorIndex)
            {
                Builder.Append(Colors[ColorIndex % Colors.Length])
                       .Append(Theme.Blocks[ColorIndex])
                       .SetColor(LeaderDotsColor)
                       .Append(" .........")
                       .SetColor(IndicatorColor)
                       .Append("0\n\n")
                       .SetCursorX(CursorX);
            }

            Builder.Append("\n").SetCursorX(CursorX);

            AddBlock(Colors, 0);
            AddBlock(Colors, 1);
            AddBlock(Colors, 2);
            AddBlock(Colors, 3);
            AddBlock(Colors, 4);
            AddBlock(Colors, 5);
            AddBlock(Colors, 6);

            Builder.ResetColor();

            Console.Out.Write(Builder);
        }

        private void DrawKeymap()
        {
            var Keymap  = Config.Keymap;
            var Builder = new StringBuilder(256);
            
            var CursorX         = Anchors.KeymapPosition.X;
            var IconColor       = Config.Theme.KeymapIcons;
            var BindsColor      = Config.Theme.KeymapBinds;
            var LeaderDotsColor = Config.Theme.LeaderDots;
            
            var TotalWidth = Anchors.KeymapSize.X - 4;
            
            void AppendBinds(char Action, ConsoleKey[] Keys, bool IsNotLast = true)
            {
                var Count = Math.Min(8, Keys.Length);
                
                Builder.SetColor(IconColor)
                       .Append(Action);

                Builder.Append(' ');
                
                Builder.SetColor(LeaderDotsColor)
                       .Append('.', TotalWidth - Count * 2 - 2);
                
                Builder.Append(' ');
                
                if (Count >= 1)
                {
                    Builder.SetColor(BindsColor);
                    Builder.Append(Keys[0].ToChar());
                }
                
                for (int i = 1; i < Count; i++)
                {
                    Builder.Append(' ');
                    Builder.Append(Keys[i].ToChar());
                }

                if (IsNotLast)
                {
                    Builder.Append('\n').SetCursorX(CursorX);
                }
            }

            Builder.Append(' ');
            
            AppendBinds('◀', Keymap.MoveLeft);
            AppendBinds('▶', Keymap.MoveRight);
            AppendBinds('↺', Keymap.RotateLeft);
            AppendBinds('↻', Keymap.RotateRight);
            AppendBinds('⤓', Keymap.HardDrop);
            AppendBinds('⏸', Keymap.Pause);
            AppendBinds('▷', Keymap.Resume);
            AppendBinds('✕', Keymap.Exit, false);

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

        private void UpdateIndicator(Vector2Int Position, int Value)
        {
            var Builder = new StringBuilder(64);
            var String  = Value.ToString();
            
            Builder.SetCursorPosition(new Vector2Int(Position.X + 9 - String.Length, Position.Y))
                   .SetColor(Config.Theme.IndicatorColor)
                   .Append(String)
                   .ResetColor();
            
            Console.Out.Write(Builder);
        }

        private void RemoveBlock(PlacedBlock Block)
        {
            DrawBlock(Block, EmptyView, Palette[0]);
        }

        private void DrawBlock(PlacedBlock Block)
        {
            DrawBlock(Block, BlockView, Palette[Block.Block.Color]);
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
            var BlockLength = BlockView.Length;

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
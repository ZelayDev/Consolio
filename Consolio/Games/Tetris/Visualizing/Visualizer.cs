using System.Text;

namespace Consolio.Tetris
{
    public sealed class Visualizer
    {
        private readonly Config  Config;
        private readonly Anchors Anchors;
        private readonly Tetris  Tetris;
        private readonly Field   Field;

        private readonly BorderStyle BorderStyle;
        private readonly string[]    Palette;

        private readonly string BlockView;
        private readonly string EmptyView;


        public Visualizer(Tetris Tetris, Field Field, Config Config)
        {
            this.Tetris = Tetris;
            this.Field = Field;
            this.Config = Config;

            BlockView = Config.Block;
            EmptyView = Config.Empty;

            Anchors = new Anchors(Config);

            BorderStyle = Config.Theme.BorderStyle;
            Palette = Array.ConvertAll(this.Config.Theme.Colors, static Color => $"\x1b[38;2;{Color.R};{Color.G};{Color.B}m");
        }


        public void Initialize()
        {
            Tetris.Paused.Changed += Update;
            Tetris.Score.Changed += Update;
            Tetris.Lines.Changed += Update;
            Tetris.Level.Changed += Update;

            Tetris.CurrentBlock.Changed += Update;
            Tetris.Shadow.Changed += Update;
            Tetris.NextBlocksChanged += _ => Update();

            Tetris.Paused.Changed += UpdatePaused;
            Tetris.Score.Changed += UpdateScore;
            Tetris.Lines.Changed += UpdateLines;
            Tetris.Level.Changed += UpdateLevel;

            Tetris.CurrentBlock.Changed += UpdateBlock;
            Tetris.Shadow.Changed += UpdateBlock;
            Tetris.NextBlocksChanged += UpdateNextBlocks;
            Tetris.OnBlockPlaced += UpdateBlockCount;

            Anchors.Update();
            DrawUI();
        }

        public void UpdateLinesAbove(int Line)
        {
            Update();

            var Field = this.Field;
            var Palette = this.Palette;
            var Anchors = this.Anchors;

            var BlockView = this.BlockView;
            var EmptyView = this.EmptyView;

            var Size = Field.Size;
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
                Console.Clear();
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
            var String = Count.ToString();
            var Position = Anchors.BlockStatisticContent + new Vector2Int(-String.Length, Index * 2 + 1);
            var Builder = new StringBuilder(64);

            Builder.SetCursorPosition(Position).SetColor(Config.Theme.IndicatorColor).Append(String).ResetColor();
            Console.Out.Write(Builder);
        }

        private void UpdatePaused(bool Old, bool New)
        {
        }


        private void DrawUI()
        {
            var Anchors = this.Anchors;
            var BorderStyle = this.BorderStyle;

            //Header
            UIHelper.DrawBorder(Anchors.HeaderPosition, Anchors.HeaderSize, BorderStyle);
            Console.Write(Config.Theme.Header);

            //Field
            UIHelper.DrawBorder(Anchors.FieldPosition, Anchors.FieldSize, BorderStyle);
            UpdateLinesAbove(0);

            //Statistics
            UIHelper.DrawBorder(Anchors.StatisticPosition, Anchors.StatisticSize, BorderStyle);
            DrawStatisticUI();

            //BlockStatistic
            UIHelper.DrawBorder(Anchors.BlockStatisticPosition, Anchors.BlockStatisticSize, BorderStyle);
            DrawBlockStatisticUI();

            //NextBlock
            UIHelper.DrawBorder(Anchors.NextBlocksPosition, Anchors.NextBlockSize, BorderStyle);

            //Keymap
            UIHelper.DrawBorder(Anchors.KeymapPosition, Anchors.KeymapSize, BorderStyle);
            DrawKeymap();
        }

        private void DrawStatisticUI()
        {
            var Theme = Config.Theme;
            var Builder = new StringBuilder(128);

            var CursorX = Anchors.StatisticPosition.X + 1;
            var TotalWidth = Anchors.StatisticSize.X - 2;
            
            var LeaderDots = Theme.LeaderDots;
            var IndicatorColor = Theme.IndicatorColor;

            void AppendIndicator(string Header, RGBColor HeaderColor, Observable<int> Value)
            {
                Builder.AppendIndicator(Header, HeaderColor, Value.Value, IndicatorColor, TotalWidth, LeaderDots);
            }

            AppendIndicator("SCORE: ", Theme.ScoreColor, Tetris.Score);
            Builder.NextLine(CursorX);
            AppendIndicator("LINES: ", Theme.LinesColor, Tetris.Lines);
            Builder.NextLine(CursorX);
            AppendIndicator("LEVEL: ", Theme.LevelColor, Tetris.Level);
            Builder.ResetColor();

            Console.Out.Write(Builder);
        }

        private void DrawBlockStatisticUI()
        {
            var Builder = new StringBuilder(512);
            var Theme = Config.Theme;
            var Colors = Theme.Colors.AsSpan(2);
            var Blocks = Theme.Blocks;
            
            var CursorX = Anchors.BlockStatisticPosition.X + 1;
            
            var LeaderDotsColor = Theme.LeaderDots;
            var IndicatorColor = Theme.IndicatorColor;

            var Statistic = Tetris.BlockStatistic;
            var TotalWidth = Anchors.BlockStatisticSize.X - 2;

            var NextLine = $"\n\n\x1b[{CursorX + 1}G";
            
            void AddBlock(Span<RGBColor> Colors, int Index, int Value, bool IsNotLast = true)
            {
                var Color = Colors[Index % Colors.Length];
                var Block = Blocks[Index];
                
                Builder.AppendIndicator(Block, Color, Value, IndicatorColor, TotalWidth, LeaderDotsColor);

                if (IsNotLast)
                {
                    Builder.Append(NextLine);
                }
            }

            Builder.NextLine(CursorX);

            AddBlock(Colors, 0, Statistic[0]);
            AddBlock(Colors, 1, Statistic[1]);
            AddBlock(Colors, 2, Statistic[2]);
            AddBlock(Colors, 3, Statistic[3]);
            AddBlock(Colors, 4, Statistic[4]);
            AddBlock(Colors, 5, Statistic[5]);
            AddBlock(Colors, 6, Statistic[6], false);

            Builder.ResetColor();

            Console.Out.Write(Builder);
        }

        private void DrawKeymap()
        {
            var Keymap = Config.Keymap;
            var Builder = new StringBuilder(256);

            var CursorX = Anchors.KeymapPosition.X + 2;
            var HeaderColor = Config.Theme.KeymapIcons;
            var BindsColor = Config.Theme.KeymapBinds;
            var LeaderDotsColor = Config.Theme.LeaderDots;

            var TotalWidth = Anchors.KeymapSize.X - 3;

            void AppendBinds(string Header, ConsoleKey[] Keys, bool IsNotLast = true)
            {
                var Count = Math.Min(8, Keys.Length);
                var LeaderDots = TotalWidth - Header.Length - Count * 2;
                
                Builder.SetColor(HeaderColor).Append(Header);
                Builder.SetColor(LeaderDotsColor).Append('.', LeaderDots);
                Builder.SetColor(BindsColor);
                
                if (Count >= 1)
                {
                    Builder.SetColor(BindsColor);
                    Builder.Append(Keys[0].ToChar());
                }

                for (var i = 1; i < Count; i++)
                {
                    Builder.Append(' ');
                    Builder.Append(Keys[i].ToChar());
                }

                if (IsNotLast)
                {
                    Builder.NextLine(CursorX);
                }
            }

            Builder.Append(' ');

            AppendBinds("◀", Keymap.MoveLeft);
            AppendBinds("▶", Keymap.MoveRight);
            AppendBinds("↺", Keymap.RotateLeft);
            AppendBinds("↻", Keymap.RotateRight);
            AppendBinds("⤓", Keymap.HardDrop);
            AppendBinds("⏸", Keymap.Pause);
            AppendBinds("▷", Keymap.Resume);
            AppendBinds("✕", Keymap.Exit, false);

            Builder.ResetColor();

            Console.Out.Write(Builder);
        }


        private void UpdateIndicator(Vector2Int Position, int Value)
        {
            var Builder = new StringBuilder(64);
            var String = Value.ToString();

            Builder.SetCursorPosition(Position.X - String.Length, Position.Y)
                   .Append(String, Config.Theme.IndicatorColor)
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
            var Height = Field.Height;
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
            var Anchors = this.Anchors;
            var Size = Field.Size;
            var BlockLength = BlockView.Length;

            var X = Anchors.FieldContent.X + Cell.X * BlockLength;
            var Y = Anchors.FieldContent.Y + (Size.Y - 1 - Cell.Y);

            return new Vector2Int(X, Y);
        }
    }
}
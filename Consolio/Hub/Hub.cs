using System.Text;

namespace Consolio
{
    public sealed class Hub : IScreen
    {
        #region Constants & StaticFields
        private const string SectionTitle = "Games";

        private static readonly GameFactory[] GameFactories =
        [
            new("Tetromino", static () => new Tetromino.Tetromino()),
            new("Snake", static () => new Snake.Snake())
        ];

        private static readonly string[] HeaderLines =
        [
            @"⣾⠟⠛⠛⣷",
            @"⣿⠀⠀⠀⠀⢰⡿⠛⠛⢿⡆⣿⠞⠛⠻⣦⢰⡿⠛⠛⢿⡆⣾⠟⠛⠻⣷⠀⣿⠀⠛⠀⣾⠟⠛⠻⣷",
            @"⣿⠀⠀⠀⠀⢸⡇⠀⠀⢸⡇⣿⠀⠀⠀⣿⠸⣧⣀⣀⡈⠁⣿⠀⠀⠀⣿⠀⣿⠀⣿⠀⣿⠀⠀⠀⣿",
            @"⣿⠀⠀⠀⣀⢸⡇⠀⠀⢸⡇⣿⠀⠀⠀⣿⢀⡈⠉⠉⢻⡆⣿⠀⠀⠀⣿⠀⣿⠀⣿⠀⣿⠀⠀⠀⣿",
            @"⢿⣦⣤⣴⡿⠸⣷⣤⣤⣾⠇⣿⠀⠀⠀⣿⠸⣷⣤⣤⣾⠃⢿⣦⣤⣴⡿⠀⣿⠀⣿⠀⢿⣦⣤⣴⡿"
        ];

        private static readonly Vector2Int HeaderSize = new(HeaderLines.Max(static Line => Line.Length), HeaderLines.Length);
        private static readonly Vector2Int MenuSize   = new(GetLongestNameLength(GameFactories) + 4, GameFactories.Length);
        #endregion

        #region Fields
        private readonly HubTheme        Theme;
        private readonly Observable<int> Selection;

        private Vector2Int ScreenSize;
        private Vector2Int Start;
        
        private bool Running;
        #endregion

        #region Properties
        public bool AutoScreenUpdate => true;
        #endregion

        #region Constructors
        public Hub()
        {
            Theme     = LoadTheme();
            Selection = new Observable<int>();
            Running   = true;

            Selection.Changed += UpdateSelection;
            
            ScreenSizeObserver.Push(this);
        }
        #endregion

        #region Running
        public static async Task Main(string[] Arguments)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.Write("\x1b[?1049h");
            Console.CursorVisible = false;

            await new Hub().Run();

            Console.Write("\x1b[?1049l");
            Console.CursorVisible = true;
        }

        public async Task Run()
        {
            while (Running)
            {
                ScreenSizeObserver.Check();
                await HandleKey(Console.ReadKey(true).Key);
            }
        }
        #endregion

        #region KeyHandling
        private async Task HandleKey(ConsoleKey Key)
        {
            switch (Key)
            {
                case ConsoleKey.DownArrow:
                    MoveSelectionDown();
                    break;

                case ConsoleKey.UpArrow:
                    MoveSelectionUp();
                    break;

                case ConsoleKey.Enter:
                    await RunGame();
                    break;

                case ConsoleKey.Escape or ConsoleKey.Backspace or ConsoleKey.Q:
                    Running = false;
                    break;
            }
        }
        
        private async Task RunGame()
        {
            try
            {
                var Game = GameFactories[Selection.Value].Factory();

                ScreenSizeObserver.Push(Game.Screen);

                await Game.Run();

                ScreenSizeObserver.Pop();
                ScreenSizeObserver.Update();
            }
            catch (Exception Exception)
            {
                Console.Write("\x1b[?1049l");
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(Exception.GetType().FullName);
                Console.WriteLine(Exception.Message);
                Console.WriteLine(Exception.StackTrace);
            }
        }

        private void MoveSelectionDown()
        {
            var NewSelection = Selection.Value + 1;
            if (NewSelection >= GameFactories.Length)
            {
                NewSelection = 0;
            }
            Selection.Value = NewSelection;
        }

        private void MoveSelectionUp()
        {
            var NewSelection = Selection.Value - 1;
            if (NewSelection < 0)
            {
                NewSelection = GameFactories.Length - 1;
            }
            Selection.Value = NewSelection;
        }
        #endregion

        #region Visual
        public void OnScreenSizeChanged(int Width, int Height)
        {
            ScreenSize = new Vector2Int(Width, Height);
            Console.Clear();
            DrawMenu();
        }
        

        private void DrawMenu()
        {
            ScreenSizeObserver.Check();

            var Theme      = this.Theme;
            var ScreenSize = this.ScreenSize;
            var Builder    = new StringBuilder(512);

            var SeparatorWidth = MenuSize.X + 2;

            var HeaderStart    = new Vector2Int((ScreenSize.X - HeaderSize.X) / 2, 0);
            var SectionStart   = new Vector2Int((ScreenSize.X - SectionTitle.Length) / 2, HeaderSize.Y + 1);
            var SeparatorStart = new Vector2Int((ScreenSize.X - SeparatorWidth) / 2, 0);
            var MenuStart      = new Vector2Int((ScreenSize.X - MenuSize.X - 1) / 2, SectionStart.Y + 2);

            Start = MenuStart;

            void DrawSeparator(int Y)
            {
                Builder.SetColor(Theme.SeparatorColor)
                       .SetCursorPosition(SeparatorStart.X, Y)
                       .Append('━', SeparatorWidth);
            }

            Builder.SetColor(Theme.HeaderColor);
            Builder.SetCursorX(HeaderStart.X);

            foreach (var Line in HeaderLines)
            {
                Builder.Append(Line);
                Builder.NextLine(HeaderStart.X);
            }

            DrawSeparator(HeaderSize.Y);

            Builder.SetColor(Theme.SectionColor)
                   .SetCursorPosition(SectionStart.X, SectionStart.Y)
                   .Append(SectionTitle);

            DrawSeparator(SectionStart.Y + 1);

            DrawMenu(Builder, MenuStart.X, MenuStart.Y);

            Builder.ResetColor();
            Console.Out.Write(Builder);
        }

        private void DrawMenu(StringBuilder Builder, int StartX, int StartY)
        {
            var Theme     = this.Theme;
            var Selection = this.Selection.Value;
            var Games     = GameFactories;

            var CursorX = StartX + 2;

            Builder.SetCursorY(StartY);

            if (Selection > 0)
            {
                Builder.SetColor(Theme.NameColor);

                for (var i = 0; i < Selection; i++)
                {
                    Builder.SetCursorX(CursorX)
                           .Append(Games[i].Name)
                           .Append('\n');
                }
            }

            Builder.SetColor(Theme.SelectedNameColor)
                   .SetCursorX(StartX)
                   .Append("< ")
                   .Append(Games[Selection].Name)
                   .SetCursorX(Start.X + MenuSize.X)
                   .Append(" >\n");

            if (Selection < GameFactories.Length - 1)
            {
                Builder.SetColor(Theme.NameColor);

                for (var i = Selection + 1; i < Games.Length; i++)
                {
                    Builder.SetCursorX(CursorX)
                           .Append(Games[i].Name)
                           .Append('\n');
                }
            }
        }

        private void UpdateSelection(int Old, int New)
        {
            ScreenSizeObserver.Check();

            var Start = this.Start;
            var Width = MenuSize.X;

            var Builder = new StringBuilder();

            Builder.SetCursorPosition(Start.X, Start.Y + Old)
                   .SetColor(Theme.NameColor)
                   .Append("  ")
                   .Append(GameFactories[Old].Name)
                   .SetCursorX(Start.X + MenuSize.X)
                   .Append("  ");

            Builder.SetCursorPosition(Start.X, Start.Y + New)
                   .SetColor(Theme.SelectedNameColor)
                   .Append("< ")
                   .Append(GameFactories[New].Name)
                   .SetCursorX(Start.X + MenuSize.X)
                   .Append(" >");

            Console.Out.Write(Builder);
        }
        #endregion

        #region HelperMethods
        private static int GetLongestNameLength(GameFactory[] Games)
        {
            return Games.Select(static Game => Game.Name.Length).Max();
        }

        private static HubTheme LoadTheme()
        {
            return HubTheme.Default;
        }
        #endregion
    }
}
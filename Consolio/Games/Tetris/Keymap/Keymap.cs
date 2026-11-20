using static System.ConsoleKey;

namespace Consolio.Tetris
{
    public sealed class Keymap
    {
        public static readonly Keymap Default = new()
        {
            MoveLeft    = [LeftArrow, A],
            MoveRight   = [RightArrow, D],
            RotateLeft  = [Q],
            RotateRight = [UpArrow, E],
            HardDrop    = [Spacebar],
            Pause       = [Escape],
            Resume      = [Enter],
            Exit        = [Backspace]
        };


        private readonly Dictionary<ConsoleKey, TetrisAction>      Map      = new();
        private readonly Dictionary<ConsoleKey, TetrisPauseAction> PauseMap = new();

        public ConsoleKey[] MoveLeft
        {
            get => field ?? [];
            init => field = BindConsoleKeys(TetrisAction.MoveLeft, value);
        }

        public ConsoleKey[] MoveRight
        {
            get => field ?? [];
            init => field = BindConsoleKeys(TetrisAction.MoveRight, value);
        }

        public ConsoleKey[] RotateLeft
        {
            get => field ?? [];
            init => field = BindConsoleKeys(TetrisAction.RotateLeft, value);
        }

        public ConsoleKey[] RotateRight
        {
            get => field ?? [];
            init => field = BindConsoleKeys(TetrisAction.RotateRight, value);
        }

        public ConsoleKey[] HardDrop
        {
            get => field ?? [];
            init => field = BindConsoleKeys(TetrisAction.HardDrop, value);
        }

        public ConsoleKey[] Pause
        {
            get => field ?? [];
            init => field = BindConsoleKeys(TetrisAction.Pause, value);
        }

        public ConsoleKey[] Resume
        {
            get => field ?? [];
            init => field = BindConsoleKeys(TetrisPauseAction.Resume, value);
        }

        public ConsoleKey[] Exit
        {
            get => field ?? [];
            init => field = BindConsoleKeys(TetrisPauseAction.Exit, value);
        }


        private ConsoleKey[] BindConsoleKeys(TetrisAction Action, ConsoleKey[] ConsoleKeys)
        {
            var Map = this.Map;

            foreach (var ConsoleKey in ConsoleKeys)
            {
                Map.Add(ConsoleKey, Action);
            }

            return ConsoleKeys;
        }

        private ConsoleKey[] BindConsoleKeys(TetrisPauseAction Action, ConsoleKey[] ConsoleKeys)
        {
            var Map     = PauseMap;
            var Removed = new List<int>(0);

            for (int i = 0; i < ConsoleKeys.Length; i++)
            {
                var ConsoleKey = ConsoleKeys[i];
                
                if (!Map.TryAdd(ConsoleKey, Action))
                {
                    Removed.Add(i);
                }
            }

            return ConsoleKeys.Remove(Removed);
        }


        public TetrisAction GetAction(ConsoleKey ConsoleKey)
        {
            return Map.GetValueOrDefault(ConsoleKey);
        }

        public TetrisPauseAction GetPauseAction(ConsoleKey ConsoleKey)
        {
            return PauseMap.GetValueOrDefault(ConsoleKey);
        }
    }
}
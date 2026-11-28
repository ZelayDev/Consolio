using static System.ConsoleKey;

namespace Consolio.Tetromino
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


        private readonly Dictionary<ConsoleKey, TetrominoAction>      Map      = new();
        private readonly Dictionary<ConsoleKey, TetrominoPauseAction> PauseMap = new();

        public ConsoleKey[] MoveLeft
        {
            get;
            init => field = BindKeys(value, TetrominoAction.MoveLeft);
        } = [];

        public ConsoleKey[] MoveRight
        {
            get;
            init => field = BindKeys(value, TetrominoAction.MoveRight);
        } = [];

        public ConsoleKey[] RotateLeft
        {
            get;
            init => field = BindKeys(value, TetrominoAction.RotateLeft);
        } = [];

        public ConsoleKey[] RotateRight
        {
            get;
            init => field = BindKeys(value, TetrominoAction.RotateRight);
        } = [];

        public ConsoleKey[] HardDrop
        {
            get;
            init => field = BindKeys(value, TetrominoAction.HardDrop);
        } = [];

        public ConsoleKey[] Pause
        {
            get;
            init => field = BindKeys(value, TetrominoAction.Pause);
        } = [];

        public ConsoleKey[] Resume
        {
            get;
            init => field = BindKeys(value, TetrominoPauseAction.Resume);
        } = [];

        public ConsoleKey[] Exit
        {
            get;
            init => field = BindKeys(value, TetrominoPauseAction.Exit);
        } = [];


        public TetrominoAction GetAction(ConsoleKey ConsoleKey)
        {
            return Map.GetValueOrDefault(ConsoleKey);
        }

        public TetrominoPauseAction GetPauseAction(ConsoleKey ConsoleKey)
        {
            return PauseMap.GetValueOrDefault(ConsoleKey);
        }
        
        
        private ConsoleKey[] BindKeys(ConsoleKey[] Keys, TetrominoAction Action)
        {
            return Map.Add(Keys, Action);
        }

        private ConsoleKey[] BindKeys(ConsoleKey[] Keys, TetrominoPauseAction Action)
        {
            return PauseMap.Add(Keys, Action);
        }
    }
}
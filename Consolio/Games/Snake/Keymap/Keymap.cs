namespace Consolio.Snake
{
    public sealed class Keymap
    {
        public static readonly Keymap Default = new()
        {

        };

        private readonly Dictionary<ConsoleKey, SnakeAction> Map = new();
        private readonly Dictionary<ConsoleKey, SnakePauseAction> PauseMap = new();

        public ConsoleKey[] Left
        {
            get;
            init => field = BindKeys(value, SnakeAction.Left);
        } = [];
        
        public ConsoleKey[] Up
        {
            get;
            init => field = BindKeys(value, SnakeAction.Up);
        } = [];
        
        public ConsoleKey[] Right
        {
            get;
            init => field = BindKeys(value, SnakeAction.Right);
        } = [];
        
        public ConsoleKey[] Down
        {
            get;
            init => field = BindKeys(value, SnakeAction.Down);
        } = [];
        
        public ConsoleKey[] Accelerate
        {
            get;
            init => field = BindKeys(value, SnakeAction.Accelerate);
        } = [];
        
        public ConsoleKey[] Pause
        {
            get;
            init => field = BindKeys(value, SnakeAction.Pause);
        } = [];
        
        public ConsoleKey[] Resume
        {
            get;
            init => field = BindKeys(value, SnakePauseAction.Resume);
        } = [];
        
        public ConsoleKey[] Exit
        {
            get;
            init => field = BindKeys(value, SnakePauseAction.Exit);
        } = [];


        public SnakeAction GetAction(ConsoleKey Key)
        {
            return Map.GetValueOrDefault(Key);
        }
        
        public SnakePauseAction GetPauseAction(ConsoleKey Key)
        {
            return PauseMap.GetValueOrDefault(Key);
        }
        
        
        private ConsoleKey[] BindKeys(ConsoleKey[] Keys, SnakeAction Action)
        {
            return Map.Add(Keys, Action);
        }
        
        private ConsoleKey[] BindKeys(ConsoleKey[] Keys, SnakePauseAction Action)
        {
            return PauseMap.Add(Keys, Action);
        }
    }
}
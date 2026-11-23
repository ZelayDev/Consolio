namespace Consolio
{
    public static class ScreenSizeObserver
    {
        private static readonly Stack<IScreen> Screens = new();
        
        private static int AutoDelay = 200;
        private static int ScreenWidth, ScreenHeight;
        
        public static bool Auto
        {
            get;
            set
            {
                if (field != value)
                {
                    if (value)
                    {
                        Task.Run(Observe);
                    }
                    field = value;
                }
            }
        }


        public static void Push(IScreen Screen)
        {
            Screens.Push(Screen);
            Auto = Screen.AutoScreenUpdate;
        }

        public static void Pop()
        {
            Screens.Pop();
            Auto = Screens.TryPeek(out var Screen) && Screen.AutoScreenUpdate;
        }


        public static void Update()
        {
            if (!Screens.TryPeek(out var Screen))
            {
                return;
            }
            
            ScreenWidth  = Console.WindowWidth;
            ScreenHeight = Console.WindowHeight;
            Screen.OnScreenSizeChanged(ScreenWidth, ScreenHeight);
        }
        
        
        public static void Check()
        {
            if (!Screens.TryPeek(out var Screen))
            {
                return;
            }
            
            var WindowWidth  = Console.WindowWidth;
            var WindowHeight = Console.WindowHeight;

            if (WindowWidth != ScreenWidth || WindowHeight != ScreenHeight)
            {
                ScreenWidth  = WindowWidth;
                ScreenHeight = WindowHeight;
                
                Screen.OnScreenSizeChanged(ScreenWidth, ScreenHeight);
            }
        }

        public static void Check<T>(T Item1)
        {
            Check();
        }
        
        public static void Check<T>(T Item1, T Item2)
        {
            Check();
        }
        

        private static async Task Observe()
        {
            while (Auto)
            {
                Check();
                await Task.Delay(AutoDelay);
            }
        }
    }
}
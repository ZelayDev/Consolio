namespace Consolio.Snake
{
    public sealed class Snake : Game
    {
        #region Data
        private readonly Field           Field;
        private readonly Renderer        Renderer;
        private readonly Config          Config;
        private readonly ExecutableQueue ExecutableQueue;
        private readonly Random          Random;

        public readonly Observable<bool> Paused = new();
        public readonly Observable<int>  Length = new(3);

        private CancellationTokenSource? GameLoopCancellationToken;

        private Vector2Int HeadPosition;
        private Vector2Int TailPosition;
        private Vector2Int FoodPosition;
        
        private Vector2Int Direction;
        
        private int Delay;

        private volatile bool GameOver;
        #endregion

        #region Properties
        protected override string Name => "Snake";

        public override IScreen Screen => Renderer;
        #endregion

        #region Constructors
        public Snake()
        {
            Config          = LoadConfig(Config.Default);
            Field           = new Field(Config.FieldSize);
            Renderer        = new Renderer(this, Config);
            ExecutableQueue = new ExecutableQueue(Config.InputBuffer);
            Random          = new();
        }
        #endregion

        #region OverrideMethods
        public override async Task Run()
        {
            Renderer.Initialize();
            
            Delay = Config.Delay.StartDelay;

            GameLoopCancellationToken = new CancellationTokenSource();
            var Token = GameLoopCancellationToken.Token;

            Task.Run(() => InputLoop(Token));
            var GameTask  = GameLoop(Token);
            var QueueTask = ExecutableQueue.Run(Token);

            try
            {
                await QueueTask;
            }
            catch (OperationCanceledException) { }

            try
            {
                await GameTask;
            }
            catch (OperationCanceledException) { }

            GameLoopCancellationToken?.Dispose();
        }
        #endregion

        #region Loops
        private async Task GameLoop(CancellationToken Token)
        {
            try
            {
                while (!Token.IsCancellationRequested)
                {
                    await Task.Delay(Delay, Token);
                    if (!Paused.Value)
                    {
                        ExecutableQueue.AddPriority(Move);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        private void InputLoop(CancellationToken Token)
        {
            try
            {
                while (!Token.IsCancellationRequested)
                {
                    HandleKey(Console.ReadKey(true).Key);
                    
                    if (GameOver)
                    {
                        Exit();
                        break;
                    }
                }
            }
            catch
            {
                GameLoopCancellationToken?.Cancel();
            }
        }

        private void HandleKey(ConsoleKey Key)
        {
            var Keymap = Config.Keymap;
            
            if (Paused.Value)
            {
                switch (Keymap.GetPauseAction(Key))
                {
                    case SnakePauseAction.Resume:
                        ExecutableQueue.Add(Resume);
                        break;
                    
                    case SnakePauseAction.Exit:
                        GameOver = true;
                        break;
                }
            }

            switch (Keymap.GetAction(Key))
            {
                case SnakeAction.Left:
                    ExecutableQueue.Add(Left);
                    break;
                
                case SnakeAction.Up:
                    ExecutableQueue.Add(Up);                
                    break;
                
                case SnakeAction.Right:
                    ExecutableQueue.Add(Right);                
                    break;
                
                case SnakeAction.Down:
                    ExecutableQueue.Add(Down);                
                    break;
                
                case SnakeAction.Accelerate:
                    ExecutableQueue.Add(Accelerate);
                    break;
                
                case SnakeAction.Pause:
                    ExecutableQueue.Add(Pause);
                    break;
            }
        }

        private void Move()
        {
            return;
            var Field = this.Field;
            
            var NewHeadPosition = HeadPosition + Direction;

            if (Field.IsWithin(NewHeadPosition) && Field.IsEmpty(NewHeadPosition))
            {
                HeadPosition = NewHeadPosition;
                
                if (NewHeadPosition == FoodPosition)
                {
                    Length.Value++;
                    PlaceFood();
                }
                else
                {
                    TailPosition = Field.RemoveTail(TailPosition);
                }
            }
            else
            {
                GameOver = true;
            }
        }
        #endregion

        #region Management
        private void Up()
        {
            Direction = Vector2Int.Up;
        }
        
        private void Right()
        {
            Direction = Vector2Int.Right;
        }
        
        private void Down()
        {
            Direction = Vector2Int.Down;
        }
        
        private void Left()
        {
            Direction = Vector2Int.Left;
        }


        private void Accelerate()
        {
            
        }
        #endregion
        
        #region Pause
        private void Pause()
        {
            if (Paused.Value)
            {
                return;
            }

            ScreenSizeObserver.Auto = true;
            Paused.Value            = true;
        }

        private void Resume()
        {
            if (!Paused.Value)
            {
                return;
            }

            var Delay = Config.Delay.ResumeDelay;

            if (Delay <= 0)
            {
                Paused.Value = false;
            }
            else
            {
                Task.Run
                (async () =>
                    {
                        await Task.Delay(Delay);
                        ScreenSizeObserver.Auto = false;
                        Paused.Value            = false;
                    }
                );
            }
        }
        
        private void Exit()
        {
            GameLoopCancellationToken!.Cancel();
        }
        #endregion
        
        #region HelpMehods
        private void PlaceFood()
        {
            var Field     = this.Field;
            var Random    = this.Random;
            var FieldSize = Field.Size;
                
            Vector2Int Position;

            do
            {
                Position = Random.NextVector2Int(FieldSize);
            } while (!Field.IsEmpty(Position));

            FoodPosition = Position;
        }
        #endregion
    }
}
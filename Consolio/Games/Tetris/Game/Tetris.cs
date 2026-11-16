namespace Consolio.Tetris
{
    public sealed class Tetris
    {
        #region Data
        private readonly Config     Config;
        private readonly Field      Field;
        private readonly Visualizer Visualizer;
        private readonly Random     Random;

        private readonly ExecutableQueue ExecutableQueue;

        public readonly Observable<int> Score = new();
        public readonly Observable<int> Lines = new();
        public readonly Observable<int> Level = new();

        public readonly Observable<PlacedBlock> CurrentBlock = new();
        public readonly Observable<PlacedBlock> Shadow       = new();
        public readonly Observable<Block>       NextBlock    = new();

        private readonly int[]         PlacedBlocks = new int[7];
        public event Action<int, int>? OnBlockPlaced;

        private readonly Vector2Int StartBlockPosition;

        private CancellationTokenSource? GameLoopCancellationToken;

        private int Delay;

        private bool Paused;
        #endregion

        #region Constructors
        public Tetris()
        {
            Config     = LoadConfiguration();
            Field      = new Field(Config.FieldSize);
            Visualizer = new Visualizer(this, Field, Config);
            Random     = new Random();

            ExecutableQueue    = new ExecutableQueue(Config.InputBuffer);
            StartBlockPosition = GetStartBlockPosition(Config.FieldSize);

            CurrentBlock.Changed += UpdateShadow;
            Lines.Changed        += UpdateLevel;
            Level.Changed        += UpdateDelay;
        }
        #endregion

        #region PublicMethods
        public async Task Run()
        {
            Console.Write("\x1b[?1049h");
            Console.CursorVisible = false;

            Visualizer.Initialize();
            CurrentBlock.Value = new PlacedBlock(StartBlockPosition, GetRandomBlock());
            NextBlock.Value    = GetRandomBlock();

            Delay = Config.Delay.StartDelay;

            Score.Value = 0;
            Lines.Value = 0;
            Level.Value = 0;

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

            Console.Write("\x1b[?1049l");
            Console.CursorVisible = true;
            GameLoopCancellationToken.Dispose();
        }
        #endregion

        #region PrivateMethods
        private async Task GameLoop(CancellationToken Token)
        {
            try
            {
                while (!Token.IsCancellationRequested)
                {
                    await Task.Delay(Delay, Token);
                    ExecutableQueue.AddPriority(Lower);
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
                    var Key = Console.ReadKey(true).Key;

                    if (HandleKey(Key))
                    {
                        GameLoopCancellationToken!.Cancel();
                        break;
                    }
                }
            }
            catch (Exception Exception)
            {
                GameLoopCancellationToken?.Cancel();
            }
        }

        private bool HandleKey(ConsoleKey Key)
        {
            switch (Key)
            {
                case ConsoleKey.A:
                    MoveLeft();
                    break;

                case ConsoleKey.D:
                    MoveRight();
                    break;

                case ConsoleKey.Q:
                    RotateLeft();
                    break;

                case ConsoleKey.E:
                    RotateRight();
                    break;
                
                case ConsoleKey.Spacebar:
                    HardDrop();
                    break;
            }

            return false;
        }


        private void Lower()
        {
            var Block       = CurrentBlock.Value;
            var NewPosition = Block.Position;
            NewPosition.Y--;

            if (Field.Overlaps(NewPosition, Block.Block))
            {
                PlaceBlock(Block);
            }
            else
            {
                CurrentBlock.Value = new PlacedBlock(NewPosition, Block.Block);
            }
        }


        private void MoveLeft()
        {
            Move(-1);
        }

        private void MoveRight()
        {
            Move(1);
        }

        private void RotateLeft()
        {
            var Block   = CurrentBlock.Value;
            var Rotated = Block.Block.RotateLeft();
            if (!Field.Overlaps(Block.Position, Rotated))
            {
                CurrentBlock.Value = new PlacedBlock(Block.Position, Rotated);
            }
        }

        private void RotateRight()
        {
            var Block   = CurrentBlock.Value;
            var Rotated = Block.Block.RotateRight();
            if (!Field.Overlaps(Block.Position, Rotated))
            {
                CurrentBlock.Value = new PlacedBlock(Block.Position, Rotated);
            }
        }


        private void HardDrop()
        {
            var Block = CurrentBlock.Value;
            var Final = new PlacedBlock(Shadow.Value.Position, Block.Block);
            CurrentBlock.Value = Final;
            PlaceBlock(Final);
        }
        

        private void Move(int Offset)
        {
            var Block       = CurrentBlock.Value;
            var NewPosition = Block.Position;
            NewPosition.X += Offset;

            if (!Field.Overlaps(NewPosition, Block.Block))
            {
                CurrentBlock.Value = new PlacedBlock(NewPosition, Block.Block);
            }
        }

        private void ChangeBlock()
        {
            CurrentBlock.Value = new PlacedBlock(StartBlockPosition, NextBlock.Value);
            NextBlock.Value    = GetRandomBlock();
        }

        private void PlaceBlock(PlacedBlock Block)
        {
            Field.Add(Block.Position, Block.Block);
            RemoveFullLines(Block.Position, Block.Block);
            ChangeBlock();

            var BlockIndex = Block.Block.Index >> 4;
            var Count      = PlacedBlocks[BlockIndex] + 1;
            PlacedBlocks[BlockIndex] = Count;
            OnBlockPlaced?.Invoke(BlockIndex, Count);
        }


        private void UpdateLevel(int OldLines, int NewLines)
        {
            var Current = Level.Value;
            var Target  = Math.Min(20, NewLines / 10);

            if (Target != Current)
            {
                Level.Value = Target;

                if (Target >= 20)
                {
                    Lines.Changed -= UpdateLevel;
                }
            }
        }

        private void UpdateDelay(int OldLevel, int NewLevel)
        {
            var DelayConfig = Config.Delay;
            Delay = (int)Single.Lerp(DelayConfig.StartDelay, DelayConfig.MinDelay, NewLevel / 20f);
        }

        private void UpdateShadow(PlacedBlock Old, PlacedBlock New)
        {
            if (Old.Block != New.Block || Old.Position.X != New.Position.X)
            {
                Shadow.Value = new PlacedBlock(GetShadowPosition(New), new Block(New.Block.Index, 1, New.Block.Id));
            }
        }


        private void RemoveFullLines(Vector2Int BlockPosition, Block Block)
        {
            var Cleared = Block.EnumerateUniqueYs(BlockPosition)
                               .Where(Field.IsFullLine)
                               .ToList();

            var ClearedCount = Cleared.Count;
            
            if (ClearedCount == 0)
            {
                return;
            }

            Lines.Value += ClearedCount;
            Score.Value += ClearedCount switch
            {
                1 => 100,
                2 => 300,
                3 => 500,
                4 => 1000,
                _ => 0
            };

            Cleared.Sort();

            RemoveLines(Cleared);

            Visualizer.UpdateLinesAbove(Cleared[0]);
        }

        private void RemoveLines(List<int> Lines)
        {
            var Shift     = 0;
            var LineIndex = 0;

            for (var ReadY = 0; ReadY < Field.Height; ReadY++)
            {
                if (LineIndex < Lines.Count && ReadY == Lines[LineIndex])
                {
                    Shift++;
                    LineIndex++;
                    continue;
                }

                var WriteY = ReadY - Shift;
                if (WriteY != ReadY)
                {
                    Field.CopyRow(ReadY, WriteY);
                }
            }

            for (var Y = Field.Height - Shift; Y < Field.Height; Y++)
            {
                Field.ClearRow(Y);
            }
        }


        private Vector2Int GetShadowPosition(PlacedBlock Block)
        {
            var Field    = this.Field;
            var Position = Block.Position;

            while (!Field.Overlaps(Position, Block.Block))
            {
                Position.Y--;
            }

            Position.Y++;

            return Position;
        }

        private Block GetRandomBlock()
        {
            return Block.GetRandomBlock(Random, Config.ColorCount);
        }


        private static Vector2Int GetStartBlockPosition(Vector2Int FieldSize)
        {
            return new Vector2Int((FieldSize.X >> 1) - 1, FieldSize.Y + 2);
        }

        private static Config LoadConfiguration()
        {
            return Config.Default; //TODO
        }
        #endregion
    }
}
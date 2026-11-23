namespace Consolio.Tetromino
{
    public sealed class Tetromino : Game
    {
        #region Data
        private readonly Config          Config;
        private readonly Field           Field;
        private readonly Visualizer      Visualizer;
        private readonly ExecutableQueue ExecutableQueue;

        private readonly IBlockRandomizer BlockRandomizer;

        public readonly Observable<bool> Paused = new();

        public readonly Observable<int> Score = new();
        public readonly Observable<int> Lines = new();
        public readonly Observable<int> Level = new();

        public readonly Observable<PlacedBlock> CurrentBlock = new();
        public readonly Observable<PlacedBlock> Shadow       = new();
        public readonly Queue<Block>            NextBlocks;

        private readonly int[] PlacedBlocks = new int[7];

        public event Action<int, int>?     OnBlockPlaced;
        public event Action<Queue<Block>>? NextBlocksChanged;

        private readonly Vector2Int StartBlockPosition;

        private CancellationTokenSource? GameLoopCancellationToken;

        private int Delay;

        private bool GameOver;
        #endregion

        #region Properties
        protected override string Name => "Tetromino";

        public override IScreen Screen => Visualizer;

        public ReadOnlySpan<int> BlockStatistic => PlacedBlocks;
        #endregion

        #region Constructors
        public Tetromino()
        {
            Config     = LoadConfig<Config>()?.Correct() ?? Config.Default;
            Field      = new Field(Config.FieldSize);
            Visualizer = new Visualizer(this, Field, Config);

            BlockRandomizer = GetRandomizer(Config.RandomizerKind, Config.Theme.Colors.Length);

            ExecutableQueue    = new ExecutableQueue(Config.InputBuffer);
            StartBlockPosition = GetStartBlockPosition(Config.FieldSize);

            NextBlocks = new Queue<Block>(Config.BlockQueue);

            CurrentBlock.Changed += UpdateShadow;
            Lines.Changed        += UpdateLevel;
            Level.Changed        += UpdateDelay;
        }
        #endregion

        #region OverrideMethods
        public override async Task Run()
        {
            Visualizer.Initialize();
            CurrentBlock.Value = new PlacedBlock(StartBlockPosition, BlockRandomizer.Next());
            FillBlockQueue();

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
                        ExecutableQueue.AddPriority(Lower);
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
                    var Key = Console.ReadKey(true).Key;

                    if (GameOver || HandleKey(Key))
                    {
                        Exit();
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
            if (Paused.Value)
            {
                switch (Config.Keymap.GetPauseAction(Key))
                {
                    case TetrisPauseAction.Resume:
                        ExecutableQueue.Add(Resume);
                        break;

                    case TetrisPauseAction.Exit:
                        return true;
                }
            }
            else
            {
                switch (Config.Keymap.GetAction(Key))
                {
                    case TetrisAction.MoveLeft:
                        ExecutableQueue.Add(MoveLeft);
                        break;

                    case TetrisAction.MoveRight:
                        ExecutableQueue.Add(MoveRight);
                        break;

                    case TetrisAction.RotateLeft:
                        ExecutableQueue.Add(RotateLeft);
                        break;

                    case TetrisAction.RotateRight:
                        ExecutableQueue.Add(RotateRight);
                        break;

                    case TetrisAction.HardDrop:
                        ExecutableQueue.Add(HardDrop);
                        break;

                    case TetrisAction.Pause:
                        ExecutableQueue.Add(Pause);
                        break;
                }
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
        #endregion

        #region Movement
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

            if (StartBlockPosition.Y - Block.Position.Y < 2)
            {
                return;
            }

            var Final = new PlacedBlock(Shadow.Value.Position, Block.Block);
            CurrentBlock.Value = Final;
            PlaceBlock(Final);
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
        #endregion

        #region Update
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
        #endregion

        #region LineClearing
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
        #endregion

        #region HelpMethods
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
            var NextBlock = NextBlocks.Dequeue();
            NextBlocks.Enqueue(BlockRandomizer.Next());
            CurrentBlock.Value = new PlacedBlock(StartBlockPosition, NextBlock);
            NextBlocksChanged?.Invoke(NextBlocks);
        }

        private void PlaceBlock(PlacedBlock Block)
        {
            if (!Field.Add(Block.Position, Block.Block))
            {
                Visualizer.ShowGameOver();
                GameOver = true;
                return;
            }

            RemoveFullLines(Block.Position, Block.Block);
            ChangeBlock();

            var BlockIndex = Block.Block.Index >> 4;
            var Count      = PlacedBlocks[BlockIndex] + 1;
            PlacedBlocks[BlockIndex] = Count;
            OnBlockPlaced?.Invoke(BlockIndex, Count);
        }


        private void Exit()
        {
            GameLoopCancellationToken!.Cancel();
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

        private void FillBlockQueue()
        {
            var Queue      = NextBlocks;
            var Randomizer = BlockRandomizer;

            for (var i = 0; i < Queue.Capacity; i++)
            {
                Queue.Enqueue(Randomizer.Next());
            }
        }

        private static Vector2Int GetStartBlockPosition(Vector2Int FieldSize)
        {
            return new Vector2Int((FieldSize.X >> 1) - 1, FieldSize.Y + 2);
        }

        private static IBlockRandomizer GetRandomizer(RandomizerKind Kind, int ColorCount)
        {
            return Kind switch
            {
                RandomizerKind.Bag => new BagRandomizer(ColorCount),
                _                  => new BlockRandomizer(ColorCount)
            };
        }
        #endregion
    }
}
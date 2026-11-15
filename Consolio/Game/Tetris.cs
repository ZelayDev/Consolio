using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Tetris
{
    public sealed class Tetris
    {
        #region Data
        private readonly Configuration Config;
        private readonly Field         Field;
        private readonly Visualizer    Visualizer;
        private readonly Random        Random;

        private readonly ExecutableQueue ExecutableQueue;

        public readonly Observable<PlacedBlock> CurrentBlock = new();
        public readonly Observable<PlacedBlock> Shadow       = new();
        public readonly Observable<Block>       NextBlock    = new();

        public readonly Observable<int> Score = new();
        public readonly Observable<int> Lines = new();
        public readonly Observable<int> Level = new();

        private readonly Vector2Int StartBlockPosition;

        private CancellationTokenSource? GameLoopCancellationToken;

        private bool Paused;
        #endregion

        #region Properties
        private Vector2Int BlockPosition
        {
            get => CurrentBlock.Value.Position;
            set => CurrentBlock.Value = new PlacedBlock(value, CurrentBlock.Value);
        }

        private Block Block
        {
            get => CurrentBlock.Value.Block;
            set => CurrentBlock.Value = new PlacedBlock(CurrentBlock.Value.Position, value);
        }
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

            CurrentBlock.Changed += UpdateShadowPosition;
        }
        #endregion

        #region PublicMethods
        public async Task Run()
        {
            Console.Write("\x1b[?1049h");
            Console.CursorVisible = false;

            Visualizer.Initialize();
            NextBlock.Value = GetRandomBlock();
            ChangeBlock();

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
                    await Task.Delay(Config.Delay, Token);
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
            return false;
        }


        private void Lower()
        {
            var NewPosition = BlockPosition;
            NewPosition.Y--;

            if (Field.Overlaps(NewPosition, Block))
            {
                Field.Add(BlockPosition, Block);
                RemoveFullLines(BlockPosition, Block);
                ChangeBlock();
            }
            else
            {
                BlockPosition = NewPosition;
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
            var Rotated = Block.RotateLeft();
            if (!Field.Overlaps(BlockPosition, Rotated))
            {
                Block = Rotated;
            }
        }

        private void RotateRight()
        {
            var Rotated = Block.RotateRight();
            if (!Field.Overlaps(BlockPosition, Rotated))
            {
                Block = Rotated;
            }
        }


        private void Move(int Offset)
        {
            var NewPosition = BlockPosition;
            NewPosition.X += Offset;

            if (!Field.Overlaps(NewPosition, Block))
            {
                BlockPosition = NewPosition;
            }
        }


        private void ChangeBlock()
        {
            CurrentBlock.Value = new PlacedBlock(StartBlockPosition, NextBlock.Value);
            NextBlock.Value    = GetRandomBlock();
        }


        private void UpdateShadowPosition(PlacedBlock Old, PlacedBlock New)
        {
            Shadow.Value = new PlacedBlock(GetShadowPosition(New), new Block(New.Block, 1));
        }

        private void RemoveFullLines(Vector2Int BlockPosition, Block Block)
        {
            var Cleared = Block.EnumerateUniqueYs(BlockPosition)
                               .Where(Field.IsFullLine)
                               .ToList();

            if (Cleared.Count == 0)
            {
                return;
            }

            Cleared.Sort();

            var Shift        = 0;
            var ClearedIndex = Cleared.Count - 1;

            for (var ReadY = Field.Height - 1; ReadY >= 0; ReadY--)
            {
                if (ClearedIndex >= 0 && ReadY == Cleared[ClearedIndex])
                {
                    Shift++;
                    ClearedIndex--;
                    continue;
                }

                var WriteY = ReadY + Shift;
                if (WriteY != ReadY)
                {
                    Field.CopyRow(ReadY, WriteY);
                }
            }

            for (var Y = 0; Y < Cleared.Count; Y++)
            {
                Field.ClearRow(Y);
            }

            Visualizer.UpdateLinesAbove(Cleared[0]);
        }


        private Vector2Int GetShadowPosition(PlacedBlock Block)
        {
            var Field    = this.Field;
            var Position = Block.Position;

            while (!Field.Overlaps(Position, Block.Block))
            {
                Position.Y--;
            }

            return Position;
        }

        private Block GetRandomBlock()
        {
            return Block.GetRandomBlock(Random, Config.ColorCount);
        }


        private static Vector2Int GetStartBlockPosition(Vector2Int FieldSize)
        {
            return new Vector2Int(FieldSize.X >> 1, FieldSize.Y + 2);
        }

        private static Configuration LoadConfiguration()
        {
            return Configuration.Default; //TODO
        }
        #endregion
    }
}
using System.Collections.Concurrent;

namespace Consolio
{
    public sealed class ExecutableQueue
    {
        private readonly ConcurrentQueue<Action> PriorityActions = new();
        private readonly ConcurrentQueue<Action> Actions         = new();
        private readonly SemaphoreSlim           Signal          = new(0);

        private readonly int Limit;

        private volatile bool _Running;

        private bool Running => _Running;


        public ExecutableQueue(int Limit)
        {
            this.Limit = Limit;
        }
        
        
        public void Add(Action Action)
        {
            if (Actions.Count >= Limit)
            {
                return;
            }
            
            Actions.Enqueue(Action);
            Signal.Release();
        }

        public void AddPriority(Action Action)
        {
            PriorityActions.Enqueue(Action);
            Signal.Release();
        }
        
        
        public async Task Run(CancellationToken Token)
        {
            _Running = true;

            try
            {
                while (!Token.IsCancellationRequested)
                {
                    await Signal.WaitAsync(Token);

                    while (PriorityActions.TryDequeue(out var PriorityAction))
                    {
                        PriorityAction();
                    }

                    if (Actions.TryDequeue(out var NormalAction))
                    {
                        NormalAction();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                
            }
            finally
            {
                _Running = false;
            }
        }
    }
}
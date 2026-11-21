using System.ComponentModel;

namespace Consolio
{
    public sealed class Observable<T>
    {
        public T Value
        {
            get;
            set
            {
                var OldValue = field;
                field = value;
                Changed?.Invoke(OldValue, value);
            }
        }

        public event Action<T, T>? Changed;


        public Observable()
        {
            Value = default!;
        }
        
        public Observable(T Value)
        {
            this.Value = Value;
        }
    }
}
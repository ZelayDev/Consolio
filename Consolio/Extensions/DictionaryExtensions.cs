namespace Consolio
{
    public static class DictionaryExtensions
    {
        extension<TKey, TValue>(Dictionary<TKey, TValue> Dictionary) where TKey : notnull
        {
            public TKey[] Add(TKey[] Keys, TValue Value)
            {
                var Removed = new List<int>(0);

                for (int i = 0; i < Keys.Length; i++)
                {
                    var Key = Keys[i];
                
                    if (!Dictionary.TryAdd(Key, Value))
                    {
                        Removed.Add(i);
                    }
                }

                return Keys.Remove(Removed);
            }
        }
    }
}
namespace Consolio
{
    public static class ArrayExtensions
    {
        extension<T>(T[] Array)
        {
            public T[] Remove(List<int> SortedIndexes)
            {
                if (SortedIndexes.Count == 0)
                {
                    return Array;
                }

                var Result = new T[Array.Length - SortedIndexes.Count];

                var SourceStart      = 0;
                var DestinationStart = 0;

                foreach (var Index in SortedIndexes)
                {
                    var Length = Index - SourceStart;
                    if (Length > 0)
                    {
                        System.Array.Copy(Array, SourceStart, Result, DestinationStart, Length);
                        DestinationStart += Length;
                    }
                    SourceStart = Index + 1;
                }

                var TailLength = Array.Length - SourceStart;
                if (TailLength > 0)
                {
                    System.Array.Copy(Array, SourceStart, Result, DestinationStart, TailLength);
                }

                return Result;
            }
        }
    }
}
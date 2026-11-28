namespace Consolio
{
    public static class RandomExtensions
    {
        extension(Random Random)
        {
            public Vector2Int NextVector2Int(Vector2Int Min, Vector2Int Max)
            {
                return Min + Random.NextVector2Int(Max - Min);
            }
            
            public Vector2Int NextVector2Int(Vector2Int Max)
            {
                return new Vector2Int
                (
                    Random.Next(Max.X),
                    Random.Next(Max.Y)
                );
            }
        }
    }
}
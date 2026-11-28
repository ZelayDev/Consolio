namespace Consolio.Snake
{
    public sealed class Field
    {
        public static readonly Vector2Int MinSize = new(6, 6);
        public static readonly Vector2Int DefaultSize = new(20, 20);
        
        public readonly Vector2Int Size;
        
        public int Width => Size.X;
        public int Height => Size.Y;
        
        public Field(Vector2Int Size)
        {
            this.Size = Size;
        }


        public bool IsWithin(Vector2Int Position)
        {
            return Position >= Vector2Int.Zero && Position < Size;
        }

        public bool IsEmpty(Vector2Int Position)
        {
            throw new NotImplementedException();
        }


        public Vector2Int RemoveTail(Vector2Int Position)
        {
            throw new NotImplementedException();
        }
    }
}
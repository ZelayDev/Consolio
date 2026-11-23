namespace Consolio.Snake
{
    public sealed class Snake : Game
    {
        #region Data
        private readonly Field Field;
        #endregion

        #region Properties
        protected override string Name => "Snake";

        public override IScreen Screen => default!;
        #endregion

        #region Constructors
        public Snake()
        {
            Field = new();
        }
        #endregion

        #region OverrideMethods
        public override async Task Run()
        {
            await Task.Delay(0);
        }
        #endregion
    }
}
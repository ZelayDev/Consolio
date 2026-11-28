namespace Consolio.Snake
{
    public sealed class Renderer : IScreen
    {
        private readonly Snake   Snake;
        private readonly Anchors Anchors;
        private readonly Theme   Theme;
        private readonly Keymap  Keymap;


        public Renderer(Snake Snake, Config Config)
        {
            this.Snake = Snake;
            
            Theme  = Config.Theme;
            Keymap = Config.Keymap;
            
            Anchors    = new(Config);
        }


        public void Initialize()
        {
            
        }
        
        public void OnScreenSizeChanged(int ScreenWidth, int ScreenHeight)
        {
            Anchors.Update(ScreenWidth, ScreenHeight);
            DrawUI();
        }
        

        private void DrawUI()
        {
            
        }
    }
}
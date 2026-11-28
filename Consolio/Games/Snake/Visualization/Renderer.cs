using System.Text;

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
            ScreenSizeObserver.Update();
        }
        
        public void OnScreenSizeChanged(int ScreenWidth, int ScreenHeight)
        {
            Anchors.Update(ScreenWidth, ScreenHeight);
            Console.Clear();
            DrawUI();
        }
        

        private void DrawUI()
        {
            var Anchors = this.Anchors;
            var Theme   = this.Theme;
            var Builder = new StringBuilder(512);

            Builder.DrawBorder(Anchors.HeaderStart, Anchors.HeaderSize, Theme.BorderStyle)
                   .Append(Theme.Header);

            Builder.DrawBorder(Anchors.StatisticStart, Anchors.StatisticSize, Theme.BorderStyle)
                   .AppendIndicator("SCORE", Theme.ScoreColor, Snake.Length.Value, Theme.ScoreColor, Anchors.StatisticSize.X - 2, Theme.LeaderDots);
            
            DrawField(Builder);
            DrawKeymap(Builder);

            Console.Out.Write(Builder);
        }

        private void DrawField(StringBuilder Builder)
        {
            Builder.DrawBorder(Anchors.FieldStart, Anchors.FieldSize, Theme.BorderStyle);
            
            //TODO
        }

        private void DrawKeymap(StringBuilder Builder)
        {
            Builder.DrawBorder(Anchors.KeymapStart, Anchors.KeymapSize, Theme.BorderStyle);
            
            //TODO
        }
    }
}
namespace Consolio
{
    public interface IScreen
    {
        public bool AutoScreenUpdate => false;
        
        public void OnScreenSizeChanged(int ScreenWidth, int ScreenHeight);
    }
}
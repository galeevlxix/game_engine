using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace game_2
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings settings = GameWindowSettings.Default;
            NativeWindowSettings windowSettings = NativeWindowSettings.Default;

            windowSettings.WindowState = WindowState.Maximized;
            windowSettings.Title = "Game";

            settings.IsMultiThreaded = false;

            GameEngine engine = new GameEngine(settings, windowSettings);
            engine.Init();
            engine.Run();
        }
    }
}

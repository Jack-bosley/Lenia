using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;


namespace Lenia.Presentation
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gameWindowSettings = new GameWindowSettings()
            {
                IsMultiThreaded = true,
                UpdateFrequency = double.MaxValue,
            };
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                IsFullscreen = false,
                StartFocused = true,
                Size = new Vector2i(1920, 1080),
                APIVersion = Version.Parse("3.3.0"),
                Title = "Lenia",
                Profile = ContextProfile.Compatability,

            };

            using LeniaWindow gameWindow = new LeniaWindow(gameWindowSettings, nativeWindowSettings);
            gameWindow.Run();
        }
    }
}

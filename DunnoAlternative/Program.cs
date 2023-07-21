using DunnoAlternative.Battle;
using DunnoAlternative.State;
using DunnoAlternative.World;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace DunnoAlternative
{
    internal class Program
    { 
        public const uint LOGIC_UPDATES_PER_SECOND = 30;
        public static float SpeedUp { private get; set; } = 1.0f;
        const uint FPS = 60;

        private static void Main()
        {
            var videoMode = new VideoMode(800, 600);
            var clock = new Clock();
            
            var deltaLogic = clock.ElapsedTime;

            var window = new RenderWindow(videoMode, "DunnoAlternative");
            window.Closed += CloseWindow;
            window.Resized += ResizeWindow;
            window.SetFramerateLimit(FPS);

            var stateManager = new StateManager();
            WorldGenerator worldGenerator = new WorldGenerator(new WorldSettings());
            stateManager.Push(new WorldState(window, stateManager, worldGenerator.players, worldGenerator.tiles));

            while (window.IsOpen)
            {
                window.DispatchEvents();

                deltaLogic += clock.ElapsedTime;
                clock.Restart();

                for (int x = 0; x < 100 && deltaLogic.AsSeconds() > 1.0f / (LOGIC_UPDATES_PER_SECOND * SpeedUp); x++)
                {
                    stateManager.Update(window);
                    deltaLogic -= Time.FromSeconds(1.0f / (LOGIC_UPDATES_PER_SECOND * SpeedUp));
                }

                stateManager.Draw(window);

                window.Display();
                window.Clear();
            }
        }
        private static void CloseWindow(object? sender, EventArgs e)
        {
            if (sender is not null)
            {
                (sender as RenderWindow)!.Close();
            }
            else
            {
                throw new ArgumentNullException(nameof(sender));
            }
        }
        private static void ResizeWindow(object? sender, EventArgs e)
        {
            if (sender is not null)
            {
                var window = (sender as RenderWindow)!;

                window.SetView(new View(new FloatRect(0,0,window.Size.X,window.Size.Y)));
            }
            else
            {
                throw new ArgumentNullException(nameof(sender));
            }
        }
    }
}

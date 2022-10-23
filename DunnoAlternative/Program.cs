using DunnoAlternative.State;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative
{
    internal class Program
    { 
        const uint LOGIC_UPDATES_PER_SECOND = 30;
        const uint FPS = 60;

        private static void Main()
        {
            var videoMode = new VideoMode(1920, 1080);
            var clock = new Clock();

            var deltaLogic = clock.ElapsedTime;

            var window = new RenderWindow(videoMode, "DunnoAlternative");
            window.Closed += CloseWindow;
            window.SetFramerateLimit(FPS);

            var statehandler = new StateHandler(new BattleState());

            while (window.IsOpen)
            {
                window.DispatchEvents();

                deltaLogic += clock.ElapsedTime;
                clock.Restart();

                while (deltaLogic.AsSeconds() > 1.0f / LOGIC_UPDATES_PER_SECOND)
                {
                    statehandler.Update();
                    deltaLogic -= Time.FromSeconds(1.0f / LOGIC_UPDATES_PER_SECOND);
                }

                statehandler.Draw();

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
    }
}

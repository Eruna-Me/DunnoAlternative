using ErunaInput;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ErunaUI
{
    public class WindowManager
    {
        private List<IWindow> windows;
        public IReadOnlyList<IWindow> Windows => windows; 
        public InputManager InputManager { get; set; }

        public WindowManager(InputManager inputManager)
        {
            windows = new List<IWindow>();
            InputManager = inputManager;
        }
        public void OnDraw(RenderWindow renderWindow)
        {
            foreach (var window in Windows)
            {
                window.OnDraw(renderWindow);
            }
        }

        public void Update()
        {
            if (InputManager.MouseButtonState[Mouse.Button.Left].buttonState == ButtonState.Release)
            {
                foreach (IWindow window in Windows.Reverse())
                {
                    if (window.OnClick(InputManager.MousePos)) InputManager.SetHandled(Mouse.Button.Left);
                    if (window.IsMouseOver(InputManager.MousePos)) break;
                }
            }
        }

        public void AddWindow(IWindow window)
        {
            windows.Add(window);
            windows = windows.OrderBy(x => x.Layer).ToList();
        }

        public void RemoveWindow(IWindow window)
        {
            windows.Remove(window);
        }

        public void ClearWindows()
        {
            windows.Clear();
        }
    }
}

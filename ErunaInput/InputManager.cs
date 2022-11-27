using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace ErunaInput
{
    public enum ButtonState
    {
        Up,
        Press,
        Down,
        Release,
    }

    public class InputManager
    {
        public Dictionary<Keyboard.Key, (ButtonState buttonState, bool handled)> KeyboardState { get; set; }
        public Dictionary<Mouse.Button, (ButtonState buttonState, bool handled)> MouseButtonState { get; set; }
        public Vector2i MousePos { get; set; }
        public Mouse.Wheel ScrollWheelValue { get; set; }
        public Mouse.Wheel HorizontalWheelValue { get; set; }


        public InputManager()
        {
            MousePos = new Vector2i();

            //init keyboard states
            KeyboardState = new Dictionary<Keyboard.Key, (ButtonState, bool)>();

            foreach (var key in Enum.GetValues(typeof(Keyboard.Key)).Cast<Keyboard.Key>().Distinct())
            {
                KeyboardState.Add(key, (ButtonState.Up, false));
            }
            //Init mouse states
            MouseButtonState = new Dictionary<Mouse.Button, (ButtonState, bool)>();

            foreach (var key in Enum.GetValues(typeof(Mouse.Button)).Cast<Mouse.Button>())
            {
                MouseButtonState.Add(key, (ButtonState.Up, false));
            }
        }

        public void Update()
        {
            UpdateKeyboardState();
            UpdateMouseState();
        }
        public void SetHandled(Enum device)
        {
            switch (device)
            {
                case Mouse.Button _:
                    return;
            }
        }
        public void SetHandled(Mouse.Button button)
        {
            MouseButtonState[button] = (MouseButtonState[button].buttonState, true);
        }

        public void SetHandled(Keyboard.Key key)
        {
            KeyboardState[key] = (KeyboardState[key].buttonState, true);
        }

        private void UpdateKeyboardState()
        {
            foreach (var key in KeyboardState.ToList())
            {
                KeyboardState[key.Key] = (UpdateKeystate(KeyboardState[key.Key].buttonState, Keyboard.IsKeyPressed(key.Key)), false);
            }
        }

        private void UpdateMouseState()
        {
            MousePos = Mouse.GetPosition();

            ScrollWheelValue = Mouse.Wheel.VerticalWheel; 
            HorizontalWheelValue = Mouse.Wheel.HorizontalWheel;

            foreach (var button in MouseButtonState.ToList())
            {
                MouseButtonState[button.Key] = (UpdateKeystate(MouseButtonState[button.Key].buttonState, Mouse.IsButtonPressed(button.Key)), false);
            }
        }

        private ButtonState UpdateKeystate(ButtonState buttonState, bool keyPressed)
        {
            if (keyPressed)
            {
                if (buttonState == ButtonState.Up || buttonState == ButtonState.Release)
                {
                    //key press event!

                    return ButtonState.Press;
                }
                else
                {
                    return ButtonState.Down;
                }
            }
            else
            {
                if (buttonState == ButtonState.Up || buttonState == ButtonState.Release)
                {
                    return ButtonState.Up;
                }
                else
                {
                    //key release event!

                    return ButtonState.Release;
                }
            }
        }
    }
}

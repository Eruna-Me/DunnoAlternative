using ErunaInput;
using SFML.Graphics;
using SFML.Window;

namespace DunnoAlternative.State
{
    public class StateManager
    {
        public readonly InputManager inputManager;

        delegate void Iterator(bool isTop, IState state);

        private readonly Stack<IState> _states = new();

        public StateManager()
        {
            inputManager = new InputManager();
        }

        public void Clear()
        {
            foreach(IState state in _states)
            {
                state.Unload();
            }
            _states.Clear();
        }   

        public void Push(IState state)
        {
            if(_states.Count > 0) _states.Peek().Unload();

            _states.Push(state);
            state.Load();
        }

        public IState Pop()
        {
            var old = _states.Pop();
            old.Unload();
            _states.Peek().Load();
            return old;
        }

        public void Draw(RenderWindow window)
        {
            IterateStack((top, state) =>
            {
                //if (top || (state.Options & StateOption.AllowBackgroundDraws) != 0)
                if (top)
                    state.Draw(window);
            });
        }

        public void Update(RenderWindow window)
        {
            inputManager.Update(window);

            SetSimSpeed();

            IterateStack((top, state) =>
            {
                //if (top || (state.Options & StateOption.AllowBackgroundUpdates) != 0)
                if (top)
                    state.Update(window);
            });
        }

        private void IterateStack(Iterator action)
        {
            if (_states.Count == 0) return; //todo proper waiting states

            var current = _states.Peek();
            //foreach (var state in _states)
            //{
            action(true, current);
            //action(state == current, state);
            //}
        }

        private void SetSimSpeed()
        {
            if (inputManager.KeyboardState[Keyboard.Key.Y].buttonState == ButtonState.Release) Program.SpeedUp = 0.5f;
            if (inputManager.KeyboardState[Keyboard.Key.U].buttonState == ButtonState.Release) Program.SpeedUp = 1.0f;
            if (inputManager.KeyboardState[Keyboard.Key.I].buttonState == ButtonState.Release) Program.SpeedUp = 2.0f;
            if (inputManager.KeyboardState[Keyboard.Key.O].buttonState == ButtonState.Release) Program.SpeedUp = 5.0f;
            if (inputManager.KeyboardState[Keyboard.Key.P].buttonState == ButtonState.Release) Program.SpeedUp = 1000.0f;
        }
    }
}
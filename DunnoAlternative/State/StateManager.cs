using SFML.Graphics;

namespace DunnoAlternative.State
{
    public class StateManager
    {
        delegate void Iterator(bool isTop, IState state);

        private Stack<IState> _states = new();

        public void Clear() => _states.Clear();

        public void Push(IState state) => _states.Push(state);

        public IState Pop() => _states.Pop();

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
    }
}
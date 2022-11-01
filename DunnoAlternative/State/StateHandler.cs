using SFML.Graphics;

namespace DunnoAlternative.State
{
    public class StateHandler
	{
		private IState currentState;

		public StateHandler(IState state)
        {
			currentState = state;
        }

		public void ChangeState (IState state)
		{
			currentState = state;
		}

		public void Update() => currentState.Update();

		public void Draw(RenderWindow window) => currentState.Draw(window);
	}
}
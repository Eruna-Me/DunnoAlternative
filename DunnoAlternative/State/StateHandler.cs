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

		public void Update(RenderWindow window) => currentState.Update(window);

		public void Draw(RenderWindow window) => currentState.Draw(window);
	}
}
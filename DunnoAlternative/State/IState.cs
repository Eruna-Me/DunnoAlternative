﻿using DunnoAlternative.Shared;
using SFML.Graphics;

namespace DunnoAlternative.State
{
	public interface IState : IDrawable
	{
		public void Update(RenderWindow window);
		public void Unload();
		public void Load();
	}
}

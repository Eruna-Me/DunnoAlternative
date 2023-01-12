using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.Shared
{
    public interface IDrawable
    {
        public void Draw (RenderWindow window);
    }
}

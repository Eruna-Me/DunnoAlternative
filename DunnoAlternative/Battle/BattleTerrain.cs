using DunnoAlternative.Shared;
using SFML.Graphics;
using SFML.System;

namespace DunnoAlternative.Battle
{
    internal class BattleTerrain : Transformable, IDrawable
    {
        private readonly VertexArray vertices;
        private RenderStates renderStates;
        private readonly Texture tileset;

        public BattleTerrain(Texture tileset, Vector2u tileSize, Vector2u size) //seed? //terraintype? //tileset meta date?
        {
            vertices = new VertexArray(PrimitiveType.Quads, size.X * size.Y * 4); //quads have 4 vertices obviously

            this.tileset = tileset;

            renderStates = new RenderStates
            {
                Texture = tileset,
                BlendMode = BlendMode.Alpha,
                Shader = null,
            };

            for (uint x = 0; x < size.X; x++)
            {
                for (uint y = 0; y < size.Y; y++)
                {
                    uint tileNumber = (uint)Global.random.Next(4);// todo

                    // find its position in the tileset texture
                    uint tu = tileNumber % (tileset.Size.X / tileSize.X);
                    uint tv = tileNumber / (tileset.Size.Y / tileSize.X);

                    uint quadIndex = (x + y * size.X) * 4;

                    vertices[quadIndex] = new Vertex(new Vector2f(x * tileSize.X, y * tileSize.Y), new Vector2f(tu * tileSize.X, tv * tileSize.Y));
                    vertices[quadIndex + 1] = new Vertex(new Vector2f((x + 1) * tileSize.X, y * tileSize.Y), new Vector2f((tu + 1) * tileSize.X, tv * tileSize.Y));
                    vertices[quadIndex + 2] = new Vertex(new Vector2f((x + 1) * tileSize.X, (y + 1) * tileSize.Y), new Vector2f((tu + 1) * tileSize.X, (tv + 1) * tileSize.Y));
                    vertices[quadIndex + 3] = new Vertex(new Vector2f(x * tileSize.X, (y + 1) * tileSize.Y), new Vector2f(tu * tileSize.X, (tv + 1) * tileSize.Y));
                }
            }
        }

        public void Draw(RenderWindow window)
        {
            renderStates.Transform = Transform;
            renderStates.Texture = tileset;

            vertices.Draw(window, renderStates);
        }
    }
}

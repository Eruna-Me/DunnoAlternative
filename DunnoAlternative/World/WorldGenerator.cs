using DunnoAlternative.Shared;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using SFML.System;

namespace DunnoAlternative.World
{
    public class WorldGenerator
    {
        private readonly Font font = new("Content/Fonts/KosugiMaru-Regular.ttf");

        public List<Player> players;
        public Tile[,] tiles;
        const int INITIAL_HEROES = 3;

        public WorldGenerator(WorldSettings settings)
        {
            //generate a world with different terrain types: plains, forests, hills, mountains, water etc
            //some like water will be impassible
            //all tiles that are passible should be connected to eachother orthogonically.
            //then fill it with players 

            players = new List<Player> {
                new Player(PlayerType.human, "Humanland", Color.Blue),
                new Player(PlayerType.CPU, "Robofactory", Color.Red),
                new Player(PlayerType.CPU, "Bob5", Color.Magenta),
                new Player(PlayerType.passive, "Rebels", Color.Yellow),
            };

            tiles = new Tile[,] {
                { new Tile(font, "First Tile", players[0], 300, new Vector2f(0,0)), new Tile(font, "Second Tile", players[1], 300, new Vector2f(0,Tile.Size.Y)), },
                { new Tile(font, "Other Tile", players[2], 300, new Vector2f(Tile.Size.X,0)), new Tile(font, "Rebel Mountain", players[3], 300, new Vector2f(Tile.Size.X,Tile.Size.Y)), },
            };


            var squadTypes = new List<SquadType>();

            foreach (var file in new DirectoryInfo("Content/SquadTypes").GetFiles())
            {
                squadTypes.Add(JObject.Parse(File.ReadAllText(file.FullName)).ToObject<SquadType>());
            };

            var heroClasses = new List<HeroClass>();

            foreach (var file in new DirectoryInfo("Content/HeroClasses").GetFiles())
            {
                heroClasses.Add(JObject.Parse(File.ReadAllText(file.FullName)).ToObject<HeroClass>());
            };

            foreach (Player player in players)
            {
                for (int i = 0; i < INITIAL_HEROES; i++)
                {
                    player.Heroes.Add(
                        new Hero(
                            heroClasses[Global.random.Next(0, heroClasses.Count)],
                            squadTypes[Global.random.Next(0, squadTypes.Count)].Texture,
                            new List<Squad> {
                            new Squad(squadTypes[Global.random.Next(0,squadTypes.Count)]),
                            new Squad(squadTypes[Global.random.Next(0, squadTypes.Count)])
                            }));
                }
            }
        }
    }
}

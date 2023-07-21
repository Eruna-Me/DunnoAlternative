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

            var factionNames = File.ReadAllLines("Content/Namelists/FactionNamesTest.txt");
            var mountainTileNames = File.ReadAllLines("Content/Namelists/TilesMountain.txt");
            var tileNames = File.ReadAllLines("Content/Namelists/TilesDefault.txt");

            players = new List<Player> {
                new Player(PlayerType.human, factionNames[Global.random.Next(factionNames.Length)], Color.Blue),
                new Player(PlayerType.CPU, factionNames[Global.random.Next(factionNames.Length)], Color.Red),
                new Player(PlayerType.CPU, factionNames[Global.random.Next(factionNames.Length)], Color.Magenta),
                new Player(PlayerType.passive, factionNames[Global.random.Next(factionNames.Length)], Color.Yellow),
            };

            tiles = new Tile[,] {
                { new Tile(font, tileNames[Global.random.Next(tileNames.Length)], players[0], 300, new Vector2f(0,0)), new Tile(font, tileNames[Global.random.Next(tileNames.Length)], players[1], 300, new Vector2f(0,Tile.Size.Y)), },
                { new Tile(font, tileNames[Global.random.Next(tileNames.Length)], players[2], 300, new Vector2f(Tile.Size.X,0)), new Tile(font, mountainTileNames[Global.random.Next(mountainTileNames.Length)], players[3], 300, new Vector2f(Tile.Size.X,Tile.Size.Y)), },
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

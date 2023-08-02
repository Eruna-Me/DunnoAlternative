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

        private const int TILE_SIZE = 300;

        public WorldGenerator(WorldSettings settings)
        {
            //generate a world with different terrain types: plains, forests, hills, mountains, water etc
            //some like water will be impassible
            //all tiles that are passible should be connected to eachother orthogonically.
            //then fill it with players 

            var factionNames = File.ReadAllLines("Content/Namelists/FactionNamesTest.txt");
            var mountainTileNames = File.ReadAllLines("Content/Namelists/TilesMountain.txt");
            var tileNames = File.ReadAllLines("Content/Namelists/TilesDefault.txt");

            players = new List<Player>();
            tiles = new Tile[settings.width, settings.height];

            for ( int x=0; x < settings.width; x++ )
            {
                for(int y=0; y < settings.height; y++ )
                {
                    Player newPlayer;

                    if(x == 0 && y == 0)
                    {
                        newPlayer = new Player(PlayerType.human, factionNames[Global.random.Next(factionNames.Length)], Color.Blue);
                    }
                    else
                    {
                        PlayerType playerType;
                        if(Global.random.NextSingle() > settings.proportionPassive)
                        {
                            playerType = PlayerType.CPU;
                        }
                        else
                        {
                            playerType = PlayerType.passive;
                        }

                        var color = new Color((byte)Global.random.Next(256), (byte)Global.random.Next(256), (byte)Global.random.Next(256));
                        newPlayer = new Player(playerType, factionNames[Global.random.Next(factionNames.Length)], color);
                    }

                    players.Add(newPlayer);
                    tiles[x, y] = new Tile(font, tileNames[Global.random.Next(tileNames.Length)], newPlayer, TILE_SIZE, new Vector2f(Tile.Size.X * x, Tile.Size.Y * y));
                }
            }

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

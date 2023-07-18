using SFML.Graphics;
using SFML.System;
using Newtonsoft.Json;
using DunnoAlternative.JSON;
using DunnoAlternative.Battle.Particles;

namespace DunnoAlternative.World
{
    public struct SquadType
    {
        [JsonConverter(typeof(TextureConverter))]
        public Texture Texture { get; set; }
        public string Name { get; set; }
        public int Soldiers { get; set; }
        public int Max { get; set; }
        public int Cost { get; set; }
        public Vector2f MoveSpeed { get; set; }
        public float Size { get; set; }
        public List<Attack> Attacks { get; set; }
        public Vector2f HP { get; set; }
        public string BloodParticle { get; set; }
    }
}

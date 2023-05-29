using Newtonsoft.Json;
using SFML.Graphics;

namespace DunnoAlternative.JSON
{
    internal class TextureConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            //todo
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new Texture(serializer.Deserialize<string>(reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

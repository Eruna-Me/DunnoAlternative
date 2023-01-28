using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace DunnoAlternative.Shared
{
    public static class Vector2Extensions
    {
        public static Vector2f Normalize(this Vector2f vector)
        {
            float length = vector.Length();

            if (length != 0)
                return new Vector2f(vector.X / length, vector.Y / length);
            else
                return vector;
        }

        public static float Length(this Vector2f vector)
        {
            return MathF.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y));
        }

        public static Vector2f ToVector2f(this Vector2i vector)
        {
            return new Vector2f(vector.X, vector.Y);
        }
        public static float RandomFromRange(this Vector2f range)
        {
            return range.X + (range.Y - range.X) * Global.random.NextSingle();
        }
    }
}

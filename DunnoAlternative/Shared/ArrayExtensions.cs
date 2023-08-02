namespace DunnoAlternative.Shared
{
    public static class ArrayExtensions
    {
        public static T GetRandom<T>(this T[] array)
        {
            return array[Global.random.Next(array.Length)];
        }
    }
}

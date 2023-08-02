namespace DunnoAlternative.Shared
{
    public static class ListExtensions
    {
        public static T GetRandom<T>(this List<T> list)
        {
            return list[Global.random.Next(list.Count)];
        }
    }
}

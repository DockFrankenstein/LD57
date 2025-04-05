namespace LD57
{
    public static partial class Extensions
    {
        public static T BetterGet<T>(this Entity entity)
        {
            foreach (var item in entity.Components)
                if (item is T)
                    return (T)(object)item;

            return default;
        }
    }
}

namespace LD57
{
    public static class Tween
    {
        public static float In(float t, float pow = 2f) =>
            MathF.Pow(t, pow);

        public static float Out(float t, float pow = 2f) =>
            1 - In(1 - t, pow);

        public static float InOut(float t, float pow = 2f) =>
            t < 0.5f ?
            In(t * 2f, pow) / 2f :
            1 - In((1 - t) * 2f, pow) / 2f;
    }
}

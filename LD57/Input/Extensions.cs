namespace LD57.Input
{
    public static class Extensions
    {
        public static void RegisterInInputFocus<T>(this T t) where T : ScriptComponent, IInputFocusable =>
            t.Services.GetService<InputFocusManager>().RegisterFocusable(t);

        public static void UnregisterInInputFocus<T>(this T t) where T : ScriptComponent, IInputFocusable =>
            t.Services.GetService<InputFocusManager>()?.UnregisterFocusable(t);
    }
}

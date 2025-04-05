namespace LD57
{
    public static partial class Extensions
    {
        public static void RegisterInQ(this ScriptComponent component) =>
            component.Services.GetService<qInstance>().RegisteredObjects.Register(component);

        public static void UnregisterInQ(this ScriptComponent component) =>
            component.Services.GetService<qInstance>().RegisteredObjects.Deregister(component);
    }
}

using qASIC.Console;
using qASIC.Options.Serialization;

namespace LD57.qUtils
{
    public class qOptionsInit : StartupScript
    {
        public OptionsManager Manager { get; private set; }

        public override void Start()
        {
            Manager = new OptionsManager(new qARKOptionsSerializer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.qark")));
            var instance = Services.GetService<qInstance>();
            var console = Services.GetService<GameConsole>();

            if (console != null)
                console.CommandList.AddBuiltInOptionsCommands(Manager);

            Services.AddService(Manager);
            instance.Services.Add(Manager);
            instance.RegisteredObjects.Register(Manager);

            Manager.Initialize();
        }
    }
}

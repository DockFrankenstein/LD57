using qASIC.Console;
using qASIC.Console.Commands;

namespace LD57.qUtils
{
    public class qConsoleInit : StartupScript
    {
        public InstanceConsoleManager ConsoleManager { get; private set; }
        public GameConsole MainConsole { get; private set; }

        public override void Start()
        {
            var instance = Services.GetService<qInstance>();
            ConsoleManager = instance.UseConsole();

            Services.AddService(ConsoleManager);
            instance.Services.Add(ConsoleManager);

            MainConsole = new GameConsole("MAIN", new GameCommandList()
                .AddBuiltInCommands()
                .FindAttributeCommands()
                .FindCommands())
            {
                Instance = instance,
            };

            ConsoleManager.RegisterConsole(MainConsole);

            Services.AddService(MainConsole);
            instance.Services.Add(MainConsole);
        }
    }
}

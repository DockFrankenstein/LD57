using qASIC.Console;
using qASIC.Options.Serialization;

namespace LD57.qUtils
{
    public class qOptionsInit : SyncScript
    {
        public Int2 minimPos;
        public Int2 minimSize;

        public OptionsManager Manager { get; private set; }

        [Option("fullscreen", true)]
        public void Sett_Fullscreen(bool value)
        {
            if (isFullscreen == value)
                return;

            isFullscreen = value;

            var screenBounds = Game.GraphicsDevice.Adapter.Outputs[0].DesktopBounds;
            Game.Window.IsBorderLess = value;

            switch (value)
            {
                case true:
                    minimPos = Game.Window.Position;
                    minimSize = new Int2(Game.Window.ClientBounds.Size.Width, Game.Window.ClientBounds.Size.Height);

                    Game.Window.Position = new Int2(0, 0);
                    Game.Window.SetSize(new Int2(screenBounds.Width, screenBounds.Height));
                    break;
                case false:
                    Game.Window.Position = minimPos;
                    Game.Window.SetSize(minimSize);
                    break;
            }

        }

        bool isFullscreen = false;

        public override void Start()
        {
            Game.Window.AllowUserResizing = true;

            var instance = Services.GetService<qInstance>();
            var console = Services.GetService<GameConsole>();

            Manager = new OptionsManager(new OptionsList(), new qARKOptionsSerializer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.qark")));

            if (console != null)
                console.CommandList.AddBuiltInOptionsCommands(Manager);

            Services.AddService(Manager);
            instance.Services.Add(Manager);
            instance.RegisteredObjects.Register(Manager);
            instance.RegisteredObjects.Register(this);

            var targetList = new OptionTargetList(instance, Manager)
                .FindOptions()
                .PopulateManagerFromTargets();

            targetList.RegisteredObjects.SyncWithOther(instance.RegisteredObjects);

            Manager.Initialize();
        }

        public override void Update()
        {
            if (Input.IsKeyPressed(Keys.F11))
                Manager.SetOptionAndApply("fullscreen", !isFullscreen);
        }
    }
}

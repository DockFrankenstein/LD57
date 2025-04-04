using LD57.LevelManagement;
using qASIC.Console;
using qASIC.Console.Commands;

namespace LD57.Commands
{
    [ConsoleCommand]
    public class Cmd_LoadLevel : GameCommand
    {
        public override string CommandName => "loadlevel";
        public override string Description => "Loads a level.";

        public override object Run(GameCommandContext context)
        {
            context.CheckArgumentCount(1);
            var levelManager = context.GetStrideService<LevelManager>();

            levelManager.LoadLevel(context[0].arg);
            return null;
        }
    }
}

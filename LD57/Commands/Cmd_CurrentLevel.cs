using LD57.LevelManagement;
using qASIC.Console;
using qASIC.Console.Commands;

namespace LD57.Commands
{
    [ConsoleCommand]
    public class Cmd_CurrentLevel : GameCommand
    {
        public override string CommandName => "currentlevel";
        public override string Description => "Prints the tag of the currently loaded level.";

        public override object Run(GameCommandContext context)
        {
            context.CheckArgumentCount(0);
            return context.GetStrideService<LevelManager>().CurrentLevelTag;
        }
    }
}

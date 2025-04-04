using qASIC.Console;
using qASIC.Console.Commands;

namespace LD57.Commands
{
    [ConsoleCommand]
    public class Cmd_Ldjam : GameCommand
    {
        public override string CommandName => "ldjam";
        public override string Description => "Ldjam.";

        public override object Run(GameCommandContext context)
        {
            context.CheckArgumentCount(0);
            context.Logs.Log("Hello Ludum Dare 57!", qColor.Green);
            return null;
        }
    }
}

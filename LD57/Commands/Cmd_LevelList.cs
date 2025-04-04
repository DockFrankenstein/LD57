using LD57.LevelManagement;
using qASIC.Console;
using qASIC.Console.Commands;
using qASIC.Text;

namespace LD57.Commands
{
    [ConsoleCommand]
    public class Cmd_LevelList : GameCommand
    {
        public override string CommandName => "levellist";
        public override object Run(GameCommandContext context)
        {
            var tree = TextTree.Fancy;
            var root = new TextTreeItem("Levels:");

            var levels = context.GetStrideService<LevelManager>();

            foreach (var item in levels.levels)
                root.Add($"{item.tag} - sceneCount:{item.scenes.Count}{(item.tag == levels.CurrentLevelTag ? " (default)" : "")}");

            context.Logs.Log(tree.GenerateTree(root));
            return null;
        }
    }
}

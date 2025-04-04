using System.Text;

namespace qASIC.Options.Commands
{
    public class OptionsList : OptionsCommand
    {
        public OptionsList(OptionsManager manager) : base(manager) { }

        public override string CommandName => "optionslist";
        public override string[] Aliases => new string[] { "settingslist", "listoptions", "listsettings" };
        public override string Description => "Shows a list of options.";

        public override object Run(CommandContext context)
        {
            context.CheckArgumentCount(0);

            StringBuilder txt = new StringBuilder("List of options:");

            foreach (var item in Manager.OptionsList)
                txt.Append($"\n- {item.Key}:{item.Value.value} (default: {item.Value.defaultValue})");

            context.Logs.Log(txt.ToString());
            return null;
        }
    }
}
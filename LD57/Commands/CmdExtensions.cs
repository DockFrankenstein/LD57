using qASIC.Console;

namespace LD57.Commands
{
    public static  class CmdExtensions
    {
        public static T GetStrideService<T>(this GameCommandContext context) where T : class =>
            context.console.Instance.Services.Get<Game>().Services.GetService<T>();
    }
}

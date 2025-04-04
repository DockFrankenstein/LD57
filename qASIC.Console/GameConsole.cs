using GameLog = qASIC.qLog;
using System.Diagnostics;
using qASIC.Console.Commands;
using System.Reflection;
using qASIC.Console.Parsing;
using System;
using qASIC.CommandPrompts;
using System.Threading.Tasks;

namespace qASIC.Console
{
    public class GameConsole : IService
    {
        public const string SYSTEM_NAME = "qASIC.Console";
        public const string SYSTEM_VERSION = "1.0.0";

        public GameConsole(ICommandList commandList = null, ArgumentsParser parser = null) :
            this(Guid.NewGuid().ToString(), commandList, parser)
        { }

        public GameConsole(string name, ICommandList commandList = null, ArgumentsParser parser = null)
        {
            Logs = new GameLogManager();

            Name = name;
            CommandList = commandList ?? new GameCommandList()
                .AddBuiltInCommands()
                .FindCommands()
                .FindAttributeCommands();

            CommandParser = parser ?? new QuashParser();

            qDebug.OnLog += QDebug_OnLog;
        }

        private void QDebug_OnLog(GameLog log)
        {
            if (LogQDebug)
                Log(log, 4, true);
        }

        /// <summary>Main static instance of <see cref="GameConsole"/> that was set using <see cref="SetAsMain"/>.</summary>
        public static GameConsole Main { get; private set; }
        /// <summary>Sets this instance as main to make it accessible from property <see cref="Main"/>.</summary>
        /// <returns>Returns itself.</returns>
        public GameConsole SetAsMain()
        {
            Main = this;
            return this;
        }

        private qInstance _instance;
        public qInstance Instance
        {
            get => _instance;
            set
            {
                var oldGetLogsFromInstance = GetLogsFromInstance;
                GetLogsFromInstance = false;

                Targets.StopSyncingWithOther(_instance?.RegisteredObjects);
                _instance = value;
                Targets.SyncWithOther(_instance?.RegisteredObjects);

                GetLogsFromInstance = oldGetLogsFromInstance;
            }
        }

        public string Name { get; private set; }

        public GameLogManager Logs { get; internal set; }

        public ICommandList CommandList { get; set; }

        public ArgumentsParser CommandParser { get; set; }

        public ICommand CurrentCommand { get; private set; } = null;
        public object ReturnedValue { get; private set; } = null;
        public LogManager CurrentCommandLogs { get; private set; } = null;

        public GameConsoleTheme Theme { get; set; } = GameConsoleTheme.Default;

        /// <summary>Should the console log messages from <see cref="qDebug"/>.</summary>
        public bool LogQDebug { get; set; } = true;

        /// <summary>Determines if console should try looking for attributes that can change log messages and colors.</summary>
        public bool UseLogModifierAttributes { get; set; } = true;

        /// <summary>Determines if it should include exceptions when logging normal errors with executing commands.</summary>
        public bool IncludeStackTraceInCommandExceptions { get; set; } = false;

        /// <summary>Determines if it should include exceptions when logging unknown errors with executing commands.</summary>
        public bool IncludeStackTraceInUnknownCommandExceptions { get; set; } = true;

        /// <summary>Initializes reflections. This will happen automatically when reflections are needed, but it can cause lag, so it's better to do it once when the application launches.</summary>
        public void InitializeReflections() =>
            ConsoleReflections.Initialize();

        #region Registering targets
        public qRegisteredObjects Targets { get; private set; } = new qRegisteredObjects();
        #endregion

        #region Executing
        /// <summary>Can the console execute commands using <see cref="Execute(string)"/>.</summary>
        public bool CanParseAndExecute =>
            CommandList != null && CommandParser != null;

        /// <summary>Can the console execute commands using <see cref="Execute(CommandArgument[])"/>.</summary>
        public bool CanExecute =>
            CommandList != null;

        /// <summary>Executes a command.</summary>
        /// <param name="cmd">Command text that will be parsed and executed.</param>
        public object Execute(string cmd) =>
            Execute(CreateContext(cmd));

        /// <summary>Executes a command asynchronously.</summary>
        /// <param name="cmd">Command text that will be parsed and executed.</param>
        public async Task<object> ExecuteAsync(string cmd) =>
            await ExecuteAsync(CreateContext(cmd));

        private bool PreprocessContext(GameCommandContext context)
        {
            //Prompt
            if (CurrentCommand != null)
            {
                if (!(ReturnedValue is CommandPrompt prompt))
                    throw new Exception("A command is already being executed!");

                context.prompt = prompt;
                context.commandName = CurrentCommand.CommandName;
                context.Logs = CurrentCommandLogs;

                if (!prompt.CanExecute(context))
                    return false;

                context.args = prompt.Prepare(context);
                return true;
            }

            //Normal
            if (CommandList == null)
                throw new Exception("Cannot execute commands with no command list!");

            if (context.Logs == null)
                context.Logs = new GameLogManager();

            bool registerLogs = context.LogOutput;
            if (registerLogs)
                Logs.RegisterManager(context.Logs);

            if (!CommandList.TryGetCommand(context.commandName, out var command))
            {
                context.Logs.LogError($"Command {context.commandName} doesn't exist");
                Logs.UnregisterManager(context.Logs);
                return false;
            }

            CurrentCommand = command;
            CurrentCommandLogs = context.Logs;

            return true;
        }

        private object PostprocessContext(GameCommandContext context)
        {
            var closeLogs = true;
            if (context.RunTaskResult && ReturnedValue is Task task)
            {
                ReturnedValue = null;
                closeLogs = false;
                Task.Run(async () =>
                {
                    await ExecuteAsync(CurrentCommand.CommandName, task, context.Logs, false);
                    if (context.CleanupLogger)
                    {
                        Logs.UnregisterManager(context.Logs);
                        context.Logs?.Close();
                    }
                });
            }

            if (ReturnedValue is CommandPrompt)
                return ReturnedValue;

            if (closeLogs && context.CleanupLogger)
            {
                Logs.UnregisterManager(CurrentCommandLogs);
                CurrentCommandLogs.Close();
            }

            CurrentCommandLogs = null;
            CurrentCommand = null;
            return ReturnedValue;
        }

        /// <summary>Executes a command.</summary>
        /// <param name="context">Command arguments.</param>
        public object Execute(GameCommandContext context)
        {
            //Before
            if (!PreprocessContext(context))
                return null;

            //Executing
            ReturnedValue = Execute(CurrentCommand.CommandName, () => CurrentCommand.Run(context), context.Logs);

            //After
            return PostprocessContext(context);
        }

        /// <summary>Executes a command asynchronously.</summary>
        /// <param name="context">Command arguments.</param>
        public async Task<object> ExecuteAsync(GameCommandContext context)
        {
            //Before
            if (!PreprocessContext(context))
                return null;

            //Executing
            ReturnedValue = Execute(CurrentCommand.CommandName, () => CurrentCommand.Run(context), context.Logs);
            if (ReturnedValue is Task task)
                ReturnedValue = await ExecuteAsync(CurrentCommand.CommandName, task, context.Logs);

            //After
            return PostprocessContext(context);
        }

        /// <summary>Executes a command.</summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="command">Command code to execute.</param>
        /// <param name="logOutput">When true, it will log the output value to the console.</param>
        public object Execute(string commandName, Func<object> command, LogManager logs = null, bool logOutput = true)
        {
            try
            {
                var output = command.Invoke();
                if (logOutput && output != null && !(output is CommandPrompt) && !(output is Task))
                    logs?.Log(output.ToString());

                return output;
            }
            catch (CommandException e)
            {
                logs?.LogError(e.ToString(IncludeStackTraceInCommandExceptions));
            }
            catch (Exception e)
            {
                logs?.LogError(IncludeStackTraceInUnknownCommandExceptions ?
                    $"There was an error while executing command '{commandName}': {e}" :
                    $"There was an error while executing command '{commandName}'.");
            }

            return null;
        }

        public async Task<object> ExecuteAsync(string commandName, Func<Task> command, LogManager logs = null, bool logOutput = true) =>
            await ExecuteAsync(commandName, command.Invoke(), logs, logOutput);

        /// <summary>Executes a command asynchronously.</summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="command">Command task to execute.</param>
        /// <param name="logOutput">When true, it will log the output value to the console.</param>
        public async Task<object> ExecuteAsync(string commandName, Task task, LogManager logs = null, bool logOutput = true)
        {
            try
            {
                if (!(task is Task<object> objTask))
                {
                    await task;
                    return null;
                }

                var output = await objTask;

                if (logOutput && output != null && !(output is CommandPrompt))
                    logs?.Log(output.ToString());

                return output;
            }
            catch (CommandException e)
            {
                logs?.LogError(e.ToString(IncludeStackTraceInCommandExceptions));
            }
            catch (Exception e)
            {
                logs?.LogError(IncludeStackTraceInUnknownCommandExceptions ?
                    $"There was an error while executing command '{commandName}': {e}" :
                    $"There was an error while executing command '{commandName}'.");
            }

            return null;
        }

        public virtual GameCommandContext CreateContext(string cmd)
        {
            var context = new GameCommandContext()
            {
                inputString = cmd,
                commandName = GetCommandName(cmd),
                args = CreateConsoleArguments(cmd),
                console = this,
            };

            return context;
        }

        protected CommandArgument[] CreateConsoleArguments(string cmd)
        {
            var args = new CommandArgument[0];

            if (!(ReturnedValue is CommandPrompt prompt) || prompt.ParseArguments)
            {
                if (CommandParser == null)
                    throw new Exception("Cannot parse commands with no parser!");

                args = CommandParser.ParseArguments(cmd);
            }

            return args;
        }

        protected string GetCommandName(string cmd)
        {
            if (CurrentCommand?.CommandName != null)
                return CurrentCommand.CommandName;

            if (CommandParser == null)
                throw new Exception("Cannot parse commands with no parser!");

            return CommandParser.ParseCommandName(cmd);
        }
        #endregion

        #region Registering Loggables
        private bool _getLogsFromInstance = true;
        /// <summary>Whenever to log messages from <see cref="Instance"/>.</summary>
        public bool GetLogsFromInstance
        {
            get => _getLogsFromInstance;
            set
            {
                if (_getLogsFromInstance == value) return;
                _getLogsFromInstance = value;

                if (_instance == null) return;

                switch (_getLogsFromInstance)
                {
                    case true:
                        Logs.RegisterLoggable(_instance);
                        break;
                    case false:
                        Logs.UnregisterLoggable(_instance);
                        break;
                }
            }
        }
        #endregion

        #region Logging
        /// <summary>Logs a message to the console.</summary>
        /// <param name="message">Message to log.</param>
        /// <param name="stackTraceIndex">Index used for gathering log customization attributes.</param>
        public void Log(string message, int stackTraceIndex = 2) =>
            Log(GameLog.CreateNow(message, qDebug.DEFAULT_COLOR_TAG), stackTraceIndex, true);

        /// <summary>Logs a warning message to the console.</summary>
        /// <param name="message">Message to log.</param>
        /// <param name="stackTraceIndex">Index used for gathering log customization attributes.</param>
        public void LogWarning(string message, int stackTraceIndex = 2) =>
            Log(GameLog.CreateNow(message, qDebug.WARNING_COLOR_TAG), stackTraceIndex);

        /// <summary>Logs an error message to the console.</summary>
        /// <param name="message">Message to log.</param>
        /// <param name="stackTraceIndex">Index used for gathering log customization attributes.</param>
        public void LogError(string message, int stackTraceIndex = 2) =>
            Log(GameLog.CreateNow(message, qDebug.ERROR_COLOR_TAG), stackTraceIndex);

        /// <summary>Logs a message to the console with a color.</summary>
        /// <param name="message">Message to log.</param>
        /// <param name="color">Message color.</param>
        /// <param name="stackTraceIndex">Index used for gathering log customization attributes.</param>
        public void Log(string message, qColor color, int stackTraceIndex = 2) =>
            Log(GameLog.CreateNow(message, color), stackTraceIndex);

        /// <summary>Logs a message to the console with a color.</summary>
        /// <param name="message">Message to log.</param>
        /// <param name="colorTag">Message color.</param>
        /// <param name="stackTraceIndex">Index used for gathering log customization attributes.</param>
        public void Log(string message, string colorTag, int stackTraceIndex = 2) =>
            Log(GameLog.CreateNow(message, colorTag), stackTraceIndex);

        /// <summary>Logs a log to the console.</summary>
        /// <param name="stackTraceIndex">Index used for gathering log customization attributes.</param>
        /// <param name="useLogModifiers">If true, the console will check for color attributes.</param>
        public void Log(GameLog log, int stackTraceIndex = 2, bool useLogModifiers = false)
        {
            if (UseLogModifierAttributes && useLogModifiers)
            {
                var stackTrace = new StackTrace();
                var stackFrame = stackTrace.GetFrame(stackTraceIndex);

                var method = stackFrame?.GetMethod();
                var declaringType = method?.DeclaringType;

                if (TryGetColorAttributeOfTrace(method, declaringType, out var colorAttr))
                {
                    log.colorTag = colorAttr.ColorTag;
                    log.color = colorAttr.Color;
                }

                if (TryGetPrefixAttributeOfTrace(method, declaringType, out var prefixAttr))
                {
                    log.message = prefixAttr.FormatMessage(log.message);
                }
            }

            Logs.Log(log);
        }

        /// <summary>Clears the console. Previous logs will still be there, but they won't show up in the output.</summary>
        public void Clear() =>
            Log(GameLog.CreateNow(string.Empty, LogType.Clear, qDebug.DEFAULT_COLOR_TAG));

        public qColor GetLogColor(GameLog log) =>
            Theme.GetLogColor(log);

        static bool TryGetPrefixAttributeOfTrace(MethodBase method, Type declaringType, out LogPrefixAttribute attribute)
        {
            attribute = null;

            if (method != null &&
                ConsoleReflections.PrefixAttributeMethods.TryGetValue(ConsoleReflections.CreateMethodId(method), out var methodAttr))
            {
                attribute = methodAttr!;
                return true;
            }

            if (declaringType != null &&
                ConsoleReflections.PrefixAttributeDeclaringTypes.TryGetValue(ConsoleReflections.CreateTypeId(declaringType), out var declaringTypeAttr))
            {
                attribute = declaringTypeAttr!;
                return true;
            }

            return false;
        }

        static bool TryGetColorAttributeOfTrace(MethodBase method, Type declaringType, out LogColorAttribute attribute)
        {
            attribute = null;

            if (method != null &&
                ConsoleReflections.ColorAttributeMethods.TryGetValue(ConsoleReflections.CreateMethodId(method), out var methodAttr))
            {
                attribute = methodAttr!;
                return true;
            }

            if (declaringType != null &&
                ConsoleReflections.ColorAttributeDeclaringTypes.TryGetValue(ConsoleReflections.CreateTypeId(declaringType), out var declaringTypeAttr))
            {
                attribute = declaringTypeAttr!;
                return true;
            }

            return false;
        }
        #endregion
    }
}
//
// Commandier
// by KryKom 2024
//

using Commandier.argument;
using Kolors;
using VoxelsCoreSharp.console.command.argument;

namespace Commandier;

public static class CommandRegistry {

    /// <summary>
    /// list of all available commands in groups
    /// </summary>
    private static List<CommandGroup> COMMAND_REGISTRY = new();

    /// <summary>
    /// whether the default commandier commands are registred
    /// </summary>
    private static bool defaultsRegistred = false;

    /// <summary>
    /// returns the entire command register
    /// </summary>
    /// <returns></returns>
    public static List<CommandGroup> getRegistry() {
        return COMMAND_REGISTRY;
    }

    /// <summary>
    /// adds a new command to the command register
    /// </summary>
    /// <param name="c"></param>
    public static void registerCommand(Command c) {
        
        foreach (CommandGroup cg in COMMAND_REGISTRY) {
            if (c.name == cg.name) {
                cg.addCommand(c);
                return;
            }
        }

        CommandGroup group = new CommandGroup(c.name, [c]);
        
        COMMAND_REGISTRY.Add(group);
    }

    /// <summary>
    /// registers default commands
    /// </summary>
    internal static void registerDefault() {
        
        if(defaultsRegistred) return;
        
        registerCommand(EXIT);
        registerCommand(HELP);
        registerCommand(HELP_COMMAND);
        registerCommand(HELP_LIST);
        registerCommand(COLORS);
        registerCommand(VARIABLE_REMOVE);
        registerCommand(VARIABLE_ADD);
        registerCommand(VARIABLE_SET);
        registerCommand(VARIABLE_LIST);
        registerCommand(DEBUG_LEVEL);
        registerCommand(DEBUG_INFO);
        registerCommand(DEBUG_WARN);
        registerCommand(DEBUG_ERROR);
    }
    
    
    // --- COMMAND CODE ---
    
    public static readonly Command HELP = new Command("help", [], (object[] args) => {
        
        Console.WriteLine("Welcome to Commandier.\n" +
                          "syntax of every command is <command_name> <args...>\n" +
                          "for more help for specific command type 'help <command_name>'\n" +
                          "to exit type 'exit'");
        
    }, "helps you understand the commands and the shell");

    public static readonly Command HELP_LIST = new Command("help", [new FixedArgument("list")], (object[] args) => {
        foreach (CommandGroup cg in COMMAND_REGISTRY) {
            foreach (Command c in cg.commands) {
                ConsoleColors.printColored($"{c.name}", ColorPalette.BASE.colors[2]);
                ConsoleColors.printlnColored($"{c.argumentsToString()}", ColorPalette.GRAY_9.colors[3]);
            }
        }
    });

    public static readonly Command HELP_COMMAND = new Command("help", [new FixedArgument("command"), new StringArgument("command_name")], (object[] args) => {
        foreach (CommandGroup cg in COMMAND_REGISTRY) {
            if (cg.name != (string)args[1]) {
                continue;
            }

            Console.WriteLine($"Usage for \'{args[1]}\':");
            
            foreach (Command c in cg.commands) {
                ConsoleColors.printColored($"   {c.name}", ColorPalette.BASE.colors[2]);
                ConsoleColors.printColored($"{c.argumentsToString()}", ColorPalette.GRAY_9.colors[3]);
                ConsoleColors.printlnColored($"  - {c.description}", ColorPalette.GRAY_9.colors[5]);
            }
            
            return;
        }
        
        Debug.error("No available command with this name.", true);

    }, "gives help to supplied command");

    public static readonly Command COLORS = new("colors", [], args => {
        ColorPalette.COLORS.printPalette();
        ColorPalette.GRAY_9.printPalette();
    }, "prints 2 used colors palettes");
    
    public static readonly Command EXIT = new Command("exit", [], args => {
        Shell.SHELL.stop();
    }, "exits the shell");

    public static readonly Command VARIABLE_LIST = new Command("var", [new FixedArgument("list")], args => {
        foreach (Variable v in Variable.variables) {
            ConsoleColors.printColored($"{v.name}: ", ColorPalette.BASE.colors[2]);
            ConsoleColors.printlnColored($"{v.value}", ColorPalette.GRAY_9.colors[3]);
        }
    }, "lists all available variables");

    public static readonly Command VARIABLE_ADD = new Command("var", [new FixedArgument("add"), new StringArgument("name"), new StringArgument("value")], args => {
        Variable v = new Variable(args[1].ToString(), args[2].ToString()); 
        Variable.addVariable(v);
    }, "creates a new variable");
    
    public static readonly Command VARIABLE_SET = new Command("var", [new FixedArgument("set"), new StringArgument("name"), new StringArgument("value")], args => {
        Variable.setVariable(args[1].ToString(), args[2].ToString());
    }, "sets an existing variable");

    public static readonly Command VARIABLE_REMOVE = new Command("var", [new FixedArgument("delete"), new StringArgument("name")], args => {
        Variable.removeVariable(args[1].ToString());
    }, "removes a variable");

    public static readonly Command DEBUG_LEVEL = new("debug", [new FixedArgument("level"), new IntArgument(0, 3, "level")], args => {
        Debug.debugLevel = (int)args[1];
    });

    public static readonly Command DEBUG_INFO = new("debug", [new FixedArgument("info"), new StringArgument("message")], args => {
        Debug.info(args[1].ToString(), true);
    }, "prints a info message into the console");
    
    public static readonly Command DEBUG_WARN = new("debug", [new FixedArgument("warn"), new StringArgument("message")], args => {
        Debug.warn(args[1].ToString(), true);
    }, "prints a warning message into the console");
    
    public static readonly Command DEBUG_ERROR = new("debug", [new FixedArgument("error"), new StringArgument("message")], args => {
        Debug.error(args[1].ToString(), true);
    }, "prints a error message into the console");
}
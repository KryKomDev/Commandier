//
// Commandier
// by KryKom 2024
//

using Commandier.argument;
using VoxelsCoreSharp.console.command.argument;

namespace Commandier;

public static class CommandRegistry {

    /// <summary>
    /// list of all available commands
    /// </summary>
    private static List<Command> COMMAND_REGISTRY_1 = new();

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
                ConsoleColors.printColored($"{c.name}", (int)Colors.PURPLE_4);
                ConsoleColors.printlnColored($"{c.argumentsToString()}", (int)Colors.GRAY_4);
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
                ConsoleColors.printColored($"   {c.name}", (int)Colors.PURPLE_4);
                ConsoleColors.printColored($"{c.argumentsToString()}", (int)Colors.GRAY_4);
                ConsoleColors.printlnColored($"  - {c.description}", (int)Colors.GRAY_2);
            }
            
            return;
        }
        
        Debug.error("No available command with this name.", true);

    }, "gives help to supplied command");
    
    public static readonly Command EXIT = new Command("exit", [], args => {
        Shell.SHELL.stop();
    }, "exits the shell");
}
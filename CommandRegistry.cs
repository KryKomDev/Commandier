//
// Commandier
// by KryKom 2024
//

using Commandier.argument;
using Kolors;

namespace Commandier;

public static class CommandRegistry {

    /// <summary>
    /// list of all available commands in groups
    /// </summary>
    private static List<CommandGroup> COMMAND_REGISTRY = new();

    /// <summary>
    /// whether the default commandier commands are registered
    /// </summary>
    private static bool defaultsRegistered = false;

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
        
        if(defaultsRegistered) return;
        
        registerCommand(EXIT);
        registerCommand(HELP);
        registerCommand(HELP_COMMAND);
        registerCommand(HELP_LIST);
        registerCommand(COLORS);
        registerCommand(COLORS_SET);
        registerCommand(COLORS_SET_EXPLICIT);
        registerCommand(COLORS_LIST_ALL);
        registerCommand(VARIABLE_REMOVE);
        registerCommand(VARIABLE_ADD);
        registerCommand(VARIABLE_SET);
        registerCommand(VARIABLE_LIST);
        registerCommand(DEBUG_LEVEL);
        registerCommand(DEBUG_INFO);
        registerCommand(DEBUG_WARN);
        registerCommand(DEBUG_ERROR);
        registerCommand(NEOFETCH);
    }
    
    
    // --- COMMAND CODE ---
   
    // help commands

    public static readonly Command HELP = new Command("help", [], (object[] args) => {
        
        ConsoleColors.printlnColored("Welcome to Commandier.\n" +
                          "  Syntax of every command is <command_name> <args...>\n" +
                          "  For more help for specific command type 'help command <command_name>'\n" +
                          "  To exit type 'exit'", Shell.PALETTE.colors[0]);
        
    }, "helps you understand the commands and the shell");

    public static readonly Command HELP_LIST = new Command("help", [new FixedArgument("list")], args => {
        ConsoleColors.printlnColored("Available commands:", Shell.PALETTE.colors[0]);
        foreach (CommandGroup cg in COMMAND_REGISTRY) {
            
            ConsoleColors.printlnColored($"  {cg.name}", Shell.PALETTE.colors[4]);
            
            // foreach (Command c in cg.commands) {
            //     ConsoleColors.printlnColored($"{c.argumentsToString()}", ColorPalette.GRAY_9.colors[3]);
            // }
        }
    });

    public static readonly Command HELP_COMMAND = new Command("help", [new FixedArgument("command"), new StringArgument("command_name")], (object[] args) => {
        foreach (CommandGroup cg in COMMAND_REGISTRY) {
            if (cg.name != (string)args[1]) {
                continue;
            }

            ConsoleColors.printlnColored($"Usages for \'{args[1]}\':", Shell.PALETTE.colors[0]);
            
            foreach (Command c in cg.commands) {
                ConsoleColors.printColored($"   {c.name}", Shell.PALETTE.colors[4]);
                ConsoleColors.printColored($"{c.argumentsToString()}", Shell.PALETTE.colors[0]);
                ConsoleColors.printlnColored($"  - {c.description}", ColorPalette.GRAY_9.colors[5]);
            }
            
            return;
        }
        
        Debug.error("No available command with this name.", true);

    }, "gives help to supplied command");

    // colors commands

    public static readonly Command COLORS = new("colors", [], args => {
        Shell.PALETTE.printPalette();
        ColorPalette.GRAY_9.printPalette();
    }, "prints used colors palettes");

    public static readonly Command COLORS_SET = new("colors", [new FixedArgument("set"), new StringArgument("palette_name")], args => {
        Shell.PALETTE = ColorPalette.getPalette((string)args[1]);
        Debug.infoColor = Shell.PALETTE.colors[3];
        Debug.warnColor = Shell.PALETTE.colors[2];
        Debug.errorColor = Shell.PALETTE.colors[1];
    }, "sets the main color palette");

    public static readonly Command COLORS_SET_EXPLICIT = new("colors", [new FixedArgument("set"), new FixedArgument("at"), new IntArgument(0, 4, "palette_index"), new StringArgument("hex_code")], args => {

        try {
            Shell.PALETTE.colors[(int)args[2]] = int.Parse((string)args[3], System.Globalization.NumberStyles.HexNumber);
            Debug.infoColor = Shell.PALETTE.colors[3];
            Debug.warnColor = Shell.PALETTE.colors[2];
            Debug.errorColor = Shell.PALETTE.colors[1];
        }
        catch (FormatException e) {
            Debug.error("Hex code is in invalid format!");
        }
        
    }, "sets a color in the main color palette");

    public static readonly Command COLORS_LIST_ALL = new("colors", [new FixedArgument("list")], args => {
        ColorPalette.printAllPalettes();
    }, "prints all palettes");
    
    // neofetch

    public static readonly Command NEOFETCH = new("neofetch", [], args => {
        ConsoleColors.printComplexColored("~        *#########*,          \x1B[1m\x1B[4m!C O M M A N D I E R\x1B[0m~ by KryKom\n" + 
                                              "~      #@@/-------\\@@#         !version:~ 24w25c\n" + 
                                             $"~    /%@(  (##*\\    (@%\\       !commands total:~ {COMMAND_REGISTRY.Count}\n" + 
                                             $"~   #@%/   @&&&@@(   \\%@#      !color palettes total:~ {ColorPalette.palettes.Count}\n" + 
                                              "~  #@#*    @%|  ,(@,  *#@#     !------~\n" + 
                                              "~   #@%\\   @&%%&&#   /%@#      !main palette:       ", 
            [("~", Shell.PALETTE.colors[0]), ("!", Shell.PALETTE.colors[4])]);
        Shell.PALETTE.printPalette();
        ConsoleColors.printComplexColored("~    \\&&(  (##/^    (&&/       !grayscale palette:  ", [("~", Shell.PALETTE.colors[0]), ("!", Shell.PALETTE.colors[4])]);
        ColorPalette.GRAY_9.printPalette();
        ConsoleColors.printlnColored("     *#@@\\-------/@@#*        \n" + 
                                     "        \\#########/           \n", Shell.PALETTE.colors[0]);
    });
    
    // exit command

    public static readonly Command EXIT = new Command("exit", [], args => {
        Shell.SHELL.stop();
    }, "exits the shell");

    // variable commands

    public static readonly Command VARIABLE_LIST = new Command("var", [new FixedArgument("list")], args => {
        foreach (Variable v in Variable.variables) {
            ConsoleColors.printColored($"{v.name}: ", Shell.PALETTE.colors[0]);
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

    // debug commands

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

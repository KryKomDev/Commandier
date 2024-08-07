//
// Commandier
// by KryKom 2024
//

using System.Globalization;
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
        registerCommand(CLOCK);
        registerCommand(CLEAR);
        registerCommand(CLOCK_A);
        registerCommand(CLOCK_D);
        registerCommand(CLOCK_E);
    }
    
    // --- COMMAND CODE ---
   
    // help commands

    public static readonly Command HELP = new("help", [], args => {
        
        ConsoleColors.printlnColored("Welcome to Commandier.\n" +
                          "  Syntax of every command is <command_name> <args...>\n" +
                          "  For more help for specific command type 'help command <command_name>'\n" +
                          "  To exit type 'exit'", Shell.PALETTE.colors[0]);
        
    }, "helps you understand the commands and the shell");

    public static readonly Command HELP_LIST = new("help", [new FixedArgument("list")], args => {
        ConsoleColors.printlnColored("Available commands:", Shell.PALETTE.colors[0]);
        foreach (CommandGroup cg in COMMAND_REGISTRY) {
            
            ConsoleColors.printlnColored($"  {cg.name}", Shell.PALETTE.colors[4]);
            
            // foreach (Command c in cg.commands) {
            //     ConsoleColors.printlnColored($"{c.argumentsToString()}", ColorPalette.GRAY_9.colors[3]);
            // }
        }
    }, "lists all available commands");

    public static readonly Command HELP_COMMAND = new("help", [new FixedArgument("command"), new StringArgument("command_name")], args => {
        foreach (CommandGroup cg in COMMAND_REGISTRY) {
            if (cg.name != (string)args[1]) {
                continue;
            }

            ConsoleColors.printlnColored($"Usages for \'{args[1]}\':", Shell.PALETTE.colors[0]);
            
            foreach (Command c in cg.commands) {
                if (c.ee != true) {
                    ConsoleColors.printColored($"   {c.name}", Shell.PALETTE.colors[4]);
                    ConsoleColors.printColored($"{c.argumentsToString()}", Shell.PALETTE.colors[0]);
                    ConsoleColors.printlnColored($"  - {c.description}", ColorPalette.GRAY_9.colors[5]);
                }
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
        catch (FormatException) {
            Debug.error("Hex code is in invalid format!");
        }
        
    }, "sets a color in the main color palette");

    public static readonly Command COLORS_LIST_ALL = new("colors", [new FixedArgument("list")], args => {
        ColorPalette.printAllPalettes();
    }, "prints all palettes");
    
    // neofetch

    public static readonly Command NEOFETCH = new("neofetch", [], args => {
        CultureInfo myCI = new CultureInfo("cz-CZ");
        Calendar myCal = myCI.Calendar;
        CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
        DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
        
        ConsoleColors.printComplexColored("~       /#############\\,         \x1B[1m\x1B[4m!C O M M A N D I E R\x1B[0m~ by KryKom\n" + 
                                             $"~      #@@/^^^^^^^^^\\@@#         !version:~ {DateTime.Today:yy}w{myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW)}d\n" + // TODO change version letter here 
                                             $"~    /%@(   (##*\\     (@%\\       !commands total:~ {COMMAND_REGISTRY.Count}\n" + 
                                             $"~   #@%/    @&&&@@(    \\%@#      !color palettes total:~ {ColorPalette.palettes.Count}\n" + 
                                              "~  #@#*     @%|  ,(@,   *#@#     ─────────\n" + 
                                              "~   #@%\\    @&%%&&#    /%@#      !main palette:       ", 
            [("~", Shell.PALETTE.colors[0]), ("!", Shell.PALETTE.colors[4])]);
        Shell.PALETTE.printPalette();
        ConsoleColors.printComplexColored("~    \\&&(   (##/^     (&&/       !grayscale palette:  ", [("~", Shell.PALETTE.colors[0]), ("!", Shell.PALETTE.colors[4])]);
        ColorPalette.GRAY_9.printPalette();
        ConsoleColors.printlnColored("     *#@@\\_________/@@#*        \n" + 
                                     "       \\#############/          \n", Shell.PALETTE.colors[0]);
    }, "writes a fancy logo with info");
    
    // clock command

    public static readonly Command CLOCK = new("clock", [], args => {
        DigitalClock.clock(Shell.PALETTE.colors[4]);
    }, "starts a digital clock screensaver");

    public static readonly Command CLOCK_A = new("clock", [new FixedArgument("analogue")], args => {
        AnalogueClock.clock(Shell.PALETTE);
    }, "starts an analogue clock screensaver");
    
    public static readonly Command CLOCK_D = new("clock", [new FixedArgument("digital")], args => {
        DigitalClock.clock(Shell.PALETTE.colors[4]);
    }, "starts an analogue clock screensaver");

    public static readonly Command CLOCK_E = new("clock", [new FixedArgument("rainbow")], args => {
        
        void action() {
            (int r, int g, int b) c = ColorFormat.ColorFromHSV(DateTime.Now.Second * 12, 1d, 1d);
            DigitalClock.COLOR = (c.r << 16) + (c.g << 8) + c.b;
        }

        DigitalClock.clock(0xff0000, action);
    }, "starts a digital rainbow clock screensaver", true);
    
    // exit command

    public static readonly Command EXIT = new Command("exit", [], args => {
        Shell.SHELL.stop();
    }, "exits the shell");

    // variable commands

    public static readonly Command VARIABLE_LIST = new("var", [new FixedArgument("list")], args => {
        foreach (Variable v in Variable.variables) {
            ConsoleColors.printColored($"{v.name}: ", Shell.PALETTE.colors[0]);
            ConsoleColors.printlnColored($"{v.value}", ColorPalette.GRAY_9.colors[3]);
        }
    }, "lists all available variables");

    public static readonly Command VARIABLE_ADD = new("var", [new FixedArgument("add"), new StringArgument("name"), new StringArgument("value")], args => {
        Variable v = new Variable(args[1].ToString()!, args[2].ToString()!); 
        Variable.addVariable(v);
    }, "creates a new variable");
    
    public static readonly Command VARIABLE_SET = new("var", [new FixedArgument("set"), new StringArgument("name"), new StringArgument("value")], args => {
        Variable.setVariable(args[1].ToString()!, args[2].ToString()!);
    }, "sets an existing variable");

    public static readonly Command VARIABLE_REMOVE = new("var", [new FixedArgument("delete"), new StringArgument("name")], args => {
        Variable.removeVariable(args[1].ToString()!);
    }, "removes a variable");

    // debug commands

    public static readonly Command DEBUG_LEVEL = new("debug", [new FixedArgument("level"), new IntArgument(0, 3, "level")], args => {
        switch (args[1]) {
            case 0: Debug.debugLevel = Debug.DebugLevel.NOTHING; break;
            case 1: Debug.debugLevel = Debug.DebugLevel.ONLY_ERRORS; break;
            case 2: Debug.debugLevel = Debug.DebugLevel.ERRORS_WARNS; break;
            case 3: Debug.debugLevel = Debug.DebugLevel.ALL; break;
        }
    }, "sets the debug message level");

    public static readonly Command DEBUG_INFO = new("debug", [new FixedArgument("info"), new StringArgument("message")], args => {
        Debug.info(args[1].ToString() ?? string.Empty, true);
    }, "prints a info message into the console");
    
    public static readonly Command DEBUG_WARN = new("debug", [new FixedArgument("warn"), new StringArgument("message")], args => {
        Debug.warn(args[1].ToString() ?? string.Empty, true);
    }, "prints a warning message into the console");
    
    public static readonly Command DEBUG_ERROR = new("debug", [new FixedArgument("error"), new StringArgument("message")], args => {
        Debug.error(args[1].ToString() ?? string.Empty, true);
    }, "prints a error message into the console");
    
    // clear command

    public static readonly Command CLEAR = new("clear", [], args => {
        Console.Clear();
    }, "clears the console history");
}

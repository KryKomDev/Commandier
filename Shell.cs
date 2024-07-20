//
// Commandier 
// by KryKom 2024
//

using System.Globalization;
using Kolors;

namespace Commandier;

public class Shell {

    public static Shell SHELL = new Shell();
    
    /// <summary>
    /// Length = 5, everything else will be ignored <br/>
    /// [0] -> normal text <br/>
    /// [1] -> error <br/>
    /// [2] -> warning <br/>
    /// [3] -> info <br/>
    /// [4] -> highlight
    /// </summary>
    public static ColorPalette PALETTE = new ColorPalette("f8ffe5-ef476f-ffc43d-06d6a0-1b9aaa");

    public static Action onStart = delegate {
        CultureInfo myCI = new CultureInfo("cz-CZ");
        Calendar myCal = myCI.Calendar;
        CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
        DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
        ConsoleColors.printlnColored($"\n\x1B[1mCommandier [{DateTime.Today:yy}w{myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW)}d] by KryKom\n(c) All rights probably not reserved :D", PALETTE.colors[4]); // TODO change version letter here
    }; 
    
    private bool running = false;
    private string prompt;
    
    public Shell(string prompt = ">") {
        this.prompt = prompt;
    }

    public void start() {
        
        CommandRegistry.registerDefault();

        onStart();
        
        running = true;
        
        while (running) {
            parse();
        }
    }

    private void parse() {
        ConsoleColors.printColored($"\n\x1B[1m{prompt}\x1B[22m ", PALETTE.colors[4]);
        string? raw = Console.ReadLine();
        
        if (string.IsNullOrEmpty(raw)) {
            return;
        }
        
        raw += " ";

        Command? runnable = null;

        string[] rawArgs = ArgumentParser.separate(raw);

        foreach (CommandGroup cg in CommandRegistry.getRegistry()) {

            if (rawArgs[0] != cg.name) {
                continue;
            }

            foreach (Command c in cg.commands) {
                if (rawArgs.Length - 1 != c.arguments.Length) {
                    continue;
                }

                bool matches = true;
                
                for (int i = 0; i < c.arguments.Length; i++) {
                    if (c.arguments[i].type() == "keyword" && (string)c.arguments[i].value == rawArgs[i + 1]) break;

                    matches = false;
                }

                if (matches) {
                    runnable = c;
                    break;
                }
            }
        }

        if (runnable == null) {
            Debug.error("No available command with this syntax.", true);
            return;
        }

        string args = "";
        int startI = 0;
        
        for (int i = 0; i < raw.Length; i++) {
            if (raw[i] == ' ') {
                startI = i + 1;
                break;
            }
        }

        for (int i = startI; i < raw.Length; i++) {
            args += "" + raw[i];
        }
        
        runnable.run(args);
    }

    public void stop() {
        running = false;
    }
}
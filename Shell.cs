//
// Commandier 
// by KryKom 2024
//

using Kolors;

namespace Commandier;

public class Shell {

    public static Shell SHELL = new Shell();
    
    private bool running = false;

    public void start() {
        
        CommandRegistry.registerDefault();
        
        running = true;
        
        while (running) {
            parse();
        }
    }

    private void parse() {
        ConsoleColors.printColored($"\n[{DateTime.Now:HH:mm:ss}]: \x1B[1m$\x1B[22m ", (int)Colors.GRAY_5);
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
            Debug.error("No available command with this syntax.");
            return;
        }

        string args = "";

        for (int i = 1; i < rawArgs.Length; i++) {
            args += $"{rawArgs[i]} ";
        }
        
        runnable.run(args);
    }

    public void stop() {
        running = false;
    }
}
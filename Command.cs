//
// VoxelsCoreSharp
// by KryKom
//

using Commandier.argument;
using Kolors;
using VoxelsCoreSharp.console.command.argument;

namespace Commandier;

public class Command {
    public readonly string name;
    public readonly ArgumentType[] arguments;
    private readonly Action<object[]> code;
    public readonly string description;

    public Command(string name, ArgumentType[] arguments, Action<object[]> code, string description = "this does not have a description...") {
        this.name = name;
        this.arguments = arguments;
        this.code = code;
        this.description = description;
    }

    public void run(string raw) {
        object[]? args = ArgumentParser.parse(raw, arguments, name);

        if (args == null) {
            return;
        }

        code(args);
    }

    public string argumentsToString() {

        string s = "";
        
        foreach (ArgumentType a in arguments) {

            if (a.type() == "keyword") {
                s += $" {a.value}";
            }
            else {
                s += $" <{a.type()}: {a.description}>";

            }
        }

        return s;
    }
}


/// <summary>
/// groups commands with the same name
/// </summary>
public class CommandGroup {

    public string name { get; private set; }
    public List<Command> commands { get; private set; } = new List<Command>();

    public CommandGroup(string name, Command[] commands) {
        this.name = name;

        foreach (Command c in commands) {
            addCommand(c);
        }
    }

    public CommandGroup(string name) {
        this.name = name;
    }

    public void addCommand(Command command) {
        if (command.name != name) {
            Debug.error($"Could not register command \'{command.name}{command.argumentsToString()}\' into the \'{name}\' command group! \n" +
                        $"           Supplied command does not have the same name as the group.");
            return;
        }
        
        foreach (Command c in commands) {

            if (command.arguments.Length != c.arguments.Length) {
                continue;
            }

            bool match = true;

            if (command.arguments.Length == 0 && c.arguments.Length == 0) {
                Debug.error($"Could not register command \'{command.name}{command.argumentsToString()}\' into the \'{name}\' command group! \n" +
                            $"           Supplied command already exists.");
                return;
            }
            
            for (int i = 0; i < command.arguments.Length; i++) {
                
                if (c.arguments[i].type() == command.arguments[i].type()) {
                    if (c.arguments[i].type() != "keyword" || c.arguments[i].value != command.arguments[i].value) {
                        match = false;
                        break;
                    }
                }
                else {
                    match = false;
                    break;
                }
            }

            if (match) {
                Debug.error($"Could not register command \'{command.name}{command.argumentsToString()}\' into the \'{name}\' command group! \n" +
                            $"           Command with the same arguments already exists.");
                return;
            }
        }
        
        // Debug.info($"Successfully added new command \'{command.name}{command.argumentsToString()}\' into the \'{name}\' command group.");
        commands.Add(command);
    }
}
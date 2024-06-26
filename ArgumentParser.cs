//
// Commandier
// by KryKom 2024
//

using System.Text.RegularExpressions;
using Commandier.argument;
using Kolors;

namespace Commandier;

/// <summary>
/// command argument separator and parser
/// </summary>
public static class ArgumentParser {
    
    /// <summary>
    /// separates arguments in string
    /// </summary>
    /// <param name="raw">raw argument string</param>
    /// <returns>array of separated arguments in string</returns>
    public static string[] separate(string raw) {
        List<string> args = new List<string>();

        string s = "";
        
        for (int i = 0; i < raw.Length; i++) {
            
            if (raw[i] == '"') {
                
                for (int j = i + 1; j < raw.Length; j++) {
                    if (raw[j] == '"') {
                        args.Add(s);
                        s = "";
                        i = j + 1;
                    }
                    else {
                        s += raw[j];
                    }
                }
            } else if (raw[i] == '*') {
                
                for (int j = i + 1; j < raw.Length; j++) {
                    if (raw[j] == '*') {
                        s = $"\"{Variable.getVariable(s)}\"";
                        args.Add(s);
                        s = "";
                        i = j + 1;
                    }
                    else {
                        s += raw[j];
                    }
                }
            }
            else if (raw[i] == ' ') {
                args.Add(s); 
                s = "";
            }
            else {
                s += raw[i];
            }
        }

        for (int i = 0; i < args.Count; i++) {
            if (args[i] == string.Empty) {
                args.RemoveAt(i);
            }
        }
        
        // foreach (var ss in args) {
        //     Debug.warn(ss, true);
        // }
        
        return args.ToArray();
    }

    /// <summary>
    /// converts raw string arguments into valid argument types
    /// </summary>
    /// <param name="raw">raw argument string</param>
    /// <param name="arguments">argument types</param>
    /// <returns>parsed arguments</returns>
    public static object[]? parse(string raw, ArgumentType[] arguments, string name) {
        
        // Debug.info(raw);
        
        string[] args = separate(raw);
        
        if (args.Length != arguments.Length) {
            Debug.error("incorrect number of arguments supplied!", true);
            return null;
        }

        object[] output = new object[args.Length];
        
        for (int i = 0; i < args.Length; i++) {
            
            ArgumentType? a = arguments[i].parse(args[i]);
            
            if (a == null) {
                Debug.error($"incorrect argument type supplied at position {i + 1}, argument must be type of <{arguments[i].type()}>!", true); 
                return null;
            }
            
            output[i] = a.value;
        }
        
        return output;
    }
}
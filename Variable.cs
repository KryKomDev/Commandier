//
// Commandier
// by KryKom 2024
//

using System.Text.RegularExpressions;
using Commandier.argument;
using Kolors;

namespace Commandier;

public class Variable(string name, string value) {

    public static readonly List<Variable> variables = new();
    
    
    public string name { get; private set; } = name;
    public string value { get; private set; } = value;

    
    public static void addVariable(Variable variable) {
        if (!Regex.IsMatch(variable.name, @"^[a-zA-Z_]+$")) {
            Debug.error("Variable has invalid name! Only alphabetical characters and '_' allowed.", true);
            return;
        }

        foreach (Variable v in variables) {
            if (v.name == variable.name) {
                Debug.error("Variable with the same name already exists!", true);
                return;
            }
        }
        
        variables.Add(variable);
    }

    public static string getVariable(string name) {
        foreach (Variable v in variables) {
            if (v.name == name) {
                return v.value;
            }
        }
        
        Debug.error("No variable with this name found!", true);

        return "";
    }

    public static void removeVariable(string name) {
        for (int i = 0; i < variables.Count; i++) {
            if (variables[i].name == name) {
                variables.RemoveAt(i);
                return;
            }
        }
        
        Debug.error("No variable with this name found!", true);
    }

    public static void setVariable(string name, string value) {
        for (int i = 0; i < variables.Count; i++) {
            if (variables[i].name == name) {
                variables[i].value = value;
                return;
            }
        }
        
        Debug.error("No variable with this name found!", true);
    }
}
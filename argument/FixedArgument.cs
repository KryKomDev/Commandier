//
// Commandier
// by KryKom 2024
//

namespace Commandier.argument;

public class FixedArgument : ArgumentType {
    
    public override object value { get; protected set; }

    public FixedArgument(string value) {
        this.value = value;
    }
    
    public override FixedArgument? parse(string raw) {
        if ((string)value == raw) {
            return new FixedArgument((string)value);
        }
        else {
            return null;
        }
    }

    public override string type() => "keyword";

    public override string description { get; protected set; } = "keyword";
}
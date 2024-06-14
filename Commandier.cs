using System.Text.RegularExpressions;
using Kolors;

namespace Commandier;

public class Commandier {
    public static void main() {

        Debug.errorColor = ColorPalette.BASE.colors[3];
        Debug.warnColor = ColorPalette.BASE.colors[4];
        Debug.infoColor = ColorPalette.BASE.colors[1];
        
        Shell.SHELL.start();
    }
}
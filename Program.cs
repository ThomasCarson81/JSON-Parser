using System.Diagnostics.CodeAnalysis;
using System.IO;


namespace JsonParser
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello, World!");
        }
    }

    public enum Token
    {
        OPENCURLY,
        CLOSECURLY,
        OPENSQUARE,
        CLOSESQUARE,
        OPENSINGLEQUOTE,
        CLOSESINGLEQUOTE,
        OPENDOUBLEQUOTE,
        CLOSEDOUBLEQUOTE,
        BOOL,
        CHAR,
        SPACE,
        BACKSLASH,
        NUMBER,
        DECIMALPOINT
    }

    public static class JSONParser
    {
        public static List<Token> Tokenize(string json)
        {

        }

    }
}

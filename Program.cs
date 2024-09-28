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

            List<Token> tokens = new List<Token>();

            int totalOpenCurlies = 0;
            int totalCloseCurlies = 0;
            int totalOpenSquares = 0;
            int totalCloseSquares = 0;
            int totalOpenSingleQuotes = 0;
            int totalCloseSingleQuotes = 0;
            int totalOpenDoubleQuotes = 0;
            int totalCloseDoubleQuotes = 0;

            int currentCurlies = 0;
            int currentSquares = 0;
            bool currentSingleQuotes = false;
            bool currentDoubleQuotes = false;

            bool ignore = false;

            foreach (char c in json)
            {
                if (ignore)
                {
                    tokens.Add(Token.CHAR);
                    ignore = false;
                    continue;
                }
                if (c == '\\')
                {
                    ignore = true;
                    continue;
                }
                if (c == '"')
                {
                    if (currentDoubleQuotes)
                    {
                        currentDoubleQuotes = false;
                        tokens.Add(Token.CLOSEDOUBLEQUOTE);
                        continue;
                    }
                    tokens.Add(Token.OPENDOUBLEQUOTE);
                    currentDoubleQuotes = true;
                    continue;
                }
                if (c == '\'')
                {
                    if (currentSingleQuotes)
                    {
                        currentSingleQuotes = false;
                        tokens.Add(Token.CLOSESINGLEQUOTE);
                        continue;
                    }
                    tokens.Add(Token.OPENSINGLEQUOTE);
                    currentSingleQuotes = true;
                    continue;
                }
                if (c == '{')
                {
                    currentCurlies++;
                    tokens.Add(Token.OPENCURLY);
                    continue;
                }
                if (c ==  '}')
                {
                    currentCurlies--;
                    tokens.Add(Token.CLOSECURLY);
                    continue;
                }
                if (c == '[')
                {
                    currentSquares++;
                    tokens.Add(Token.OPENSQUARE);
                    continue;
                }
                if (c == ']')
                {
                    currentSquares--;
                    tokens.Add(Token.CLOSESQUARE);
                    continue;
                }
            }

            if (totalOpenCurlies != totalCloseCurlies ||
                totalOpenSquares != totalCloseSquares ||
                totalOpenSingleQuotes != totalCloseSingleQuotes ||
                totalOpenDoubleQuotes != totalCloseDoubleQuotes)
            {
                throw new Exception("invalid JSON string");
            }

            return tokens;
        }
    }
}

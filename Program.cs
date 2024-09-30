using System.Diagnostics.CodeAnalysis;
using System.IO;


namespace JsonParser
{
    internal class Program
    {
        public static Dictionary<string, object> parsedObj = new Dictionary<string, object>();
        public static string jsonString = "{ \"name\": \"John\", \"age\": 46, \"isMale\": true }";
        static void Main()
        {
            parsedObj = JSONParser.Parse(jsonString);
            foreach (string key in parsedObj.Keys)
            {
                Console.WriteLine(key + ": " + parsedObj[key]);
            }
        }
    }

    public enum Token
    {
        OPENCURLY,
        CLOSECURLY,
        OPENSQUARE,
        CLOSESQUARE,
        OPENSTRING,
        CLOSESTRING,
        BOOL,
        CHAR,
        SPACE,
        BACKSLASH,
        NUMBER,
        DECIMALPOINT,
        NULL
    }

    public static class JSONParser
    {
        public static Dictionary<string, object> Parse(string json)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            return dict;
        }

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

            bool isInString = false;
            bool stringIsSingle = false;

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
                    if (isInString)
                    {
                        if (!stringIsSingle) // if string is made of ""
                        {
                            isInString = false;
                            tokens.Add(Token.CLOSESTRING);
                            continue;
                        }
                        // string is made of ''
                        tokens.Add(Token.CHAR);
                        continue;
                    }
                    // no active string, so start one
                    tokens.Add(Token.OPENSTRING);
                    stringIsSingle = false;
                    isInString = true;
                    continue;
                }
                if (c == '\'')
                {
                    if (isInString)
                    {
                        if (stringIsSingle) // if string is made of ''
                        {
                            isInString = false;
                            tokens.Add(Token.CLOSESTRING);
                            continue;
                        }
                        // string is made of ""
                        tokens.Add(Token.CHAR);
                        continue;
                    }
                    // no active string, so start one
                    tokens.Add(Token.OPENSTRING);
                    stringIsSingle = true;
                    isInString = true;
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

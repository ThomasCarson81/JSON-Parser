﻿using System;
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
        public const char COMMA = ',';
        public const char COLON = ':';
        public const char LEFTBRACKET = '[';
        public const char RIGHTBRACKET = ']';
        public const char LEFTBRACE = '{';
        public const char RIGHTBRACE = '}';
        public const char QUOTE = '"';

        public static readonly char[] WHITESPACE = [ ' ', '\t', '\b', '\n', '\r' ];
        public static readonly char[] SYNTAX = [COMMA, COLON,  LEFTBRACKET, RIGHTBRACKET, LEFTBRACE, RIGHTBRACE];

        public const int FALSE_LEN = 5;
        public const int TRUE_LEN = 4;
        public const int NULL_LEN = 4;

        public static Dictionary<string, object> Parse(string json)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            return dict;
        }

        public static List<object?> Lex(string str)
        {
            List<object?> tokens = new List<object?>();

            string? jsonString;
            object? jsonNumber;
            bool? jsonBool;
            bool? jsonNull;

            while (str.Length > 0)
            {
                (jsonString, str) = LexString(str);

                if (jsonString != null)
                {
                    tokens.Add(jsonString);
                    continue;
                }

                bool isInt;
                (jsonNumber, str, isInt) = LexNumber(str);

                if (jsonNumber != null)
                {
                    if (isInt)
                    {
                        tokens.Add((int)jsonNumber);
                    }
                    else
                    {
                        tokens.Add((float)jsonNumber);
                    }
                    continue;
                }

                (jsonBool, str) = LexBool(str);
                if (jsonBool != null)
                {
                    tokens.Add(jsonBool);
                    continue;
                }
                if (jsonNull != null)
                {
                    tokens.Add(jsonNull);
                    continue
                }

                if (WHITESPACE.Contains(str[0]))
                {
                    str = str.Substring(1);
                }
                else if (SYNTAX.Contains(str[0]))
                {
                    tokens.Add(Convert.ToString(str[0]));
                    str = str.Substring(1);
                }
                else
                {
                    throw new Exception($"Unexpected character: {str[0]}");
                }
            }
            return tokens;
        }

        public static (string?, string) LexString(string str)
        {
            string jsonString = "";

            if (str[0] == QUOTE)
            {
                str = str.Substring(1);
            }
            else 
            {
                return (null, str);
            }
            foreach (char c in str)
            {
                if (c == QUOTE)
                {
                    return (jsonString, str.Substring(jsonString.Length + 1));
                }
                else
                {
                    jsonString += c;
                }
            }

            throw new Exception("Expected end-of-string quote");
        }
        public static (bool?, string) LexBool(string str)
        {
            int string_len = str.Length;
            if (string_len >= TRUE_LEN &&
                str.Substring(0, TRUE_LEN) == "true"
               )
            {
                return (true, str.Substring(TRUE_LEN));
            }
            else if (string_len >= FALSE_LEN &&
                     str.Substring(0, FALSE_LEN) == "false"
                    )
            {
                return (false, str.Substring(FALSE_LEN));
            }
            return (null, str);
        }
        public static (float?, string, bool) LexNumber(string str)
        {
            string jsonNumber = "";
            char[] numberChars = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', 'e', '.'];
            foreach (char c in str)
            {
                if (numberChars.Contains(c))
                {
                    jsonNumber += c;
                }
                else
                {
                    break;
                }
            }
            string rest = str.Substring(jsonNumber.Length);

            if (jsonNumber.Length == 0)
            {
                return (null, str, false);
            }
            if (jsonNumber.Contains('.'))
            {
                return (Convert.ToSingle(jsonNumber), rest, false);
            }
            return (Convert.ToInt32(jsonNumber), rest, true);
        }
        public static (bool?, string) LexNull(string str)
        {
            int string_len = str.Length;

            if (string_len >= NULL_LEN && 
                str.Substring(0,NULL_LEN) == "null"
               )
            {
                return (true, str.Substring(NULL_LEN));
            }
            return (null, str);
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

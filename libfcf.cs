using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libfcf
{

    public class Token
    {
        public string value = "";

        public int lineStart;
        public int lineEnd;
        public int charStart;
        public int charEnd;  
    }

    internal class TokenIdentifier : Token
    {
        public new string value;

        public new int lineStart;
        public new int lineEnd;
        public new int charStart;
        public new int charEnd;

        public TokenIdentifier(string value, int lineStart, int lineEnd, int charStart, int charEnd)
        {
            this.value = value;

            this.lineStart = lineStart;
            this.lineEnd = lineEnd;
            this.charStart = charStart;
            this.charEnd = charEnd;
        }
    }

    internal class TokenString : Token
    {
        public new string value;

        public new int lineStart;
        public new int lineEnd;
        public new int charStart;
        public new int charEnd;

        public TokenString(string value, int lineStart, int lineEnd, int charStart, int charEnd)
        {
            this.value = value;
            
            this.lineStart = lineStart;
            this.lineEnd = lineEnd;
            this.charStart = charStart;
            this.charEnd = charEnd;
        }
    }

    internal class TokenNumber : Token
    {
        public new float value;

        public new int lineStart;
        public new int lineEnd;
        public new int charStart;
        public new int charEnd;

        public TokenNumber(float value, int lineStart, int lineEnd, int charStart, int charEnd)
        {
            this.value = value;
            
            this.lineStart = lineStart;
            this.lineEnd = lineEnd;
            this.charStart = charStart;
            this.charEnd = charEnd;
        }
    }

    internal class TokenBoolean : Token
    {
        public new bool value;

        public new int lineStart;
        public new int lineEnd;
        public new int charStart;
        public new int charEnd;

        public TokenBoolean(bool value, int lineStart, int lineEnd, int charStart, int charEnd)
        {
            this.value = value;
            
            this.lineStart = lineStart;
            this.lineEnd = lineEnd;
            this.charStart = charStart;
            this.charEnd = charEnd;
        }
    }

    internal class TokenAssign : Token
    {
        public new string value = "=";

        public new int lineStart;
        public new int lineEnd;
        public new int charStart;
        public new int charEnd;

        public TokenAssign(int lineStart, int lineEnd, int charStart, int charEnd)
        {
            this.lineStart = lineStart;
            this.lineEnd = lineEnd;
            this.charStart = charStart;
            this.charEnd = charEnd;
        }
    }

    internal class TokenComma : Token
    {
        public new string value = ",";

        public new int lineStart;
        public new int lineEnd;
        public new int charStart;
        public new int charEnd;

        public TokenComma(int lineStart, int lineEnd, int charStart, int charEnd)
        {
            this.lineStart = lineStart;
            this.lineEnd = lineEnd;
            this.charStart = charStart;
            this.charEnd = charEnd;
        }
    }

    internal class TokenArrayStart : Token
    {
        public new string value = "[";

        public new int lineStart;
        public new int lineEnd;
        public new int charStart;
        public new int charEnd;

        public TokenArrayStart(int lineStart, int lineEnd, int charStart, int charEnd)
        {
            this.lineStart = lineStart;
            this.lineEnd = lineEnd;
            this.charStart = charStart;
            this.charEnd = charEnd;
        }
    }

    internal class TokenArrayEnd : Token
    {
        public new string value = "]";

        public new int lineStart;
        public new int lineEnd;
        public new int charStart;
        public new int charEnd;

        public TokenArrayEnd(int lineStart, int lineEnd, int charStart, int charEnd)
        {
            this.lineStart = lineStart;
            this.lineEnd = lineEnd;
            this.charStart = charStart;
            this.charEnd = charEnd;
        }
    }

    internal class TokenDictStart : Token
    {
        public new string value = "{";

        public new int lineStart;
        public new int lineEnd;
        public new int charStart;
        public new int charEnd;

        public TokenDictStart(int lineStart, int lineEnd, int charStart, int charEnd)
        {
            this.lineStart = lineStart;
            this.lineEnd = lineEnd;
            this.charStart = charStart;
            this.charEnd = charEnd;
        }
    }

    internal class TokenDictEnd : Token
    {
        public new string value = "}";

        public new int lineStart;
        public new int lineEnd;
        public new int charStart;
        public new int charEnd;

        public TokenDictEnd(int lineStart, int lineEnd, int charStart, int charEnd)
        {
            this.lineStart = lineStart;
            this.lineEnd = lineEnd;
            this.charStart = charStart;
            this.charEnd = charEnd;
        }
    }

    public class InvalidTokenTypeException : Exception
    {
        public dynamic token;

        public InvalidTokenTypeException(string message, dynamic token) : base(message)
        {
            this.token = token;
        }
    }


    public static class FCF
    {
        internal static string GetTokenAsHuman(Token t)
        {
            if (t.GetType() == typeof(TokenString))
            {
                return "[" + t + ": \"" + ((dynamic)t).value + "\"]";
            }
            else
            {
                return "[" + t + "]";
            }
        }

        public static string[] SterilizeStringArray(string[] fileContents) {
            List<string> fileContentsArr = new List<string>();
            foreach (string line in fileContents)
            {
                string line2 = line.Split("//").First();
                line2 = line2 + "\n";

                fileContentsArr.Add(line2.TrimStart().TrimEnd());
            }

            List<string> newFileContentsArr = new List<string>();
            bool isString = false;
            foreach(string line in fileContentsArr) {
                string newLine = "";
                foreach (char c in line)
                {
                    if (c == '"')
                    {
                        isString = !isString;
                    }

                    if (!isString && c == ' ')
                    {
                        continue;
                    }

                    newLine += c;
                }
                // if(isString) {
                //     newLine += "\n";
                // }
                newFileContentsArr.Add(newLine);
            }
            return newFileContentsArr.ToArray();

        }

        internal static Token[] TokenizeFromFile(string path)
        {
            string[] fileContentsArray = File.ReadAllLines(path);

            return TokenizeFromMemory(SterilizeStringArray(fileContentsArray));
        }

        internal static Token[] TokenizeFromMemory(string[] fileContents)
        {
            // fileContents = "{" + fileContents + "}";

            fileContents[0] = fileContents[0].Insert(0, "{");
            fileContents[fileContents.Length - 1] = fileContents[fileContents.Length - 1].Insert(fileContents[fileContents.Length - 1].Length, "}");

            // Console.WriteLine(fileContents[0]);

            List<Token> stack = new List<Token>();

            string currentWord = "";
            bool isString = false;

            var boolvalues = new String[2] { "true", "false" };

            int lineIndex = -1;
            int charIndex = -1;

            void HandleCurrentWord() {
                if (currentWord != "")
                {
                    float value = 0f;
                    if (float.TryParse(currentWord, out value))
                    {
                        stack.Add(new TokenNumber(value, lineIndex, lineIndex, charIndex - currentWord.Length, charIndex));
                        currentWord = "";
                    }
                    else
                    {
                        if (boolvalues.Contains(currentWord))
                        {
                            stack.Add(new TokenBoolean(currentWord == "true", lineIndex, lineIndex, charIndex - currentWord.Length, charIndex));
                        }
                        else
                        {
                            stack.Add(new TokenIdentifier(currentWord, lineIndex, lineIndex, charIndex - currentWord.Length, charIndex));
                        }

                        currentWord = "";
                    }
                }
            }

            foreach(string line in fileContents) {
                lineIndex ++;
                charIndex = -1;
                foreach (char c in line)
                {
                    charIndex++;
                    switch (c)
                    {
                        case '=':
                            HandleCurrentWord();

                            stack.Add(new TokenAssign(lineIndex, lineIndex, charIndex, charIndex + 1));
                            break;
                        case ',':
                            HandleCurrentWord();

                            stack.Add(new TokenComma(lineIndex, lineIndex, charIndex, charIndex + 1));
                            break;
                        case '[':
                            HandleCurrentWord();

                            stack.Add(new TokenArrayStart(lineIndex, lineIndex, charIndex, charIndex + 1));
                            break;
                        case ']':
                            HandleCurrentWord();

                            stack.Add(new TokenArrayEnd(lineIndex, lineIndex, charIndex, charIndex + 1));
                            break;
                        case '{':
                            HandleCurrentWord();

                            stack.Add(new TokenDictStart(lineIndex, lineIndex, charIndex, charIndex + 1));
                            break;
                        case '}':
                            HandleCurrentWord();

                            stack.Add(new TokenDictEnd(lineIndex, lineIndex, charIndex, charIndex + 1));
                            break;
                        default:
                            if (c != '"')
                            {
                                currentWord += c;
                            }

                            if (c == '"')
                            {
                                if (isString)
                                {
                                    stack.Add(new TokenString(currentWord, lineIndex, lineIndex, charIndex - currentWord.Length - 1, charIndex + 1));
                                    currentWord = "";
                                }

                                isString = !isString;
                            }

                            break;
                    }
                }
            }

            return stack.ToArray();
        }

        internal static dynamic[] ParseDict(Token[] tokens)
        {
            Dictionary<string, dynamic> json_object = new Dictionary<string, dynamic>();
            var t = tokens[0];
            if (t.GetType() == typeof(TokenDictEnd))
            {
                return new dynamic[] { json_object, tokens.ToList().Skip(1).ToArray() };
            }

            while (true)
            {
                var json_key = tokens[0];
                if (json_key.GetType() == typeof(TokenIdentifier))
                {
                    tokens = tokens.ToList().Skip(1).ToArray();
                }
                else
                {
                    throw new InvalidTokenTypeException("Expected identifier type for key got: " + GetTokenAsHuman(json_key), json_key);
                }

                if (tokens[0].GetType() != typeof(TokenAssign))
                {
                    throw new InvalidTokenTypeException($"Expected '=' got {GetTokenAsHuman(tokens[0])}", tokens[0]);
                }

                var tmp = Parse(tokens.ToList().Skip(1).ToArray());
                var json_value = tmp[0];
                tokens = tmp[1];

                if (!(json_value.GetType() == typeof(TokenString) || json_value.GetType() == typeof(TokenNumber) || json_value.GetType() == typeof(TokenBoolean) ||
                     json_value.GetType() == typeof(List<dynamic>) || json_value.GetType() == typeof(Dictionary<string, dynamic>)))
                {
                    throw new InvalidTokenTypeException($"expected string, number or bool got {GetTokenAsHuman(json_value)}", json_value);
                }

                // json_object.Add((string)(((TokenIdentifier)json_key).value), json_value);

                if (json_value.GetType() == typeof(TokenString) || json_value.GetType() == typeof(TokenNumber) || json_value.GetType() == typeof(TokenBoolean))
                {
                    json_object.Add((string)(((TokenIdentifier)json_key).value), ((dynamic)json_value).value);
                }
                else
                {
                    json_object.Add((string)(((TokenIdentifier)json_key).value), json_value);
                }

                t = tokens[0];
                if (t.GetType() == typeof(TokenDictEnd))
                {
                    return new dynamic[] { json_object, tokens.ToList().Skip(1).ToArray() };
                }
                else if (t.GetType() != typeof(TokenComma))
                {
                    throw new InvalidTokenTypeException($"Expected '=' after pair in object got {GetTokenAsHuman(t)}", t);
                }

                tokens = tokens.ToList().Skip(1).ToArray();
            }
        }

        internal static dynamic[] ParseArray(Token[] tokens)
        {
            List<dynamic> json_array = new List<dynamic>();

            var t = tokens[0];
            if (t.GetType() == typeof(TokenArrayEnd))
            {
                return new dynamic[] { json_array, tokens.ToList().Skip(1).ToArray() };
            }

            while (true)
            {
                var tmp = Parse(tokens);
                var json = tmp[0];
                tokens = tmp[1];

                if (json.GetType() == typeof(TokenString) || json.GetType() == typeof(TokenNumber) || json.GetType() == typeof(TokenBoolean) || json.GetType() == typeof(TokenIdentifier))
                {
                    json_array.Add(((dynamic)json).value);
                }
                else
                {
                    json_array.Add(json);
                }

                t = tokens[0];
                if (t.GetType() == typeof(TokenArrayEnd))
                {
                    return new dynamic[] { json_array, tokens.ToList().Skip(1).ToArray() };
                }
                else if (t.GetType() != typeof(TokenComma))
                {
                    throw new InvalidTokenTypeException($"Expected comma after object in array got {GetTokenAsHuman(t)}", t);
                }
                else
                {
                    tokens = tokens.ToList().Skip(1).ToArray();
                }
            }
        }

        internal static dynamic[] Parse(Token[] tokens)
        {
            var t = tokens[0];

            // Console.WriteLine("Parsing Array");
            if (t.GetType() == typeof(TokenArrayStart))
            {
                // Console.WriteLine("Parsing Array: " + t);
                return ParseArray(tokens.ToList().Skip(1).ToArray());
            }
            else if (t.GetType() == typeof(TokenDictStart))
            {
                // Console.WriteLine("Parsing Dict: " + t);
                return ParseDict(tokens.ToList().Skip(1).ToArray());
            }
            else
            {
                return new dynamic[] { t, tokens.ToList().Skip(1).ToArray() };
            }
        }

        public static string SerializeObjectToJson(dynamic obj)
        {
            string output = "";

            int i = -1;
            if (obj.GetType() == typeof(Dictionary<string, dynamic>))
            {
                output += "{";
                foreach (KeyValuePair<string, dynamic> kvp in obj)
                {
                    i++;
                    if (kvp.Value.GetType() == typeof(List<dynamic>) || kvp.Value.GetType() == typeof(Dictionary<string, dynamic>))
                    {
                        output += $"\"{kvp.Key}\": {SerializeObjectToJson(kvp.Value)}";
                    }
                    else
                    {
                        if (kvp.Value.GetType() == typeof(string))
                        {
                            output += $"\"{kvp.Key}\": \"{kvp.Value}\"";
                        }
                        else if (kvp.Value.GetType() == typeof(bool))
                        {
                            var boolval = kvp.Value ? "true" : "false";
                            output += $"\"{kvp.Key}\": {boolval}";
                        }
                        else
                        {
                            output += $"\"{kvp.Key}\": {kvp.Value}";
                        }
                    }
                    if (i != obj.Count - 1)
                    {
                        output += ", ";
                    }
                }
                output += "}";
            }
            else if (obj.GetType() == typeof(List<dynamic>))
            {
                output += "[";
                foreach (dynamic val in obj)
                {
                    i++;
                    if (val.GetType() == typeof(List<dynamic>) || val.GetType() == typeof(Dictionary<string, dynamic>))
                    {
                        output += $"{SerializeObjectToJson(val)}";
                    }
                    else
                    {
                        if (val.GetType() == typeof(string))
                        {
                            output += $"\"{val}\"";
                        }
                        else if (val.GetType() == typeof(bool))
                        {
                            output += val ? "true" : "false";
                        }
                        else
                        {
                            output += val;
                        }
                    }
                    if (i != obj.Count - 1)
                    {
                        output += ", ";
                    }
                }
                output += "]";
            }

            return output;
        }

        public static string SerializeObject(dynamic obj)
        {
            string output = "";

            int i = -1;
            if (obj.GetType() == typeof(Dictionary<string, dynamic>))
            {
                output += "{";
                foreach (KeyValuePair<string, dynamic> kvp in obj)
                {
                    i++;
                    if (kvp.Value.GetType() == typeof(List<dynamic>) || kvp.Value.GetType() == typeof(Dictionary<string, dynamic>))
                    {
                        output += $"{kvp.Key} = {SerializeObject(kvp.Value)}";
                    }
                    else
                    {
                        if (kvp.Value.GetType() == typeof(string))
                        {
                            output += $"{kvp.Key} = \"{kvp.Value}\"";
                        }
                        else if (kvp.Value.GetType() == typeof(bool))
                        {
                            var boolval = kvp.Value ? "true" : "false";
                            output += $"{kvp.Key} = {boolval}";
                        }
                        else
                        {
                            output += $"{kvp.Key} = {kvp.Value}";
                        }
                    }
                    if (i != obj.Count - 1)
                    {
                        output += ", ";
                    }
                }
                output += "}";
            }
            else if (obj.GetType() == typeof(List<dynamic>))
            {
                output += "[";
                foreach (dynamic val in obj)
                {
                    i++;
                    if (val.GetType() == typeof(List<dynamic>) || val.GetType() == typeof(Dictionary<string, dynamic>))
                    {
                        output += $"{SerializeObject(val)}";
                    }
                    else
                    {
                        if (val.GetType() == typeof(string))
                        {
                            output += $"\"{val}\"";
                        }
                        else if (val.GetType() == typeof(bool))
                        {
                            output += val ? "true" : "false";
                        }
                        else
                        {
                            output += val;
                        }
                    }
                    if (i != obj.Count - 1)
                    {
                        output += ", ";
                    }
                }
                output += "]";
            }

            return output;
        }

        public static dynamic DeserializeObject(Token[] tokens)
        {
            return Parse(tokens)[0];
        }

        public static dynamic DeserializeObjectFromFile(string filePath)
        {
            var stack = TokenizeFromFile(filePath);

            // foreach(var d in stack) {
            //     Console.WriteLine(d + ": \"" + ((dynamic)d).value + "\"");
            // }

            return DeserializeObject(stack);
        }

        public static dynamic DeserializeObjectFromMemory(string memory)
        {
            var stack = TokenizeFromMemory(new string[] {memory});
            return DeserializeObject(stack);
        }
    }
}

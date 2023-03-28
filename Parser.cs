using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static libfcf.Tokenizer;

namespace libfcf;

public static class Parser
{
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

    public static Dictionary<string, object>  DeserializeObject(Token[] tokens)
    {
        return (Dictionary<string, object>)Parse(tokens)[0];
    }

    public static Dictionary<string, object> DeserializeObjectFromFile(string filePath)
    {
        var stack = TokenizeFromFile(filePath);

        return DeserializeObject(stack);
    }

    public static List<object>  DeserializeArray(Token[] tokens)
    {
        return (List<object>)Parse(tokens)[0];
    }

    public static List<object> DeserializeArrayFromFile(string filePath)
    {
        var stack = TokenizeFromFile(filePath);

        return DeserializeArray(stack);
    }

    public static List<object> DeserializeArrayFromMemory(string memory)
    {
        var stack = TokenizeFromMemory(memory.Split('\n'));
        return DeserializeArray(stack);
    }
}

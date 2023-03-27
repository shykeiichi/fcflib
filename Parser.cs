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

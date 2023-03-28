namespace libfcf;

public static class Serializer {
    public static string SerializeObjectToJson(Dictionary<string, object> obj)
    {
        string output = "";

        int i = -1;
        if (obj.GetType() == typeof(Dictionary<string, object>))
        {
            output += "{";
            foreach (KeyValuePair<string, object> kvp in obj)
            {
                i++;
                if (kvp.Value.GetType() == typeof(List<object>) || kvp.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    output += $"\"{kvp.Key}\": {SerializeObjectToJson((Dictionary<string, object>)kvp.Value)}";
                }
                else
                {
                    if (kvp.Value.GetType() == typeof(string))
                    {
                        output += $"\"{kvp.Key}\": \"{kvp.Value}\"";
                    }
                    else if (kvp.Value.GetType() == typeof(bool))
                    {
                        var boolval = ((bool)kvp.Value).ToString().ToLower();
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
        else if (obj.GetType() == typeof(List<object>))
        {
            output += "[";
            foreach (object val in obj)
            {
                i++;
                if (val.GetType() == typeof(List<object>))
                {
                    output += $"{SerializeObjectToJson((List<object>)val)}";
                } else if(val.GetType() == typeof(Dictionary<string, object>)) 
                {
                    output += $"{SerializeObjectToJson((Dictionary<string, object>)val)}";
                }
                else
                {
                    if (val.GetType() == typeof(string))
                    {
                        output += $"\"{val}\"";
                    }
                    else if (val.GetType() == typeof(bool))
                    {
                        output += ((bool)val).ToString().ToLower();
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

    public static string SerializeObjectToJson(List<object> obj)
    {
        string output = "";

        int i = -1;

        output += "[";
        foreach (object val in obj)
        {
            i++;
            if (val.GetType() == typeof(List<object>))
            {
                output += $"{SerializeObjectToJson((List<object>)val)}";
            }
            else if(val.GetType() == typeof(Dictionary<string, object>)) 
            {
                output += $"{SerializeObjectToJson((Dictionary<string, object>)val)}";
            }
            else
            {
                if (val.GetType() == typeof(string))
                {
                    output += $"\"{val}\"";
                }
                else if (val.GetType() == typeof(bool))
                {
                    output += ((bool)val).ToString().ToLower();
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

        return output;
    }

    public static string SerializeObject(Dictionary<string, object> obj)
    {
        string output = "";

        int i = -1;

        output += "{";
        foreach (KeyValuePair<string, object> kvp in obj)
        {
            i++;
            if (kvp.Value.GetType() == typeof(List<object>))
            {
                output += $"{kvp.Key} = {SerializeObject((List<object>)kvp.Value)}";
            }
            else if(kvp.Value.GetType() == typeof(Dictionary<string, object>)) 
            {
                output += $"{kvp.Key} = {SerializeObject((Dictionary<string, object>)kvp.Value)}";
            }
            else
            {
                if (kvp.Value.GetType() == typeof(string))
                {
                    output += $"{kvp.Key} = \"{kvp.Value}\"";
                }
                else if (kvp.Value.GetType() == typeof(bool))
                {
                    var boolval = ((bool)kvp.Value).ToString().ToLower();
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

        return output;
    }

    public static string SerializeObject(List<object> obj)
    {
        string output = "";

        int i = -1;

        output += "[";
        foreach (object val in obj)
        {
            i++;
            if (val.GetType() == typeof(List<object>))
            {
                output += $"{SerializeObject((List<object>)val)}";
            } else if(val.GetType() == typeof(Dictionary<string, object>)) 
            {
                output += $"{SerializeObject((Dictionary<string, object>)val)}";
            }
            else
            {
                if (val.GetType() == typeof(string))
                {
                    output += $"\"{val}\"";
                }
                else if (val.GetType() == typeof(bool))
                {
                    output += ((bool)val).ToString().ToLower();
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

        return output;
    }
}
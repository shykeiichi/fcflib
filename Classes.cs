namespace libfcf;

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
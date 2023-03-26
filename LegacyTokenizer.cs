namespace libfcf;

public static class LegacyTokenizer {
    internal static string GetTokenAsHuman(Token t)
    {
        if (t.GetType() == typeof(TokenString))
        {
            return $"[{t}: \"{((dynamic)t).value}\"]";
        }
        else
        {
            return $"[{t}: '{((dynamic)t).value}']";
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
}
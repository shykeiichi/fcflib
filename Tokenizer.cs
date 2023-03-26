namespace libfcf;

public static class Tokenizer {
    public static Token[] TokenizeFromFile(string path) {
        return TokenizeFromMemory(File.ReadAllLines(path));
    }   

    public static Token[] TokenizeFromMemory(string[] fileLines) {
        fileLines[0] = fileLines[0].Insert(0, "{");
        fileLines[fileLines.Length - 1] = fileLines[fileLines.Length - 1].Insert(fileLines[fileLines.Length - 1].Length, "}");

        for(var i = 0; i < fileLines.Length; i++) {
            fileLines[i] = fileLines[i].Split("//")[0];
        } 

        List<Token> tokens = new List<Token>();

        string currentWord = "";
        
        int lineIdx;
        int charIdx;

        bool isString = false;
        string[] boolValues = new string[] {"false", "true"};

        void HandleCurrentWord() {
            if(currentWord != "") {
                if(isString) {
                    tokens.Add(new TokenString(currentWord, lineIdx, lineIdx, charIdx - currentWord.Length, charIdx));
                } else {
                    float value;
                    if(float.TryParse(currentWord, out value)) {
                        tokens.Add(new TokenNumber(value, lineIdx, lineIdx, charIdx - value.ToString().Length, charIdx));
                    } else {
                        if(boolValues.Contains(currentWord.ToLower())) {
                            tokens.Add(new TokenBoolean(currentWord.ToLower() == "true", lineIdx, lineIdx, charIdx - currentWord.Length, charIdx));
                        } else {
                            tokens.Add(new TokenIdentifier(currentWord, lineIdx, lineIdx, charIdx - currentWord.Length, charIdx));
                        }
                    }
                }
            }
            currentWord = "";
        }

        lineIdx = -1;
        foreach(string line in fileLines) {
            lineIdx++;
            charIdx = -1;
            foreach(char c in line) {
                charIdx++;

                if(isString) {
                    if(c == '"') {
                        HandleCurrentWord();
                        isString = false;
                        continue;
                    }
                    currentWord += c;
                    continue;
                }

                switch(c) {
                    case '=': {
                        HandleCurrentWord();

                        tokens.Add(new TokenAssign(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;
                    case '"': {
                        isString = true;
                    } break;
                    case ',': {
                        HandleCurrentWord();

                        tokens.Add(new TokenComma(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;

                    case '[': {
                        HandleCurrentWord();

                        tokens.Add(new TokenArrayStart(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;
                    case ']': {
                        HandleCurrentWord();

                        tokens.Add(new TokenArrayEnd(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;

                    case '{': {
                        HandleCurrentWord();

                        tokens.Add(new TokenDictStart(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;
                    case '}': {
                        HandleCurrentWord();

                        tokens.Add(new TokenDictEnd(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;
                    default: {
                        if(c != ' ') {
                            currentWord += c;
                        } else {
                            HandleCurrentWord();
                        }
                    } break;
                }
            }
        }

        return tokens.ToArray();
    }
}
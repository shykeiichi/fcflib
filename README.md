# Fjord Config Format

## How to use

### Deserialize from file

```c#
using libfcf;

Dictionary<string, dynamic> deserializedObject = Parser.DeserializeObjectFromFile("./config.fc");
```

### Deserialize from memory

```c#
using libfcf;

string memory = """
version = 1.4,
name = FCF,
buildOptions = [
    "Release",
    "Beta",
    "Alpha"
]
""";

Dictionary<string, dynamic> deserializedObject = Parser.DeserializeObjectFromMemory(memory);
```

## JSON Comparison

```json
{
    "persons": [
        {
            "name": "keii",
            "age": 16,
            "alive": true
        },
        {
            "name": "keii2",
            "age": 17,
            "alive": false
        }
    ]
}
```

```
persons = [
    {
        name = "keii",
        age = 16,
        alive = true
    },
    {
        name = "keii2",
        age = 17,
        alive = false
    }
]
```

Differences:
- Implied top level object eg. no need to surround top level object in curly braces "{}"
- Names are identifiers not strings to have a clear distinction. 'false', 'true' and all float parsable numbers are invalid identifiers.
- Usage of equals "=" instead of colon ":" to more clearly designate an assign operation
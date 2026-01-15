# jsoncheck

Validate JSON from clipboard, file, or stdin.

## Install (global tool)

dotnet tool install --global jsoncheck-cli

## Usage

### Clipboard

jsoncheck -c

### File

jsoncheck -f path/to/file.json

### Stdin (explicit)

jsoncheck -
echo '{ "a": 1 }' | jsoncheck -

### Disable colors

jsoncheck -c --no-color

## Exit codes

0 = valid JSON  
1 = invalid JSON  
2 = usage / input error

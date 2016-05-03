## Kaleidoscope's changes to current C# 6 language

### Language change
* Removed `operator true` and `operator false`
* Don't support '\U########' (UTF-32) character literal in `char` and `string`
* '\u####' character literal are still supported, but can only be used in `char` and `string` constants (not available in variable and function names, etc.)
* Failed to assign an `out` parameter will only issue a warning

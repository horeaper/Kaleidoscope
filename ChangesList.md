## Kaleidoscope's changes to current C# language

- Removed `operator true` and `operator false`
- No support for '\U########' (UTF-32) character literal
- '\u####' character literal are still supported, but can only be used in `char` and `string` constants (not available in class or type names, for example.)
- Failed to assign an `out` parameter will only issue a warning
- `dynamic` is now a keyword
- `var` is now a keyword
- `partial` no longer allowed on methods
- `unsafe` no longer allowed on class or methods, only available as block declare in method body
- `extern` only allowed on method, and is always (implicated) static

## Kaleidoscope's changes to current C# 6 language

### Minor language changes
* Removed `operator true` and `operator false`
* Don't support '\U########' (UTF-32) character literal in `char` and `string`
* '\u####' character literal are still supported, but can only be used in `char` and `string` constants (not available in variable and function names, etc.)
* Failed to assign an `out` parameter will only issue a warning
* `dynamic` is now a keyword
* `partial` no longer allowed on methods

### Allow `null` and `bool?` on if
```C#
var target = SomeMethod();  // 'target' is a reference type
if (target) {
    // will execute if target != null
}
if (!target) {
    // will execute if target == null
}
if ((target as MyClass)?.IsAlive) {     // 'IsAlive' is boolean
    // will execute if target is MyClass type and IsAlive is true
}
```

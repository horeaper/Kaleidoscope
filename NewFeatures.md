## New language features in Kaleidoscope

### Digit separator (extended C# 7 feature)
Use `'` (single quote) or `_` (underscore) as digit separator

```
0x80'00'00'00
100'0000'0000UL
123_456_789
3.14_15____926f     // only underscores can be used consecutively
0xFF'FF_FF'FF       // yes you can mix them!
```

Some WRONG (won't compile) examples:
```
3.14_           // separators cannot be the last digit character
1000'_0000      // looks weird, right?
200''000''000   // single quotes cannot be used consecutively
123456'.245     // separators must be followed by a digit number
```

### Binary number literal (C# 7 feature)
```
0b10010011
0b10'11'01'00
0b1100_1100_1100
```

### Octal number literal
```
0o103752
0o777'666'555
```

### `out` local variable
```C#
string text = "123123"
if (int.TryParse(text, out int value)) {
    // Do anything with 'value'
}
// 'value' will be out of scope
```

### `is` local variable (C# 7 feature)
```C#
object obj = SomeMethod();
if (obj is MyClass cls) {
    // Do whatever you like with 'cls'
}
// 'cls' will be out-of-scope
```

### `ref` to literals
```C#
void SomeMethod(ref int index) { /*...*/ }
void AnotherMethod(ref bool value) { /*...*/ }

SomeMethod(ref 0);
AnotherMethod(ref false);
```

### New "Keyword" generic constraint type
- `interface`
- `enum`, `enum(int)`, `enum(ushort)`, etc.
- `delegate(void(int, string))`, `delegate(int(void))`, etc.
- `cpp`, means C++ type only

The type constraint of enum can be any primitive integer type (`byte`, `ushort`, `int`, `long`, etc)

### `struct` inheritance
struct can inherit from these types:

- other structs
- C/C++ structure which contains only primitive (scalar) type members

### Inherit from generics
```C#
class MyClass<T> : T where T : ICloneable
{
    // Whatever
}
```

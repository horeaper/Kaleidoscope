## What is Kaleidoscope
Kaleidoscope transpiles C# code into C++ code, gains the ability to use C++ classes inside C# code directly (and some other things).
 
## What can Kaleidoscope do?
- Write C# code as you normally do (with a few differences)
- Use C++ classes directly (through a new contextual keyword "cpp::", write something like `cpp::std.vector<long>` and use C++ vector with C# `System.Int64`!
- Inline C/C++ code
- Generated code compiles on any modern C++ compilers (GCC 4.9+, clang 3.4+, VC2015+)

## What Kaleidoscope CANNOT do?
- Compile to IL (I'm NOT Mono/Roslyn)
- Translate IL to C++ code (I'm NOT IL2CPP)
- Generate binary assembly directly (I'm NOT IL2ASM)

## What Kaleidoscope is going to do?
- UWP, Android and iOS support
- Emscripten support (when threading is available)
- COM compatiability

## What are the current status of Kaleidoscope?
Still working on the basics =_=

- [x] Tokenizer
- [x] Analyzer
- [ ] Binder
- [ ] Abstract Expression 
- [ ] Structure Generator
- [ ] Statement Generator
- [ ] Additional Features

### And more...
- Trello page: [https://trello.com/b/kUBGwx7Z](https://trello.com/b/kUBGwx7Z)

## What is Kaleidoscope
Kaleidoscope transpiles C# code into C++ code, gains the ability to use C++ classes inside C# code directly (and some other things).
 
## What can Kaleidoscope do?
- Write C# code as you normally do (with only a few differences, very few, I sware!)
- Use C++ classes directly (through a new contextual keyword "cpp::", write something like `cpp::std.vector<long>` and use C++ vector with C# `System.Int64`!
- Inline C/C++ code
- Generated code compiles on any modern C++ compilers (GCC 4.9+, clang 3.4+, VC2015+)

## What Kaleidoscope CANNOT do?
- Compile to IL (I'm NOT Mono/Roslyn)
- Translate IL to C++ code (I'm NOT IL2CPP)
- Generate assembly directly (I'm NOT IL2ASM)

## What Kaleidoscope is going to do?
- Create a new syntax for C# code which looks like F# (They keep asking: "When are you going to remove the curly braces and make C# 100-percent F#?" How about now? ^_^)
- Write F# codes (real F# codes!)
- Android and iOS support
- Works with Emscripten
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
- My blog (Chinese): [http://blog.sina.com.cn/u/2140949941](http://blog.sina.com.cn/u/2140949941)
- My blog (English): [https://horeaper.blogspot.com/](https://horeaper.blogspot.com/)

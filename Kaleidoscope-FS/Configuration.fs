module Configuration

open System.Collections.Generic

let mutable IsMiniMode = false

let DefinedSymbols = new SortedSet<string>()
let mutable OutputFolder : string option = None
let mutable IsDebugMode = false
let IncludeSearchPaths = new List<string>()
let AdditionalIncludeFiles = new List<string>()

let mutable InputFiles : Kaleidoscope.SourceTextFile[] option = None

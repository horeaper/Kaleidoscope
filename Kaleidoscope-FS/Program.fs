open System.IO
open System.Linq

let CommandLineHelpString = 
    @"Kaleidoscope Transpiler (alpha)
usage: kalc [options] <input_files...>

Options:
  -h                Display this help info
  -D <SYMBOL>       Define conditional compilation symbol
  -o <folder>       Set output directory. If not specified, the file will be generated in current directory
  -debug            Set transpiler's output to debug mode
  -inc <folder>     Specify C++ header files search path
  -header <file>    Include additional C++ header file

Internal options:
  -mini             Enable mini mode (don't include standard libraries)
  -i <folder>       Include all *.cs files in this folder to process
"

let readFileContent file =
    try
        use fileHandle = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        use reader = new StreamReader(fileHandle)
        (true, reader.ReadToEnd())
    with
        | _ ->
            (false, "")


let processCommandArguments (argv:string[]) = 
    try
        let inputFiles = seq {
            let mutable currentIndex = 0
            let mutable isRunning = true
            while isRunning do
                match argv.[currentIndex] with
                | "-h" ->
                    printfn "%s" CommandLineHelpString
                    isRunning <- false
                | "-D" ->
                    currentIndex <- currentIndex + 1
                    Configuration.DefinedSymbols.Add argv.[currentIndex] |> ignore
                | "-o" ->
                    currentIndex <- currentIndex + 1
                    Configuration.OutputFolder <- Some(argv.[currentIndex])
                | "-debug" ->
                    Configuration.IsDebugMode <- true
                | "-inc" ->
                    currentIndex <- currentIndex + 1
                    Configuration.IncludeSearchPaths.Add(argv.[currentIndex])
                | "-header" ->
                    currentIndex <- currentIndex + 1
                    Configuration.AdditionalIncludeFiles.Add(argv.[currentIndex])

                | "-mini" ->
                    Configuration.IsMiniMode <- true
                | "-i" ->
                    currentIndex <- currentIndex + 1
                    let files = Directory.GetFiles(argv.[currentIndex], "*.cs", SearchOption.AllDirectories)
                    for file in files do
                        yield file
                | file ->
                    yield file

                currentIndex <- currentIndex + 1
                if currentIndex >= argv.Length then
                    isRunning <- false
        }

        let openedFiles = seq {
            for file in inputFiles do
                if not (File.Exists(file)) then
                    printfn "File not found: %s" file
                    raise (FileNotFoundException("Input file not found", file))
                else
                    let fileContent = readFileContent file
                    if (fst fileContent) then
                        yield (snd fileContent, file)
                    else
                        printfn "Cannot read file: %s" file
                        raise (IOException(file))
        }

        let result = openedFiles |> Seq.map (fun (fileContent, filePath) -> new Kaleidoscope.SourceTextFile(fileContent, filePath))
        Configuration.InputFiles <- Some(result.ToArray())

        0
    with
        | _ -> -1


let readArgumentsFromRspFile filePath = 
    try
        let argv = File.ReadAllLines(filePath)
        processCommandArguments argv
    with
        | _ ->
            printfn "Cannot open input file: %s" filePath
            -1


[<EntryPoint>]
let main argv = 
    if argv.Length = 0 then
        printf "%s" CommandLineHelpString
        -1
    else
        if argv.Length = 1 && argv.[0].[0] = '@' then
            readArgumentsFromRspFile argv.[0]
        else
            processCommandArguments argv

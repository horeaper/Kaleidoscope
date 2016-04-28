namespace Kaleidoscope

type SourceTextFile (fileContent:string, filePath:string) =
    member this.FileContent = fileContent
    member this.FilePath = filePath

    override this.ToString() = 
        sprintf "[SourceTextFile] %s" this.FilePath
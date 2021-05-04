module Vynchronizer.Core.Resource


open System
open System.IO

[<Struct>]
type public ResourceType =
    | UnknownType
    | Library
    | Executable
    | Text
    | Image
    | Video
    | Music
    | Document

[<Struct>]
type public ResourceStorage =
    | UnknownStorage
    | LocalFileSystem
    | GoogleDrive

type public ResourcePath = {
    Value: string
}

type public ResourceExtension = {
    Value: string
}

type public ResourceMetadata = {
    Path: ResourcePath
    Name: string
    Extension: ResourceExtension
    SizeInBytes: bigint
    Type: ResourceType
    Created: DateTime
    Modified: DateTime
    Accessed: DateTime
    Attributes: FileAttributes
}

let private unknownResourceTypes = [ "*.*" ]
let private libraryResourceTypes = [ "*.dll"; "*.lib"; "*.ocx"; "*.drv" ]
let private executableResourceTypes = [ "*.exe"; "*.msi" ]
let private textResourceTypes = [ "*.txt"; "*.md" ]
let private imageResourceTypes = [ "*.png"; "*.jpg"; "*.jpeg"; "*.bmp"; "*.jpe"; "*.jfif" ]
let private videoResourceTypes = [ "*.mkv"; "*.mp4"; "*.flv"; "*.avi"; "*.mov"; "*.3gp" ]
let private musicResourceTypes = [ "*.mp3"; "*.flac"; "*.alac"; "*.wav"; "*.aac"; "*.dsd"; "*.aiff"; "*.ogg" ]
let private documentResourceTypes = [ "*.doc"; "*.docx"; "*.xlsx"; "*.xlsm"; "*.xlsb"; "*.xltx"; "*.xltm" ]

let public getPatterns resourceType =
    match resourceType with
        | UnknownType -> unknownResourceTypes
        | Library -> libraryResourceTypes
        | Executable -> executableResourceTypes
        | Text  -> textResourceTypes
        | Image -> imageResourceTypes
        | Video -> videoResourceTypes
        | Music -> musicResourceTypes
        | Document -> documentResourceTypes

let private tryGetValue valueToFind returnIfFound collection =
    let someValueOrNone = collection |> List.tryFind (fun item -> item = valueToFind)
    match someValueOrNone with
        | None -> None
        | Some _ -> Some returnIfFound

let public getResourceTypeByExtension (extension: ResourceExtension) =
    let fixedExtension =
        if extension.Value.StartsWith(".", StringComparison.Ordinal) then $"*{extension.Value}"
        else extension.Value

    let resourceTypeOrNone = tryGetValue fixedExtension Library libraryResourceTypes
                            |> Option.orElse (tryGetValue fixedExtension Executable executableResourceTypes)
                            |> Option.orElse (tryGetValue fixedExtension Text textResourceTypes)
                            |> Option.orElse (tryGetValue fixedExtension Image imageResourceTypes)
                            |> Option.orElse (tryGetValue fixedExtension Video videoResourceTypes)
                            |> Option.orElse (tryGetValue fixedExtension Music musicResourceTypes)
                            |> Option.orElse (tryGetValue fixedExtension Document documentResourceTypes)

    match resourceTypeOrNone with
        | None -> UnknownType
        | Some resourceType -> resourceType

let public tryGetMetadataFromLocalFile (path: ResourcePath) =
    match (File.Exists path.Value) with
        | false -> Error $"File {path.Value} was not found."
        | true ->
            let fileInfo = FileInfo(path.Value)
            let (path: ResourcePath) = { Value = fileInfo.FullName }
            let (extension: ResourceExtension) = { Value = fileInfo.Extension }
            let result = {
                Path = path
                Name = fileInfo.Name
                Extension = extension
                SizeInBytes = bigint fileInfo.Length
                Type = getResourceTypeByExtension extension
                Created = fileInfo.CreationTimeUtc
                Modified =  fileInfo.LastWriteTimeUtc
                Accessed = fileInfo.LastAccessTimeUtc
                Attributes = fileInfo.Attributes
            }
            Ok result

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

[<Struct>]
type public SuccessOperationResultType =
    | DataWereSynchronized
    | DataWereNewer

type public SuccessOperationResult = {
    SucceessType: SuccessOperationResultType
    Message: string
}

[<Struct>]
type public FailedOperationResultType =
    | FailedToGetSourceMetadata
    | FailedToGetTargetMetadata
    | FailedToWriteDataToTarget

type public FailedOperationResult = {
    FailedType: FailedOperationResultType
    Message: string
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

let public wrapErrorToOperationResult failedType error =
    let failedOperationResult = {
        FailedType = failedType
        Message = error
    }
    failedOperationResult

module Vynchronizer.Core.Target


open Vynchronizer.Core.Source
open Vynchronizer.Core.Resource

type public TargetSpec = {
    StorageType: ResourceStorage
    Path: ResourcePath
}

type public OperationResult = {
    Success: bool
    Message: string
}

let public tryGetTagetMetadataFromLocalFile (targetSpec: TargetSpec) =
     tryGetMetadataFromLocalFile targetSpec.Path

let public writeDataToTargetLocalFile (targetMetadata: ResourceMetadata) (targetSpec: TargetSpec) (dataSource: DataSource<'TData>) =
    printfn $"{targetMetadata}" // TODO: replace with logger.
    let operationResult = {
        Success = true
        Message = "This is success!"
    }
    Ok operationResult

let public writeDataToLocalFile (targetSpec: TargetSpec) (dataSource: DataSource<'TData>) =
    let tagetMetadataOrError = tryGetTagetMetadataFromLocalFile targetSpec
    match tagetMetadataOrError with
        | Ok tagetMetadata -> writeDataToTargetLocalFile tagetMetadata targetSpec dataSource
        | Error error -> Error error

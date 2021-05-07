module Vynchronizer.Core.Target


open System.IO
open Vynchronizer.Core.Source
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Local.AsyncFileReader

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

    async {
        use stream1 = dataSource.Data dataSource.Metadata
        use stream2 = File.Open(targetMetadata.Path.Value, FileMode.Truncate) // Reset content of target file.
        let readSeq1 = readInBlocks stream1 dataSource.Spec.BlockSize
        let writeSeq2 = writeInBlocks stream2

        let! results = copyBlocks readSeq1 writeSeq2 Seq.empty

        let isSuccess = results |> Seq.forall (fun item -> item)
        let message =
            if isSuccess then "Local file content was copied successfully."
            else "Failed to copy local file content."

        let operationResult = {
            Success = isSuccess
            Message = message
        }
        return Ok operationResult
    }


let public writeDataToLocalFile (targetSpec: TargetSpec) (dataSource: DataSource<'TData>) =
    let tagetMetadataOrError = tryGetTagetMetadataFromLocalFile targetSpec

    async {
        match tagetMetadataOrError with
            | Ok tagetMetadata -> return! writeDataToTargetLocalFile tagetMetadata targetSpec dataSource
            | Error error -> return Error error
    }

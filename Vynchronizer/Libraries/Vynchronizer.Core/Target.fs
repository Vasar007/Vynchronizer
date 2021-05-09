module Vynchronizer.Core.Target

open System.IO
open Vynchronizer.Core.Metadata
open Vynchronizer.Core.Source
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Utils
open Vynchronizer.Core.Local.AsyncFileReader


[<Struct>]
type public ConflictResolutionPolicy =
    | ReplaceAll
    | ReplaceIfNewer

type public TargetSpec = {
    StorageType: ResourceStorage
    Path: ResourcePath
    ConflictResolution: ConflictResolutionPolicy
}

let public tryGetTagetMetadataFromLocalFile (targetSpec: TargetSpec) =
     tryGetMetadataFromLocalFile targetSpec.Path
        |> Result.map logObject
        |> Result.mapError (wrapErrorToOperationResult FailedToGetTargetMetadata)

let public writeDataToTargetLocalFileAsync (targetSpec: TargetSpec) (dataSource: DataSource<'TData>) (targetMetadata: ResourceMetadata) =
    async {
        use stream1 = dataSource.Data dataSource.Metadata
        use stream2 = File.Open(targetMetadata.Path.Value, FileMode.Truncate) // Reset content of target file.
        let readSeq1 = readInBlocksAsync stream1 dataSource.Spec.BlockSize
        let writeSeq2 = writeInBlocksAsync stream2

        let! results = copyBlocksAsync readSeq1 writeSeq2 Seq.empty

        let isSuccess = results |> Seq.forall (fun item -> item)
        if isSuccess then
            let successOperationResult = {
                SucceessType = DataWereSynchronized
                Message = "Local file content was copied successfully."
            }
            return Ok successOperationResult
        else
            let failedOperationResult = {
                FailedType = FailedToWriteDataToTarget
                Message = "Failed to copy local file content."
            }
            return Error failedOperationResult
    }

module Vynchronizer.Core.Operator

open System
open Vynchronizer.Core.Metadata
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Source
open Vynchronizer.Core.Target


let private shouldSynchonizeSourceToTarget sourceMetadata targetMetadata =
    match compareMetadata sourceMetadata targetMetadata with
        | SourceIsNewer -> true // Source is newer, should copy data.
        | TargetIsNewer -> false // Target is newer, skip copying.
        | HaveDifferentSizes -> true // Source and target have different sizes, should copy data.
        | Other -> false // Some other result, skip copying.

let private shouldSynchonizeDataDependOnConfig targetSpec sourceMetadata targetMetadata =
    match targetSpec.ConflictResolution with
        | ReplaceAll -> true
        | ReplaceIfNewer -> shouldSynchonizeSourceToTarget sourceMetadata targetMetadata

let private getSourceData (sourceSpec: SourceSpec) =
    match sourceSpec.StorageType with
        | ResourceStorage.UnknownStorage -> failwith "Unknown source resource storage."
        | ResourceStorage.LocalFileSystem -> processDataFromLocalFile
        | ResourceStorage.GoogleDrive -> raise (NotSupportedException("Google Drive storage is not supported."))

let private getTargetWriter (targetSpec: TargetSpec) =
    match targetSpec.StorageType with
        | ResourceStorage.UnknownStorage -> failwith "Unknown target resource storage."
        | ResourceStorage.LocalFileSystem -> struct (tryGetTagetMetadataFromLocalFile, writeDataToTargetLocalFileAsync)
        | ResourceStorage.GoogleDrive -> raise (NotSupportedException("Google Drive storage is not supported."))

// TODO: add rule to specify when we should copy data.
let public processSpecsAsync sourceSpec targetSpec =
    let source = getSourceData sourceSpec
    let struct (getTargetMetadata, writeDataToTargetAsync) = getTargetWriter targetSpec

    async {
        let dataSourceOrError = source sourceSpec

        let tryGetTargetMetadataWithSource dataSource =
            getTargetMetadata targetSpec
                |> Result.bind (fun targetMetadata -> Ok struct (dataSource, targetMetadata))

        let writeDataToTarget struct (dataSource, targetMetadata) =
            async {
                if shouldSynchonizeDataDependOnConfig targetSpec dataSource.Metadata targetMetadata then
                    return! writeDataToTargetAsync targetSpec dataSource targetMetadata
                else
                    let successOperationResult = {
                        SucceessType = DataWereNewer
                        Message = "Local file content was not copied because target data is newer."
                    }
                    return Ok successOperationResult
            }

        return! dataSourceOrError
            |> Result.bind tryGetTargetMetadataWithSource
            |> Result.bindAsync writeDataToTarget
    }

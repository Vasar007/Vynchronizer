module Vynchronizer.Core.Source

open System.IO
open Vynchronizer.Core.Metadata
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Utils
open Vynchronizer.Core.Local.AsyncFileReader


type public SourceSpec = {
    StorageType: ResourceStorage
    Path: ResourcePath
    BlockSize: int
}

type public DataEnumerable = Async<AsyncReadSeqInner<byte[]>>

type public DataSource<'TData> = {
    Spec: SourceSpec
    Metadata: ResourceMetadata
    Data: ResourceMetadata -> 'TData
}

let public tryGetSourceMetadataFromLocalFile (sourceSpec: SourceSpec) =
    tryGetMetadataFromLocalFile sourceSpec.Path
        |> Result.map logObject
        |> Result.mapError (wrapErrorToOperationResult FailedToGetSourceMetadata)

let public getDataSourceFromLocalFile (sourceMetadata: ResourceMetadata) (sourceSpec: SourceSpec) =
    let sourceData = fun (metadata: ResourceMetadata) -> File.OpenRead(metadata.Path.Value)
    let dataSource = {
        Spec = sourceSpec
        Metadata = sourceMetadata
        Data = sourceData
    }
    Ok dataSource

let public processDataFromLocalFile (sourceSpec: SourceSpec) =
    let sourceMetadataOrError = tryGetSourceMetadataFromLocalFile sourceSpec
    match sourceMetadataOrError with
        | Ok sourceMetadata -> getDataSourceFromLocalFile sourceMetadata sourceSpec
        | Error error -> Error error

module Vynchronizer.Core.Source


open Vynchronizer.Core.Resource

type public SourceSpec = {
    StorageType: ResourceStorage
    Path: ResourcePath
}

type public DataEnumerable<'TData> =
    | SincEnumerable of seq<'TData>
    | AsyncEnumerable of Async<seq<'TData>>

type public DataSource<'TData> = {
    SourceData: DataEnumerable<'TData>
}

let public tryGetSourceMetadataFromLocalFile (sourceSpec: SourceSpec) =
    tryGetMetadataFromLocalFile sourceSpec.Path

let public getDataSourceFromLocalFile (sourceMetadata: ResourceMetadata) (sourceSpec: SourceSpec) =
    printfn $"{sourceMetadata}" // TODO: replace with logger.
    let dataSource = {
        SourceData = SincEnumerable Seq.empty
    }
    Ok dataSource

let public processDataFromLocalFile (sourceSpec: SourceSpec) =
    let sourceMetadataOrError = tryGetSourceMetadataFromLocalFile sourceSpec
    match sourceMetadataOrError with
        | Ok sourceMetadata -> getDataSourceFromLocalFile sourceMetadata sourceSpec
        | Error error -> Error error

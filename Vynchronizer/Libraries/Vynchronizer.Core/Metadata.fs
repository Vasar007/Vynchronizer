module Vynchronizer.Core.Metadata

open System
open System.IO
open Vynchronizer.Core.Resource


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

[<Struct>]
type public MetadataComparisonResult =
    | SourceIsNewer
    | TargetIsNewer
    | HaveDifferentSizes
    | Other

let public compareMetadata sourceMetadata targetMetadata =
    match sourceMetadata, targetMetadata with
        | source, target when source.Modified > target.Modified -> SourceIsNewer
        | source, target when source.Modified < target.Modified -> TargetIsNewer
        | source, target when source.SizeInBytes <> target.SizeInBytes -> HaveDifferentSizes
        | _ -> Other

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

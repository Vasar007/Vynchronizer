module Vynchronizer.Core.Resource


open System

[<Struct>]
type ResourceType =
    | Unknown
    | Library
    | Executable
    | Text
    | Image
    | Music
    | Document

[<Struct>]
type ResourceStorage =
    | Unknown
    | LocalFileSystem
    | GoogleDrive

type ResourceMetadata = {
    Path: string
    Name: string
    Extension: string
    SizeInBytes: bigint
    Type: ResourceType
    Created: DateTime
    Modified: DateTime
    Accessed: DateTime
    Attributes: seq<string>
}

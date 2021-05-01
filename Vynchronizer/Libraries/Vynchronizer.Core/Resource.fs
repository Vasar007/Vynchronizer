module Vynchronizer.Core.Resource


open System

[<Struct>]
type public ResourceType =
    | Unknown
    | Library
    | Executable
    | Text
    | Image
    | Music
    | Document

[<Struct>]
type public ResourceStorage =
    | Unknown
    | LocalFileSystem
    | GoogleDrive

type public ResourceMetadata = {
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

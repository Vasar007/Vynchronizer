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

// TODO: refactor to functional style.
type IResource =
    abstract member GetMetadata: returnLatest: bool -> ResourceMetadata

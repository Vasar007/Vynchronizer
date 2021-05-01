module Vynchronizer.Core.Source


open System
open Vynchronizer.Core.Resource

type public SourceSpec = {
    StorageType: ResourceStorage
}

type public DataSource<'TData> = {
    Enumerator: seq<'TData>
}

let public getDummySourceMetadata returnLatest =
    let result = {
        Path = "Path"
        Name = "Name"
        Extension = "Extension"
        SizeInBytes = new bigint(42)
        Type = ResourceType.Unknown
        Created = DateTime.Now
        Modified =  DateTime.Now
        Accessed = DateTime.Now
        Attributes = Seq.empty
    }
    result

let public getDummyDataFromSource sourceSpec =
    let dataSource = {
        Enumerator = Seq.empty
    }
    dataSource

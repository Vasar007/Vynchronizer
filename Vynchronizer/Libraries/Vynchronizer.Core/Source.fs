module Vynchronizer.Core.Source


open System
open Vynchronizer.Core.Resource

type DataSource<'TData> = {
    Enumerator: seq<'TData>
}

// TODO: refactor to functional style.
type ISource<'TData> =
    inherit IResource
    abstract member GetData: unit -> DataSource<'TData>

type DummySource<'TData>() =
    interface ISource<'TData> with
        member _.GetMetadata returnLatest =
            let result = {
                Path = "Path"
                Name = "Name"
                Extension = "Extension"
                Size = new bigint(42)
                Type = ResourceType.Unknown
                Created = DateTime.Now
                Modified =  DateTime.Now
                Accessed = DateTime.Now
                Attributes = Seq.empty
            }
            result

        member _.GetData() =
            let dataSource = {
                Enumerator = Seq.empty
            }
            dataSource

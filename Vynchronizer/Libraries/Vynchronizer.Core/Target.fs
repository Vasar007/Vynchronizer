module Vynchronizer.Core.Target


open System
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Source

type OperationResult = {
    Success: bool
    Message: string
}

// TODO: refactor to functional style.
type ITarget<'TData> =
    inherit IResource
    abstract member WriteData: dataSource: DataSource<'TData> -> Result<OperationResult, Exception>

type DummyTarget<'TData>() =
    interface ITarget<'TData> with
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

        member _.WriteData dataSource =
            let operationResult = {
                Success = true
                Message = "This is success!"
            }
            Ok operationResult

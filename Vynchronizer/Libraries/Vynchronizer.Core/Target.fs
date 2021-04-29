module Vynchronizer.Core.Target


open System
open Vynchronizer.Core.Resource

type TargetSpec = {
    DummyTarget: unit
}

type OperationResult = {
    Success: bool
    Message: string
}

let getTargetMetadata returnLatest =
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

let writeDataToTarget targetSpec dataSource =
    let operationResult = {
        Success = true
        Message = "This is success!"
    }
    Ok operationResult

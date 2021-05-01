module Vynchronizer.Core.Target


open System
open Vynchronizer.Core.Resource

type public TargetSpec = {
    StorageType: ResourceStorage
}

type public OperationResult = {
    Success: bool
    Message: string
}

let public getDummyTargetMetadata returnLatest =
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

let public writeDummyDataToTarget targetSpec dataSource =
    let operationResult = {
        Success = true
        Message = "This is success!"
    }
    Ok operationResult

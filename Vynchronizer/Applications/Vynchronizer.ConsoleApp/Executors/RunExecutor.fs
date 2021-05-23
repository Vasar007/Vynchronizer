module Vynchronizer.ConsoleApp.Executors.RunExecutor

open Vynchronizer.ConsoleApp
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Source
open Vynchronizer.Core.Target
open Vynchronizer.Core.Operator


let private executeForSpecsAsync sourceSpec targetSpec =
    printfn "Executing operation for source and target."

    async {
        let! operationResult = processSpecsAsync sourceSpec targetSpec
        match operationResult with
            | Ok success -> printfn $"Success operation: {success.SucceessType.ToString()}, message: {success.Message}"
            | Error failed -> printfn $"Failed operation: {failed.FailedType.ToString()}, message: {failed.Message}"

        return ExitCodes.successExitCode
    }

let internal executeRunCommandAsync runOptions =
    let (sourceSpec: SourceSpec) = {
        StorageType = ResourceStorage.LocalFileSystem
        Path = { Value = "1.txt" }
        BlockSize = 1000
    }

    let (targetSpec: TargetSpec) = {
        StorageType = ResourceStorage.LocalFileSystem
        Path = { Value = "2.txt" }
        ConflictResolution = ReplaceIfNewer
    }

    executeForSpecsAsync sourceSpec targetSpec

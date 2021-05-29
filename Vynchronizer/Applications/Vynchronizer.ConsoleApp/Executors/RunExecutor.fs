module Vynchronizer.ConsoleApp.Executors.RunExecutor

open Vynchronizer.ConsoleApp
open Vynchronizer.ConsoleApp.Options.RuleOptions
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

let internal executeRunCommandAsync (runOptions: ExecuteRuleOptions) =
    let sourceStorageType = convertResourceStorage runOptions.SourceStorageType
    let targetStorageType = convertResourceStorage runOptions.TargetStorageType
    let targetConflictResolution = convertConflictResolutionPolicy runOptions.TargetConflictResolution

    let (sourceSpec: SourceSpec) = {
        StorageType = sourceStorageType
        Path = { Value = runOptions.SourcePath }
        BlockSize = runOptions.SourceBlockSize
    }

    let (targetSpec: TargetSpec) = {
        StorageType = targetStorageType
        Path = { Value = runOptions.TargetPath }
        ConflictResolution = targetConflictResolution
    }

    executeForSpecsAsync sourceSpec targetSpec

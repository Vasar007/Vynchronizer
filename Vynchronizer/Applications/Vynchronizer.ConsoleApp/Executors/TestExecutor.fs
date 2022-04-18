module Vynchronizer.ConsoleApp.Executors.TestExecutor

open Acolyte.Linq
open Vynchronizer.ConsoleApp
open Vynchronizer.ConsoleApp.Options.TestOptions
open Vynchronizer.Core.Local.AsyncFileReader
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Source
open Vynchronizer.Core.Target
open Vynchronizer.Core.Operator


let private sampleOfComparing =
    async {
        let file1 = "1.txt"
        let file2 = "2.txt"
        let size = 1000
        printfn $"Comparing two files {file1} and {file2} with size blocks {size.ToString()}"

        let! comareResult = compareFilesAsync file1 file2 size
        printfn $"Comparison result: {comareResult.ToString()}"

        return ExitCodes.successExitCode
    }

let private sampleOfCopying =
    async {
        let file1 = "1.txt"
        let file2 = "2.txt"
        let size = 1000
        printfn $"Copying data between two files {file1} and {file2} with size blocks {size.ToString()}"

        let! comareResult = copyDataAsync file1 file2 size
        printfn $"Comparison result: {comareResult.ToSingleString()}"

        return ExitCodes.successExitCode
    }

let private sampleOfOperator =
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

    printfn "Executing operation for source and target."

    async {
        let! operationResult = processSpecsAsync sourceSpec targetSpec
        match operationResult with
            | Ok success -> printfn $"Success operation: {success.SucceessType.ToString()}, message: {success.Message}"
            | Error failed -> printfn $"Failed operation: {failed.FailedType.ToString()}, message: {failed.Message}"

        return ExitCodes.successExitCode
    }

let internal executeTestCommandAsync testOptions =
    async {
        let convertedCase = convertTestCase testOptions.Case
        match convertedCase with
            | Comparing -> return! sampleOfComparing
            | Copying -> return! sampleOfCopying
            | Operator -> return! sampleOfOperator
    }

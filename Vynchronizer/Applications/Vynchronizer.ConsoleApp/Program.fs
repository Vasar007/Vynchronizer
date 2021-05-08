module Vynchronizer.ConsoleApp.Program


open System
open Acolyte.Collections
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

        return 0
    }

let private sampleOfCopying =
    async {
        let file1 = "1.txt"
        let file2 = "2.txt"
        let size = 1000
        printfn $"Copying data between two files {file1} and {file2} with size blocks {size.ToString()}"

        let! comareResult = copyDataAsync file1 file2 size
        printfn $"Comparison result: {comareResult.ToSingleString()}"

        return 0
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

        return 0
    }


let private asyncMain (args: string[]) =
    async {
        let option = 2

        match option with
            | 0 -> return! sampleOfComparing
            | 1 -> return! sampleOfCopying
            | 2 -> return! sampleOfOperator
            | _ -> return 0
    }

[<EntryPoint>]
let private main args =
    try
        try
            printfn "Vynchronizer started."
    
            asyncMain args |> Async.RunSynchronously // return an integer exit code.
        with
            | ex ->
                printfn $"Exception occurred: {ex}"
                1 // return an integer exit code.
    finally
        printfn "Vynchronizer stopped."
        printfn "Press any key to close this window..."
        Console.ReadKey() |> ignore

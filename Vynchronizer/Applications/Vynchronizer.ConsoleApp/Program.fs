module Vynchronizer.ConsoleApp.Program


open System
open Vynchronizer.Core.Local.AsyncFileReader
open Vynchronizer.Core.Resource
open Vynchronizer.Core.Source
open Vynchronizer.Core.Target
open Vynchronizer.Core.Operator

let sampleOfComparing =
    async {
        let file1 = "1.txt"
        let file2 = "2.txt"
        let size = 1000
        printfn $"Comparing two files {file1} and {file2} with size blocks {size.ToString()}"

        let! comareResult = compareFiles file1 file2 size
        printfn "Comparison result: %b" comareResult

        return 0
    }

let sampleOfOperator =
    let source = new DummySource<byte>()
    let target = new DummyTarget<byte>()
    printfn "Executing operation for source and target."

    let operationResult = copyData source target
    match operationResult with
        | Ok result -> printfn $"Operation result: {result.Success.ToString()}, message: {result.Message}"
        | Error error -> printfn $"Error: {error}"

    0

let asyncMain (args: string[]) =
    async {
        let option = 1

        match option with
            | 0 -> return! sampleOfComparing
            | 1 -> return sampleOfOperator
            | _ -> return 0
    }

[<EntryPoint>]
let main args =
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
